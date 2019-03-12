using DotHass.Server.Abstractions;
using DotHass.Server.Abstractions.Channels;
using DotHass.Server.Abstractions.Events;
using DotHass.Server.Abstractions.Message;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using static DotNetty.Codecs.Http.HttpResponseStatus;
using static DotNetty.Codecs.Http.HttpVersion;

namespace DotHass.Server.Websocket
{
    public sealed class WebSocketClientHandler : MessageToMessageCodec<IFullHttpRequest, IResponseMessage>
    {
        private const string WebsocketPath = "/ws";

        private readonly IConnectionChannelGroup channelGroup;

        private ServerOptions Options;

        public static readonly AttributeKey<WebSocketServerHandshaker> wshsKey = AttributeKey<WebSocketServerHandshaker>.ValueOf(WebSocketConstansts.WebSocketServerHandshakerKey);

        public WebSocketClientHandler(IConnectionChannelGroup group, ServerOptions Options)
        {
            this.channelGroup = group;
            this.Options = Options;
        }

        //channel.Pipeline.DisconnectAsync(超时)会触发inactive和unregistered。。。
        //客户端主动关闭(代码调用closeasync)的时候也会触发inactive和unregistered
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            channelGroup.Remove(context.Channel);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        protected override void Encode(IChannelHandlerContext ctx, IResponseMessage msg, List<object> output)
        {
            output.Add(new BinaryWebSocketFrame(msg.ToAllBuffer()));
        }

        protected override void Decode(IChannelHandlerContext ctx, IFullHttpRequest req, List<object> output)
        {
            // Handle a bad request.
            if (!req.Result.IsSuccess)
            {
                SendHttpResponse(ctx, req, new DefaultFullHttpResponse(Http11, BadRequest));
                return;
            }

            // Allow only GET methods.
            if (!Equals(req.Method, HttpMethod.Get))
            {
                SendHttpResponse(ctx, req, new DefaultFullHttpResponse(Http11, Forbidden));
                return;
            }

            // Handshake
            var wsFactory = new WebSocketServerHandshakerFactory(
                GetWebSocketLocation(req), null, true, 5 * 1024 * 1024);
            var handshaker = wsFactory.NewHandshaker(req);

            if (handshaker == null)
            {
                WebSocketServerHandshakerFactory.SendUnsupportedVersionResponse(ctx.Channel);
            }
            else
            {
                handshaker.HandshakeAsync(ctx.Channel, req);
            }

            ctx.GetAttribute(wshsKey).Set(handshaker);
            var connectionInfo = channelGroup.FindConnectionInfo(ctx.Channel);
            if (connectionInfo == null)
            {
                channelGroup.Add(new ConnectionInfo(ctx.Channel, this.Options));
            }
            ctx.Channel.Pipeline.FireUserEventTriggered(new HandShakeEvent());
        }

        private static void SendHttpResponse(IChannelHandlerContext ctx, IFullHttpRequest req, IFullHttpResponse res)
        {
            // Generate an error page if response getStatus code is not OK (200).
            if (res.Status.Code != 200)
            {
                IByteBuffer buf = Unpooled.CopiedBuffer(Encoding.UTF8.GetBytes(res.Status.ToString()));
                res.Content.WriteBytes(buf);
                buf.Release();
                HttpUtil.SetContentLength(res, res.Content.ReadableBytes);
            }

            // Send the response and close the connection if necessary.
            Task task = ctx.Channel.WriteAndFlushAsync(res);
            if (!HttpUtil.IsKeepAlive(req) || res.Status.Code != 200)
            {
                task.ContinueWith((t, c) => ((IChannelHandlerContext)c).CloseAsync(),
                    ctx, TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        private string GetWebSocketLocation(IFullHttpRequest req)
        {
            bool result = req.Headers.TryGet(HttpHeaderNames.Host, out ICharSequence value);
            Debug.Assert(result, "Host header does not exist.");
            string location = value.ToString() + WebsocketPath;

            if (Options.IsSsl)
            {
                return "wss://" + location;
            }
            else
            {
                return "ws://" + location;
            }
        }

        public override bool IsSharable => true;
    }
}