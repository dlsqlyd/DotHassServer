using DotNetty.Buffers;
using System.Text;

namespace DotHass.Server.Abstractions
{
    public static class ByteBufferExtension
    {
        public static string ReadShortString(this IByteBuffer buffer, Encoding encoding)
        {
            int length = buffer.ReadByte();
            return length <= 0 ? string.Empty : buffer.ReadString(length, encoding);
        }

        public static int WriteShortString(this IByteBuffer buffer, string str, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(str);
            buffer.WriteByte(bytes.Length);
            buffer.WriteBytes(bytes);
            return bytes.Length;
        }

        public static string ReadLongString(this IByteBuffer buffer, Encoding encoding)
        {
            int length = buffer.ReadInt();
            return length <= 0 ? string.Empty : buffer.ReadString(length, encoding);
        }

        public static int WriteLongString(this IByteBuffer buffer, string str, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(str);
            buffer.WriteInt(bytes.Length);
            buffer.WriteBytes(bytes);
            return bytes.Length;
        }
    }
}