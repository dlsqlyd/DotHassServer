using DotHass.Server.Abstractions.Message;
using DotNetty.Buffers;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;

namespace DotHass.Server.Websocket
{
    public class WebSocketFrameHandler : SimpleChannelInboundHandler<WebSocketFrame>
    {
        public static readonly AttributeKey<WebSocketServerHandshaker> wshsKey = AttributeKey<WebSocketServerHandshaker>.ValueOf(WebSocketConstansts.WebSocketServerHandshakerKey);
        private IMessageFactory messageFactory;

        public WebSocketFrameHandler(IMessageFactory messageFactory)
        {
            this.messageFactory = messageFactory;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, WebSocketFrame frame)
        {
            WebSocketServerHandshaker handshaker = ctx.GetAttribute(wshsKey).Get();
            // Check for closing frame
            if (frame is CloseWebSocketFrame)
            {
                handshaker.CloseAsync(ctx.Channel, (CloseWebSocketFrame)frame.Retain());
                return;
            }

            if (frame is PingWebSocketFrame)
            {
                ctx.WriteAsync(new PongWebSocketFrame((IByteBuffer)frame.Content.Retain()));
                return;
            }

            if (frame is TextWebSocketFrame text)
            {
                var uri = text.Text();
                var textmsg = messageFactory.Parse(uri);
                if (textmsg != null)
                {
                    ctx.FireChannelRead(textmsg);
                }
                return;
            }

            if (frame is BinaryWebSocketFrame)
            {
                var binarymsg = messageFactory.Parse(frame.Content);
                if (binarymsg != null)
                {
                    ctx.FireChannelRead(binarymsg);
                }
                return;
            }
        }

        public override bool IsSharable => true;
    }
}