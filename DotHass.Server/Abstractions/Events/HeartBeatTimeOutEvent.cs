using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;

namespace DotHass.Server.Abstractions.Events
{
    public class HeartBeatTimeoutEvent
    {
        public IdleState State
        {
            get;
            set;
        }

        public IChannel Channel
        {
            get;
            set;
        }
    }
}