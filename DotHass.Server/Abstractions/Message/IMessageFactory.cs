using DotHass.Server.Abstractions.Channels;
using DotNetty.Buffers;

namespace DotHass.Server.Abstractions.Message
{
    public interface IMessageFactory
    {
        IRequestMessage Parse(string uri);

        IRequestMessage Parse(IByteBuffer data);

        IResponseMessage CreateHandShake(ConnectionInfo client);

        IResponseMessage Create(params object[] parameters);
    }
}