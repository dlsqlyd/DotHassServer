using DotHass.Server.Abstractions.Events;
using DotHass.Server.Abstractions.Message;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace DotHass.Server.Tcp
{
    /// <summary>
    /// 1.处理将bytes转换为Imessage
    /// 2.将Imessage转换为bytes传出去
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TcpClientHandler : MessageToMessageCodec<IByteBuffer, IResponseMessage>
    {
        private IMessageFactory messageFactory;

        public TcpClientHandler(IMessageFactory messageFactory)
        {
            this.messageFactory = messageFactory;
        }

        /// <summary>
        /// 触发一个握手事件
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelActive(IChannelHandlerContext context)
        {
            context.Channel.Pipeline.FireUserEventTriggered(new HandShakeEvent());
        }

        //channel.Pipeline.DisconnectAsync(超时)会触发inactive和unregistered。。。
        //客户端主动关闭(代码调用closeasync)的时候也会触发inactive和unregistered
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            context.Channel.Parent.Pipeline.FireUserEventTriggered(new ClientDisconnectEvent() { Channel = context.Channel });
        }

        protected override void Decode(IChannelHandlerContext ctx, IByteBuffer msg, List<object> output)
        {
            var message = messageFactory.Parse(msg);
            if (message == null)
            {
                return;
            }
            output.Add(message);
        }

        protected override void Encode(IChannelHandlerContext ctx, IResponseMessage msg, List<object> output)
        {
            output.Add(msg.ToAllBuffer());
        }

        public override bool IsSharable => true;
    }
}