using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System.Net;

namespace DotHass.Server.Udp
{
    public class UdpSocketDatagramChannel : SocketDatagramChannel
    {
        protected EndPoint RemoteEndPoint;
        public new IChannel Parent { get; }
        protected override EndPoint RemoteAddressInternal => this.RemoteEndPoint;

        public UdpSocketDatagramChannel(IChannel parent, EndPoint sender) : base()
        {
            this.Parent = parent;
            this.RemoteEndPoint = sender;
            this.SetState(StateFlags.Active);
            this.CacheRemoteAddress();
        }

        public override bool Active => this.Open;

        protected override void ScheduleSocketRead()
        {
            base.ScheduleSocketRead();
        }
    }
}