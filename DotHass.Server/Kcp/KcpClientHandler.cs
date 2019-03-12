using DotHass.Server.Abstractions.Events;
using DotHass.Server.Udp;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace DotHass.Server.Kcp
{
    public class KcpClientHandler : SimpleChannelInboundHandler<DatagramPacket>
    {
        public KcpClientHandler() : base(false)
        {
        }

        /// <summary>
        /// 向客户端发送一个握手消息,分配clientid
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelActive(IChannelHandlerContext context)
        {
            context.Channel.Pipeline.FireUserEventTriggered(new HandShakeEvent());
        }

        //channel.Pipeline.DisconnectAsync(超时)会触发inactive和unregistered。。。
        //由于是udp客户端主动关闭(closeasync)不会有任何反应
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            (context.Channel as UdpSocketDatagramChannel).Parent.Pipeline.FireUserEventTriggered(new ClientDisconnectEvent() { Channel = context.Channel });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="msg"></param>
        protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket msg)
        {
            //https://github.com/skywind3000/kcp/wiki/Cooperate-With-Tcp-Server
            if (msg.Sender.ToString() != ctx.Channel.RemoteAddress.ToString())
            {
                return;
            }
            //这里的msg.Content是被kcp封装过的.
            ctx.FireChannelRead(msg.Content);
        }

        public override bool IsSharable => true;
    }
}