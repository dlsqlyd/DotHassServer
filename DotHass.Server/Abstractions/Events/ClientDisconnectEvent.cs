using DotNetty.Transport.Channels;

namespace DotHass.Server.Abstractions.Events
{
    public class ClientDisconnectEvent
    {
        public IChannel Channel
        {
            get;
            set;
        }
    }
}