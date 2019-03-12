using DotNetty.Buffers;
using System.Collections.Specialized;

namespace DotHass.Server.Abstractions.Message
{
    public interface IResponseMessage
    {
        NameValueCollection Headers { get; }

        IByteBuffer ToAllBuffer();

        IByteBuffer ToBodyBuffer();

        void AddHeader(string key, string value);
    }
}