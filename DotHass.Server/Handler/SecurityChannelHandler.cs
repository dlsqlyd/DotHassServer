using DotHass.Server.Abstractions.Channels;
using DotHass.Server.Abstractions.Message;
using DotHass.Server.Message;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Elskom.Generic.Libs;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace DotHass.Server.Handler
{
    public class SecurityChannelHandler : MessageToMessageCodec<DefaultRequestMessagePacket, DefaultResponseMessagePacket>
    {
        private IConnectionChannelGroup channelGroup;
        private readonly SecurityOptions options;
        private readonly BlowFish blowFish;

        public override bool IsSharable => true;

        public SecurityChannelHandler(IConnectionChannelGroup group, IOptions<SecurityOptions> options)
        {
            this.channelGroup = group;
            this.options = options.Value;
            this.blowFish = new BlowFish(this.options.Key);
        }

        protected override void Decode(IChannelHandlerContext ctx, DefaultRequestMessagePacket msg, List<object> output)
        {
            var connectionInfo = channelGroup.FindConnectionInfo(ctx.Channel);

            if (connectionInfo == null || msg.ClientId <= 0 || string.IsNullOrEmpty(msg.SessionId) || connectionInfo.ClientId != msg.ClientId || connectionInfo.SessionId != msg.SessionId)
            {
                ctx.CloseAsync();
                return;
            }

            switch (msg.ContractId)
            {
                case (int)ContractType.HandShake:
                    //握手,实际上在childchannel永远不会接受到握手消息
                    break;

                case (int)ContractType.HeartBeat:
                    //心跳直接忽略
                    break;

                default:
                    var o = blowFish.DecryptECB(msg.Sign);
                    if (o == GetInputKey(msg))
                    {
                        output.Add(msg);
                    }
                    break;
            }
        }

        protected override void Encode(IChannelHandlerContext ctx, DefaultResponseMessagePacket msg, List<object> output)
        {
            output.Add(msg);
        }

        public string GetInputKey(DefaultRequestMessagePacket msg)
        {
            return msg.ClientId + msg.SessionId + msg.ContractId + msg.BodyContent.Length.ToString();
        }
    }
}