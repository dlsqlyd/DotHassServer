namespace DotHass.Server.Abstractions.Channels
{
    using DotNetty.Transport.Channels;
    using System.Security.Cryptography;
    using System.Threading;

    public sealed class ConnectionInfo
    {
        private static int _lastId = 0;
        private static readonly RandomNumberGenerator CryptoRandom = RandomNumberGenerator.Create();

        /// <summary>
        /// 为kcp使用,值因为是整形的可猜测, 有session避免这个问题
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// 随机不可猜测的
        /// </summary>
        public string SessionId { get; set; }

        public IChannel Channel { get; set; }
        public ServerOptions ServerOptions { get; }

        public ConnectionInfo(IChannel channel)
        {
            this.Channel = channel;
            this.ClientId = GetNextId();
            this.SessionId = GetNewSessionId();
        }

        public ConnectionInfo(IChannel channel, ServerOptions options) : this(channel)
        {
            this.ServerOptions = options;
        }

        private static int GetNextId() => Interlocked.Increment(ref _lastId);

        //SessionKeyLength = 36; "382c74c3-721d-4f34-80e5-57657b6cbc27"
        private static string GetNewSessionId()
        {
            var guidBytes = new byte[16];
            CryptoRandom.GetBytes(guidBytes);
            return new System.Guid(guidBytes).ToString();
        }

        public override string ToString()
        {
            return this.ClientId + "|" + this.SessionId;
        }
    }
}