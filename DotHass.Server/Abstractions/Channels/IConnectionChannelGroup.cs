using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;

namespace DotHass.Server.Abstractions.Channels
{
    public interface IConnectionChannelGroup : IChannelGroup
    {
        ConnectionInfo FindConnectionInfo(IChannelId id);

        bool Add(ConnectionInfo connection);
    }
}