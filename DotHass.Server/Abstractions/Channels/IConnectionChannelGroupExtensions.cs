using DotNetty.Transport.Channels;

namespace DotHass.Server.Abstractions.Channels
{
    public static class IConnectionChannelGroupExtensions
    {
        public static int FindClientID(this IConnectionChannelGroup group, IChannel channel)
        {
            return group.FindConnectionInfo(channel.Id).ClientId;
        }

        public static ConnectionInfo FindConnectionInfo(this IConnectionChannelGroup group, IChannel channel)
        {
            return group.FindConnectionInfo(channel.Id);
        }
    }
}