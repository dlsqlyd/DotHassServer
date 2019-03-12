using DotHass.Server.Abstractions;
using DotHass.Server.Abstractions.Message;
using DotNetty.Buffers;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DotHass.Server.Message
{
    /// <summary>
    /// 1.消息包，消息头+ 消息内容
    /// </summary>
    public class DefaultRequestMessagePacket : IRequestMessage
    {
        private MessageHeader _header { get; set; }

        public NameValueCollection Headers => _header;

        public byte[] BodyContent { get; set; }

        private Stream _inputStream;

        public Stream InputStream
        {
            get
            {
                if (_inputStream == null)
                {
                    _inputStream = new MemoryStream(this.BodyContent);
                }
                return _inputStream;
            }
        }

        public int ClientId => Convert.ToInt32(_header.Get(Constants.Headers.ClientId));
        public int Id => Convert.ToInt32(_header.Get(Constants.Headers.MsgId));
        public int ContractId => Convert.ToInt32(_header.Get(Constants.Headers.ContractId));
        public string Sign => _header.Get(Constants.Headers.SignKey);
        public string SessionId => _header.Get(Constants.Headers.SessionId);

        public DefaultRequestMessagePacket()
        {
        }

        public bool Parse(IByteBuffer buffer)
        {
            try
            {
                _header = ParseHeader(buffer);

                var messageBodyLen = Convert.ToInt32(_header.Get(Constants.Headers.ContentLength));

                //消息体由协议序列化后的内容构成
                if (messageBodyLen > 0)
                {
                    this.BodyContent = new byte[messageBodyLen];
                    buffer.ReadBytes(this.BodyContent);
                }
                else//保证protobuffer对象默认状态下可反序列化(protobuffer序列化对象默认值时，字节数组为0)
                {
                    this.BodyContent = new byte[0];
                }
                return true;
            }
            catch (Exception)
            {
            }

            return false;
        }

        public static MessageHeader ParseHeader(IByteBuffer buffer)
        {
            var header = new MessageHeader();
            //解析的顺序不能错
            var ClientId = buffer.ReadInt();
            var MessageId = buffer.ReadInt();
            var ContractId = buffer.ReadInt();
            var SessionId = buffer.ReadShortString(Encoding.UTF8);
            var Sign = buffer.ReadShortString(Encoding.UTF8);
            var MessageBodyLen = buffer.ReadInt();

            header.Set(Constants.Headers.ClientId, ClientId.ToString());
            header.Set(Constants.Headers.MsgId, MessageId.ToString());
            header.Set(Constants.Headers.ContractId, ContractId.ToString());
            header.Set(Constants.Headers.SessionId, SessionId.ToString());
            header.Set(Constants.Headers.SignKey, Sign.ToString());
            header.Set(Constants.Headers.ContentLength, MessageBodyLen.ToString());

            return header;
        }

        public bool Parse(string uri)
        {
            try
            {
                ParseUrl(uri, out string baseUrl, out NameValueCollection nvc);
                _header = new MessageHeader();
                _header.Set(Constants.Headers.ClientId, nvc.Get("clientid"));
                _header.Set(Constants.Headers.MsgId, nvc.Get("msgid"));
                _header.Set(Constants.Headers.ContractId, nvc.Get("contractid"));
                _header.Set(Constants.Headers.SessionId, nvc.Get("sid"));
                _header.Set(Constants.Headers.SignKey, nvc.Get("sign"));
                _header.Set(Constants.Headers.ContentLength, nvc.Get("len"));

                nvc.Remove("clientid");
                nvc.Remove("msgid");
                nvc.Remove("contractid");
                nvc.Remove("sid");
                nvc.Remove("sign");
                nvc.Remove("len");

                StringBuilder sb = new StringBuilder("");
                foreach (String s in nvc.AllKeys)
                {
                    sb.Append(s + "=" + nvc[s] + "&");
                }
                sb.Remove(sb.Length - 1, 1);

                if (sb.Length > 0)
                {
                    this.BodyContent = Encoding.UTF8.GetBytes(sb.ToString());
                }
                else//保证protobuffer对象默认状态下可反序列化(protobuffer序列化对象默认值时，字节数组为0)
                {
                    this.BodyContent = new byte[0];
                }

                return true;
            }
            catch (Exception)
            {
            }

            return false;
        }

        /// <summary>
        /// 分析 url 字符串中的参数信息
        /// 这里没有用下面的方法，因为url不完整。解析不了
        ///  Uri uri = new Uri(pURL);
        /// HttpUtility.ParseQueryString(uri.Query, System.Text.Encoding.UTF8);
        ///
        /// </summary>
        /// <param name="url">输入的 URL</param>
        /// <param name="baseUrl">输出 URL 的基础部分</param>
        /// <param name="nvc">输出分析后得到的 (参数名,参数值) 的集合</param>
        public void ParseUrl(string url, out string baseUrl, out NameValueCollection nvc)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            nvc = new NameValueCollection();
            baseUrl = "";

            if (url == "")
                return;

            int questionMarkIndex = url.IndexOf('?');

            if (questionMarkIndex == -1)
            {
                baseUrl = url;
                return;
            }
            baseUrl = url.Substring(0, questionMarkIndex);
            if (questionMarkIndex == url.Length - 1)
                return;
            string ps = url.Substring(questionMarkIndex + 1);

            // 开始分析参数对
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(ps);

            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
        }

        /// <summary>
        /// 消息头
        /// </summary>
        public class MessageHeader : NameValueCollection
        {
        }
    }
}