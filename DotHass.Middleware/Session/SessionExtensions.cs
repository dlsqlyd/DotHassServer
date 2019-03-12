using DotHass.Middleware.Abstractions;
using System;
using System.Text;

namespace DotHass.Middleware.Session
{
    public static class SessionExtensions
    {
        public static void SetLong(this ISession session, string key, long value)
        {
            session.Set(key, BitConverter.GetBytes(value));
        }

        public static long? GetLong(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null)
            {
                return null;
            }
            return BitConverter.ToInt64(data);
        }

        public static void SetInt32(this ISession session, string key, int value)
        {
            var bytes = new byte[]
            {
                (byte)(value >> 24),
                (byte)(0xFF & (value >> 16)),
                (byte)(0xFF & (value >> 8)),
                (byte)(0xFF & value)
            };
            session.Set(key, bytes);
        }

        public static int? GetInt32(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null || data.Length < 4)
            {
                return null;
            }
            return data[0] << 24 | data[1] << 16 | data[2] << 8 | data[3];
        }

        public static void SetString(this ISession session, string key, string value)
        {
            session.Set(key, Encoding.UTF8.GetBytes(value));
        }

        public static string GetString(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null)
            {
                return null;
            }
            return Encoding.UTF8.GetString(data);
        }

        public static byte[] Get(this ISession session, string key)
        {
            byte[] value = null;
            session.TryGetValue(key, out value);
            return value;
        }
    }
}