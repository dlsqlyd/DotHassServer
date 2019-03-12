using DotHass.Server.Abstractions.Channels;
using DotHass.Server.Abstractions.Message;
using DotHass.Server.RequestProcessing;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DotHass.Server.Message
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    /// <summary>
    /// 1.创建和分发到控制器
    /// 2.处理超时等消息
    /// 3.验证签名消息
    /// </summary>
    public class DefaultMessageHandler : IMessageHandler
    {
        private IConnectionChannelGroup channelGroup;
        private readonly ILogger<DefaultMessageHandler> logger;
        private readonly IServiceProvider serviceprovider;
        public static readonly AttributeKey<AppFunc> appKey = AttributeKey<AppFunc>.ValueOf(Constants.AppAttributeKey);

        public DefaultMessageHandler(IConnectionChannelGroup group, IServiceProvider provider, ILogger<DefaultMessageHandler> logger)
        {
            this.channelGroup = group;
            this.logger = logger;
            this.serviceprovider = provider;
        }

        /// <summary>
        /// 不用特别处理。。超时之后同时会触发HandlerInactive
        /// </summary>
        /// <param name="channel"></param>
        public void HandlerTimeout(IChannel channel)
        {
            this.logger.LogDebug(channel + "超时");
        }

        public void HandlerActive(IChannel channel)
        {
            this.logger.LogDebug(channel + "连接成功");
        }

        public void HandlerInactive(IChannel channel)
        {
            this.logger.LogDebug(channel + "关闭连接");
        }

        public void HandlerException(IChannel channel, Exception e)
        {
            this.logger.LogDebug(channel + "发生异常" + e);
        }

        public async Task Handler<I>(IChannel channel, I msg) where I : IRequestMessage
        {
            OwinListenerContext owinContext = null;

            var defaultMsg = msg as DefaultRequestMessagePacket;
            AppFunc _appFunc = channel.GetAttribute<AppFunc>(appKey).Get();
            var connectioninfo = channelGroup.FindConnectionInfo(channel);

            try
            {
                owinContext = new OwinListenerContext(connectioninfo, msg);
                PopulateServerKeys(owinContext.Environment);

                await _appFunc(owinContext.Environment);

                owinContext.End();
                owinContext.Dispose();
            }
            catch (Exception ex)
            {
                owinContext.Environment[Constants.ResponseStatusCode] = HttpStatusCode.InternalServerError;

                if (owinContext != null)
                {
                    owinContext.End(ex);
                    owinContext.Dispose();
                }

                this.logger.LogError(ex, "消息处理错误");
            }

            var statusCode = (int)owinContext.Environment[Constants.ResponseStatusCode];
            byte[] responseBody = null;
            if (statusCode == (int)HttpStatusCode.OK)
            {
                responseBody = (owinContext.Environment[Constants.ResponseBody] as ListenerStreamWrapper).ToArray();
            }

            var response = new DefaultResponseMessagePacket(
                statusCode,
                defaultMsg.ContractId,
                defaultMsg.Id,
                responseBody
                );
            var headers = owinContext.Environment[Constants.ResponseHeaders] as ResponseHeadersDictionary;

            foreach (var item in headers)
            {
                response.Headers.Add(item.Key, item.Value.FirstOrDefault());
            }

            await channel.WriteAndFlushAsync(response);
        }

        private void PopulateServerKeys(CallEnvironment environment)
        {
            environment.Add(Constants.RequestServiceKey, this.serviceprovider);
        }
    }
}