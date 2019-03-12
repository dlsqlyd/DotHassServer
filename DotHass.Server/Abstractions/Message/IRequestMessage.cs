using DotNetty.Buffers;
using System.Collections.Specialized;
using System.IO;

namespace DotHass.Server.Abstractions.Message
{
    public interface IRequestMessage
    {
        Stream InputStream { get; }

        NameValueCollection Headers { get; }

        bool Parse(IByteBuffer byteBuffer);

        bool Parse(string uri);
    }
}