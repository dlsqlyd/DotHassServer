using DotNetty.Transport.Channels;
using System;
using System.Threading.Tasks;

namespace DotHass.Server.Abstractions.Message
{
    /// <summary>
    /// string sourceUserID, int contractaID, byte[] info
    /// </summary>
    public interface IMessageHandler
    {
        void HandlerTimeout(IChannel channel);

        void HandlerActive(IChannel channel);

        void HandlerInactive(IChannel channel);

        void HandlerException(IChannel ctx, Exception e);

        Task Handler<I>(IChannel ctx, I message) where I : IRequestMessage;
    }
}