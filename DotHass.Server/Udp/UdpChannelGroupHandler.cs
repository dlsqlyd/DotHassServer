using DotHass.Server.Abstractions;
using DotHass.Server.Abstractions.Channels;
using DotHass.Server.Abstractions.Events;
using DotHass.Server.Abstractions.Message;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System.Threading.Tasks;

namespace DotHass.Server.Udp
{
    /// <summary>
    /// 1.udp,需要客户端连接后.主动发送一次握手包
    /// 2.服务端再开启一个新的udpsocker,在新的socker向客户端发送握手...建立一条新的socker
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UdpChannelGroupHandler : SimpleChannelInboundHandler<DatagramPacket>
    {
        private volatile IConnectionChannelGroup channelGroup;
        private IMessageFactory messageFactory;
        private readonly ServerOptions _options;

        public UdpChannelGroupHandler(IConnectionChannelGroup group, IMessageFactory messageFactory, ServerOptions Options)
        {
            this.channelGroup = group;
            this.messageFactory = messageFactory;
            this._options = Options;
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            //用户断开连接事件。。包括超时。客户端主动断开
            if (evt is ClientDisconnectEvent)
            {
                var eventState = evt as ClientDisconnectEvent;
                var child = eventState.Channel;
                channelGroup.Remove(child);
            }
            else
            {
                context.FireUserEventTriggered(evt);
            }
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket msg)
        {
            var message = messageFactory.Parse(msg.Content);
            if (message == null || int.Parse(message.Headers.Get(Constants.Headers.ContractId)) != (int)ContractType.HandShake)
            {
                return;
            }
            IChannel child = new UdpSocketDatagramChannel(ctx.Channel, msg.Sender);
            var connectionInfo = channelGroup.FindConnectionInfo(child);
            if (connectionInfo == null)
            {
                channelGroup.Add(new ConnectionInfo(child, this._options));
            }
            ctx.FireChannelRead(child);
        }

        public override Task CloseAsync(IChannelHandlerContext context)
        {
            //虽然在eventloopclose的时候会关闭所有和他有关的channel（ServerBootstrapAcceptor）
            //但是这里是当主的关闭的时候触发
            channelGroup.CloseAsync();
            return base.CloseAsync(context);
        }

        public override bool IsSharable => true;
    }
}