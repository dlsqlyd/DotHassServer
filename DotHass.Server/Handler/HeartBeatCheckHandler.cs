using DotHass.Server.Abstractions.Events;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;

namespace DotHass.Server.Handler
{
    public class HeartBeatCheckHandler : ChannelHandlerAdapter
    {
        // 失败计数器：未收到client端发送的ping请求
        private int unRecPingTimes = 0;

        // 定义服务端没有收到心跳消息的最大次数
        private const int MAX_UN_REC_PING_TIMES = 3;

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            var channel = context.Channel;
            if (evt is IdleStateEvent)
            {
                HeartBeatTimeoutEvent heartEvent = null;

                var eventState = evt as IdleStateEvent;
                if (eventState.State == IdleState.ReaderIdle)
                {
                    /*读超时*/
                    // 失败计数器次数大于等于3次的时候，关闭链接，等待client重连
                    if (unRecPingTimes >= MAX_UN_REC_PING_TIMES)
                    {
                        // 连续超过N次未收到client的ping消息，那么关闭该通道，等待client重连
                        heartEvent = new HeartBeatTimeoutEvent
                        {
                            Channel = channel,
                            State = IdleState.ReaderIdle
                        };
                    }
                    else
                    {
                        // 失败计数器加1
                        unRecPingTimes++;
                    }
                }
                else if (eventState.State == IdleState.WriterIdle)
                {
                    /*写超时*/
                    heartEvent = new HeartBeatTimeoutEvent
                    {
                        Channel = context.Channel,
                        State = IdleState.WriterIdle
                    };
                }
                else if (eventState.State == IdleState.AllIdle)
                {
                    /*总超时*/
                    heartEvent = new HeartBeatTimeoutEvent
                    {
                        Channel = context.Channel,
                        State = IdleState.AllIdle
                    };
                }

                if (heartEvent != null)
                {
                    channel.Pipeline.FireUserEventTriggered(heartEvent);
                    channel.Pipeline.DisconnectAsync();
                }
            }
            else
            {
                context.FireUserEventTriggered(evt);
            }
        }
    }
}