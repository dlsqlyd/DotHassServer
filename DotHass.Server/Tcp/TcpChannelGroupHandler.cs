using DotHass.Server.Abstractions;
using DotHass.Server.Abstractions.Channels;
using DotHass.Server.Abstractions.Events;
using DotNetty.Transport.Channels;
using System.Threading.Tasks;

namespace DotHass.Server.Tcp
{
    /// <summary>
    /// 管理clientchannelgroup
    /// 添加的时候会分配clientid
    /// </summary>
    public class TcpChannelGroupHandler : SimpleChannelInboundHandler<IChannel>
    {
        private volatile IConnectionChannelGroup channelGroup;
        private readonly ServerOptions _options;

        public TcpChannelGroupHandler(IConnectionChannelGroup group, ServerOptions Options)
        {
            channelGroup = group;
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

        protected override void ChannelRead0(IChannelHandlerContext ctx, IChannel child)
        {
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
            //但是这里是当主动关闭的时候触发
            channelGroup.CloseAsync();
            return base.CloseAsync(context);
        }

        public override bool IsSharable => true;
    }
}