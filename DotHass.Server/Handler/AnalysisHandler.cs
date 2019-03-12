using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace DotHass.Server.Handler
{
    public class AnalysisHandler : ChannelHandlerAdapter
    {
        private Stopwatch watch;

        public override void ChannelRead(IChannelHandlerContext ctx, object message)
        {
            watch = System.Diagnostics.Stopwatch.StartNew();
            ctx.FireChannelRead(message);
        }

        public override void Flush(IChannelHandlerContext ctx)
        {
            ctx.Flush();

            //tcp下第一次发送数据的时候。没有执行channelread
            if (watch != null)
            {
                watch.Stop();
                Console.WriteLine($"---------------Execution Time: {watch.ElapsedMilliseconds} ms");
            }
        }


    }
}
