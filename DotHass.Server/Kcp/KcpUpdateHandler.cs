using DotHass.Server.Abstractions.Channels;
using DotHass.Server.Abstractions.Message;
using DotHass.Server.Udp;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using kcpwarpper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static kcpwarpper.KCP;

namespace DotHass.Server.Kcp
{
    /// <summary>
    /// 该处理器不能共享
    /// </summary>
    public unsafe class KcpUpdateHandler : MessageToMessageDecoder<IByteBuffer>
    {
        private IMessageFactory messageFactory;
        private unsafe IKCPCB* kcp;
        private readonly KcpOptions options;
        private UdpSocketDatagramChannel channel;
        private ConcurrentQueue<byte[]> IncomingData = new ConcurrentQueue<byte[]>();
        private ConcurrentQueue<byte[]> OutgoingData = new ConcurrentQueue<byte[]>();
        private byte[] recbuf;

        private readonly ILogger<KcpUpdateHandler> logger;

        public bool IsRunning { get; internal set; } = false;

        public KcpUpdateHandler(IChannel channel, IConnectionChannelGroup group, IMessageFactory messageFactory, IOptions<KcpOptions> options, ILogger<KcpUpdateHandler> logger)
        {
            this.options = options.Value;
            this.recbuf = new byte[this.options.MAX_RECBUFF_LEN];
            this.channel = channel as UdpSocketDatagramChannel;
            this.messageFactory = messageFactory;
            this.logger = logger;
            this.Start((uint)group.FindClientID(channel));
        }

        #region 发送数据

        /// <summary>
        /// 发送数据  主要的操作接口
        /// </summary>
        /// <param name="data"></param>
        public virtual void SendOperationResponse(byte[] data)
        {
            if (data.Length == 0 || kcp == null)
            {
                return;
            }
            fixed (byte* b = &data[0])
            {
                ikcp_send(kcp, b, data.Length);
            }
        }

        /// <summary>
        /// 将buff构建成kcp包
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="len"></param>
        /// <param name="kcp"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        protected int Output(sbyte* buf, int len, IKCPCB* kcp, void* user)
        {
            byte[] kcppack = new byte[len];
            Marshal.Copy(new IntPtr(buf), kcppack, 0, len);
            OutgoingData.Enqueue(kcppack);
            return 0;
        }

        #endregion 发送数据

        #region 接受数据

        //使用channel.EventLoop.Execute(this.UpdateInternal);递归调用有问题
        /// 当客户端主动关闭,有消息未发送完会不停发送.然后挂掉..
        /// 由于是udp客户端主动关闭.是收不到断线处理的
        /// 如果schedule再有问题,改用while
        internal void UpdateInternal()
        {
            while (IncomingData.TryDequeue(out var buf))
            {
                BeforeOperationRequest(buf);
            }
            while (OutgoingData.TryDequeue(out var buf2))
            {
                this.channel.WriteAndFlushAsync(new DatagramPacket(Unpooled.Buffer(buf2.Length).WriteBytes(buf2), this.channel.RemoteAddress));
            }
            DeriverUpdate();
        }

        protected virtual void BeforeOperationRequest(byte[] buf)
        {
            if (buf.Length == 0 || this.kcp == null)
            {
                return;
            }
            fixed (byte* p = &buf[0])
            {
                ikcp_input(this.kcp, p, buf.Length);
            }
        }

        protected void DeriverUpdate()
        {
            if (kcp == null) return;
            fixed (byte* b = &recbuf[0])
            {
                int kcnt = 0;
                do
                {
                    kcnt = ikcp_recv(kcp, b, recbuf.Length);
                    if (kcnt > 0)
                    {
                        byte[] data = new byte[kcnt];
                        Array.Copy(recbuf, data, kcnt);
                        OnOperationRequest(data);
                    }
                } while (kcnt > 0);
            }

            try
            {
                ikcp_update(kcp, (uint)Environment.TickCount);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 处理请求数据
        /// </summary>
        /// <param name="data"></param>
        private void OnOperationRequest(byte[] data)
        {
            var message = messageFactory.Parse(Unpooled.Buffer(data.Length).WriteBytes(data));
            if (message == null)
            {
                return;
            }
            channel.Pipeline.FireChannelRead(message);
        }

        #endregion 接受数据

        protected override void Decode(IChannelHandlerContext ctx, IByteBuffer msg, List<object> output)
        {
            IncomingData.Enqueue(msg.Array);
        }

        public override Task WriteAsync(IChannelHandlerContext ctx, object msg)
        {
            if (msg is IResponseMessage cast)
            {
                var result = cast.ToAllBuffer();

                ///握手消息的话直接返回,因为此时客户端还有clientid.接受不了kcp消息
                if (int.Parse(cast.Headers.Get(Constants.Headers.ContractId)) == (int)ContractType.HandShake)
                {
                    return ctx.WriteAsync(result);
                }
                else
                {
                    this.SendOperationResponse(result.Array);
                    return Task.CompletedTask;
                }
            }
            else
            {
                return ctx.WriteAsync(msg);
            }
        }

        //channel.Pipeline.DisconnectAsync(超时)会触发inactive和unregistered。。。
        //由于是udp客户端主动关闭(closeasync)不会有任何反应
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            this.Stop();
            base.ChannelInactive(context);
        }

        public void Start(uint clientid)
        {
            if (this.IsRunning)
            {
                return;
            }

            this.CreateKcp(clientid);
            kcp->output = Marshal.GetFunctionPointerForDelegate(new d_output(Output));

            this.IsRunning = true;

            Task.Run(() =>
            {
                try
                {
                    while (this.IsRunning)
                    {
                        this.UpdateInternal();
                        SpinWait.SpinUntil(() => !this.IsRunning, this.options.UpdateInterval);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "kcp update error");
                }
            });
        }

        public void CreateKcp(uint clientid)
        {
            if (kcp == null)
            {
                var errno = ikcp_create(clientid, (void*)0);
                if ((int)errno != -1)
                {
                    kcp = errno;
                    /*该调用将会设置协议的最大发送窗口和最大接收窗口大小，默认为32. 这个可以理解为 TCP的 SND_BUF 和 RCV_BUF，只不过单位不一样 SND/RCV_BUF 单位是字节，这个单位是包。*/
                    ikcp_wndsize(kcp, this.options.SndWindowSize, this.options.RecWindowSize);

                    /*
                    nodelay ：是否启用 nodelay模式，0不启用；1启用。
                    interval ：协议内部工作的 interval，单位毫秒，比如 10ms或者 20ms
                    resend ：快速重传模式，默认0关闭，可以设置2（2次ACK跨越将会直接重传）
                    nc ：是否关闭流控，默认是0代表不关闭，1代表关闭。
                    普通模式：`ikcp_nodelay(kcp, 0, 40, 0, 0);
                    极速模式： ikcp_nodelay(kcp, 1, 10, 2, 1);
                     */
                    ikcp_nodelay(kcp, this.options.NoDelay, this.options.NoDelayInterval, this.options.NoDelayResend, this.options.NoDelayNC);

                    /*最大传输单元：纯算法协议并不负责探测 MTU，默认 mtu是1400字节，可以使用ikcp_setmtu来设置该值。该值将会影响数据包归并及分片时候的最大传输单元。*/
                    int mtu = Math.Min(this.options.MAX_DATA_LEN, this.options.MTU);
                    ikcp_setmtu(kcp, mtu);//可能还要浪费几个字节

                    /*最小RTO：不管是 TCP还是 KCP计算 RTO时都有最小 RTO的限制，即便计算出来RTO为40ms，由于默认的 RTO是100ms，协议只有在100ms后才能检测到丢包，快速模式下为30ms，可以手动更改该值：*/
                    kcp->rx_minrto = this.options.RTO;
                }
                else
                {
                    kcp = null;
                    throw new InvalidCastException($"kcp create failed {(int)errno}");
                }
            }
        }

        public void Stop()
        {
            if (!this.IsRunning)
            {
                return;
            }
            this.IsRunning = false;
            if (kcp != null)
            {
                Console.WriteLine($"release kcp {(int)kcp}");
                ikcp_release(kcp);
                kcp = null;
            }
        }

        public void Flush()
        {
            if (kcp == null)
            {
                return;
            }
            ikcp_flush(kcp);
        }

        public int WaitSend
        {
            get
            {
                if (kcp != null)
                {
                    return ikcp_waitsnd(kcp);
                }
                return -1;
            }
        }
    }
}