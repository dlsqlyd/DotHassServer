using DotHass.Server.Abstractions.Channels;
using DotHass.Server.Abstractions.Events;
using DotHass.Server.Abstractions.Message;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;

namespace DotHass.Server.Handler
{
    /// <summary>
    /// dotnetty 服务端消息读取器，此处将消息封装成MessagePacket，再交由MessageDispatcher处理
    /// </summary>
    public class MessageChannelHandler : SimpleChannelInboundHandler<IRequestMessage>
    {
        private IMessageHandler messageHandler;
        private IMessageFactory messageFactory;
        private IConnectionChannelGroup channelGroup;
        private readonly ILogger<MessageChannelHandler> logger;

        public MessageChannelHandler(IMessageHandler customizeHandler, IMessageFactory messageFactory, IConnectionChannelGroup group, ILogger<MessageChannelHandler> logger)
        {
            this.messageHandler = customizeHandler;
            this.messageFactory = messageFactory;
            this.channelGroup = group;
            this.logger = logger;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
            this.messageHandler.HandlerActive(context.Channel);
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            //心跳超时状态
            if (evt is HeartBeatTimeoutEvent)
            {
                var eventState = evt as HeartBeatTimeoutEvent;
                messageHandler.HandlerTimeout(eventState.Channel);
            }
            else if (evt is HandShakeEvent)
            {
                //当客户端连接成功后.会触发握手事件.然后主动向客户端推送sid和clienid
                var response = messageFactory.CreateHandShake(channelGroup.FindConnectionInfo(context.Channel));
                context.Channel.WriteAndFlushAsync(response);
            }
            context.FireUserEventTriggered(evt);
        }

        /// <summary>
        /// https://github.com/Azure/DotNetty/issues/265
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="msg"></param>
        protected override async void ChannelRead0(IChannelHandlerContext ctx, IRequestMessage msg)
        {
            try
            {
                switch (int.Parse(msg.Headers.Get(Constants.Headers.ContractId)))
                {
                    case (int)ContractType.HandShake:
                        //握手,实际上在childchannel永远不会接受到握手消息
                        break;

                    case (int)ContractType.HeartBeat:
                        //心跳直接忽略
                        break;

                    default:
                        await messageHandler.Handler(ctx.Channel, msg);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "message handler error");
            }
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            this.messageHandler.HandlerInactive(context.Channel);
            base.ChannelInactive(context);
        }

        public override void ExceptionCaught(IChannelHandlerContext ctx, Exception e)
        {
            messageHandler.HandlerException(ctx.Channel, e);
        }

        public override bool IsSharable => true;
    }
}