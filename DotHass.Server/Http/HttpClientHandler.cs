using DotHass.Server.Abstractions;
using DotHass.Server.Abstractions.Channels;
using DotHass.Server.Abstractions.Events;
using DotHass.Server.Abstractions.Message;
using DotNetty.Codecs;
using DotNetty.Codecs.Http;
using DotNetty.Common;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;

namespace DotHass.Server.Http
{
    internal class HttpClientHandler : MessageToMessageCodec<IHttpRequest, IResponseMessage>
    {
        private static readonly ThreadLocalCache Cache = new ThreadLocalCache();

        private sealed class ThreadLocalCache : FastThreadLocal<AsciiString>
        {
            protected override AsciiString GetInitialValue()
            {
                DateTime dateTime = DateTime.UtcNow;
                return AsciiString.Cached($"{dateTime.DayOfWeek}, {dateTime:dd MMM yyyy HH:mm:ss z}");
            }
        }

        private readonly ICharSequence date = Cache.Value;

        private IConnectionChannelGroup channelGroup;
        private IMessageFactory messageFactory;
        private ServerOptions Options;

        public HttpClientHandler(IMessageFactory messageFactory, IConnectionChannelGroup group, ServerOptions Options)
        {
            this.channelGroup = group;
            this.messageFactory = messageFactory;
            this.Options = Options;
        }

        //channel.Pipeline.DisconnectAsync(超时)会触发inactive和unregistered。。。
        //客户端主动关闭(代码调用closeasync)的时候也会触发inactive和unregistered
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            channelGroup.Remove(context.Channel);
        }

        protected override void Decode(IChannelHandlerContext ctx, IHttpRequest msg, List<object> output)
        {
            try
            {
                string uri = msg.Uri;
                switch (uri)
                {
                    case "/handshake":
                        var connectionInfo = channelGroup.FindConnectionInfo(ctx.Channel);
                        if (connectionInfo == null)
                        {
                            channelGroup.Add(new ConnectionInfo(ctx.Channel, this.Options));
                        }
                        ctx.Channel.Pipeline.FireUserEventTriggered(new HandShakeEvent());
                        break;

                    default:
                        var message = messageFactory.Parse(uri);
                        if (message != null)
                        {
                            output.Add(message);
                        }
                        break;
                }
            }
            finally
            {
                ReferenceCountUtil.Release(msg);
            }
        }

        protected override void Encode(IChannelHandlerContext ctx, IResponseMessage msg, List<object> output)
        {
            var buf = msg.ToBodyBuffer();
            var status = HttpResponseStatus.ValueOf(Convert.ToInt32(msg.Headers.Get(Constants.ResponseStatusCode)));
            var response = new DefaultFullHttpResponse(HttpVersion.Http11, status, buf, false);

            HttpHeaders headers = response.Headers;

            headers.Set(HttpHeaderNames.Server, AsciiString.Cached(Options.Name));
            headers.Set(HttpHeaderNames.Date, this.date);
            headers.Set(HttpHeaderNames.ContentLength, AsciiString.Cached($"{msg.Headers.Get(Constants.Headers.ContentLength)}"));

            if (Convert.ToInt32(msg.Headers.Get(Constants.Headers.ContractId)) == (int)ContractType.HandShake)
            {
                headers.Set(HttpHeaderNames.ContentType, AsciiString.Cached("text/plain"));
            }
            else
            {
                headers.Set(HttpHeaderNames.ContentType, AsciiString.Cached(msg.Headers.Get(Constants.Headers.ContentType)));
            }
            output.Add(response);
        }

        public override bool IsSharable => true;

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) => context.CloseAsync();

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
    }
}