using DotHass.Server.Abstractions.Events;
using DotHass.Server.Abstractions.Message;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System.Collections.Generic;

namespace DotHass.Server.Udp
{
    public class UdpClientHandler : MessageToMessageCodec<DatagramPacket, IResponseMessage>
    {
        private IMessageFactory messageFactory;

        public UdpClientHandler(IMessageFactory messageFactory)
        {
            this.messageFactory = messageFactory;
        }

        /// <summary>
        /// 向客户端发送一个握手消息,分配clientid
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelActive(IChannelHandlerContext context)
        {
            context.Channel.Pipeline.FireUserEventTriggered(new HandShakeEvent());
        }

        //close会触发inactive和unregistered。。。
        //客户端主动关闭的时候也会触发inactive和unregistered
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            (context.Channel.Parent as UdpSocketDatagramChannel).Pipeline.FireUserEventTriggered(new ClientDisconnectEvent() { Channel = context.Channel });
        }

        protected override void Decode(IChannelHandlerContext ctx, DatagramPacket msg, List<object> output)
        {
            var message = messageFactory.Parse(msg.Content);
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