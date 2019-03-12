using DotHass.Server.Abstractions.Message;
using DotNetty.Buffers;
using System;
using System.Collections.Specialized;

namespace DotHass.Server.Message
{
    public class DefaultResponseMessagePacket : IResponseMessage
    {
        private MessageHeader _header { get; set; }

        public NameValueCollection Headers => _header;

        public byte[] BodyContent { get; set; }
        protected int BodyLength { get { return Convert.ToInt32(_header.Get(Constants.Headers.ContentLength)); } }

        public DefaultResponseMessagePacket(int stateCode, int contractID, int messageID, byte[] body)
        {
            this.BodyContent = body ?? new byte[0];
            this._header = new MessageHeader();
            this.AddHeader(Constants.ResponseStatusCode, stateCode.ToString());
            this.AddHeader(Constants.Headers.MsgId, messageID.ToString());
            this.AddHeader(Constants.Headers.ContractId, contractID.ToString());
            this.AddHeader(Constants.Headers.ContentLength, this.BodyContent.Length.ToString());
        }

        public void AddHeader(string key, string value)
        {
            this.Headers.Set(key, value);
        }

        public void SerializeHeader(IByteBuffer buffer)
        {
            buffer.WriteInt(Convert.ToInt32(_header.Get(Constants.ResponseStatusCode)));
            buffer.WriteInt(Convert.ToInt32(_header.Get(Constants.Headers.ContractId)));
            buffer.WriteInt(Convert.ToInt32(_header.Get(Constants.Headers.MsgId)));
            buffer.WriteInt(Convert.ToInt32(_header.Get(Constants.Headers.ContentLength)));
        }

        public IByteBuffer ToAllBuffer()
        {
            var buffer = Unpooled.Buffer(20 + this.BodyLength);
            SerializeHeader(buffer);
            if (this.BodyContent != null)
            {
                buffer.WriteBytes(this.BodyContent);
            }
            return buffer;
        }

        public IByteBuffer ToBodyBuffer()
        {
            var buffer = Unpooled.Buffer(this.BodyLength);
            if (this.BodyContent != null)
            {
                buffer.WriteBytes(this.BodyContent);
            }

            return buffer;
        }

        /// <summary>
        /// 消息头
        /// </summary>
        public class MessageHeader : NameValueCollection
        {
        }
    }
}