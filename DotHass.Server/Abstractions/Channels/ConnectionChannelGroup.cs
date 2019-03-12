using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotHass.Server.Abstractions.Channels
{
    public class ConnectionChannelGroup : IConnectionChannelGroup
    {
        private static int nextId;
        private readonly ConcurrentDictionary<IChannelId, ConnectionInfo> channels = new ConcurrentDictionary<IChannelId, ConnectionInfo>();

        public ConnectionChannelGroup()
            : this($"group-{Interlocked.Increment(ref nextId):X2}")
        {
        }

        public ConnectionChannelGroup(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            this.Name = name;
        }

        public bool IsEmpty => this.channels.Count == 0;

        public string Name { get; }

        public IChannel Find(IChannelId id)
        {
            var connction = this.FindConnectionInfo(id);
            if (connction != null)
            {
                return connction.Channel;
            }
            return default;
        }

        public ConnectionInfo FindConnectionInfo(IChannelId id)
        {
            if (this.channels.TryGetValue(id, out ConnectionInfo connction))
            {
                return connction;
            }
            return default;
        }

        public Task WriteAsync(object message) => this.WriteAsync(message, ChannelMatchers.All());

        public Task WriteAsync(object message, IChannelMatcher matcher)
        {
            Contract.Requires(message != null);
            Contract.Requires(matcher != null);
            var futures = new Dictionary<IChannel, Task>();
            foreach (ConnectionInfo c in this.channels.Values)
            {
                if (matcher.Matches(c.Channel))
                {
                    futures.Add(c.Channel, c.Channel.WriteAsync(SafeDuplicate(message)));
                }
            }

            ReferenceCountUtil.Release(message);
            return new DefaultChannelGroupCompletionSource(this, futures /*, this.executor*/).Task;
        }

        public IChannelGroup Flush(IChannelMatcher matcher)
        {
            foreach (ConnectionInfo c in this.channels.Values)
            {
                if (matcher.Matches(c.Channel))
                {
                    c.Channel.Flush();
                }
            }
            return this;
        }

        public IChannelGroup Flush() => this.Flush(ChannelMatchers.All());

        public int CompareTo(IChannelGroup other)
        {
            int v = string.Compare(this.Name, other.Name, StringComparison.Ordinal);
            if (v != 0)
            {
                return v;
            }

            return this.GetHashCode() - other.GetHashCode();
        }

        void ICollection<IChannel>.Add(IChannel item) => this.Add(item);

        public void Clear()
        {
            this.channels.Clear();
        }

        public bool Contains(IChannel item)
        {
            this.channels.TryGetValue(item.Id, out ConnectionInfo connection);
            return connection != null && connection.Channel == item;
        }

        public void CopyTo(IChannel[] array, int arrayIndex) => this.ToArray().CopyTo(array, arrayIndex);

        public int Count => this.channels.Count;

        public bool IsReadOnly => false;

        public bool Remove(IChannel channel)
        {
            if (this.channels.TryRemove(channel.Id, out ConnectionInfo ch))
            {
                return true;
            }

            return false;
        }

        public IEnumerator<IChannel> GetEnumerator() => this.channels.Values.Select(connection => connection.Channel).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.channels.Values.GetEnumerator();

        public Task WriteAndFlushAsync(object message) => this.WriteAndFlushAsync(message, ChannelMatchers.All());

        public Task WriteAndFlushAsync(object message, IChannelMatcher matcher)
        {
            Contract.Requires(message != null);
            Contract.Requires(matcher != null);
            var futures = new Dictionary<IChannel, Task>();
            foreach (ConnectionInfo c in this.channels.Values)
            {
                if (matcher.Matches(c.Channel))
                {
                    futures.Add(c.Channel, c.Channel.WriteAndFlushAsync(SafeDuplicate(message)));
                }
            }

            ReferenceCountUtil.Release(message);
            return new DefaultChannelGroupCompletionSource(this, futures /*, this.executor*/).Task;
        }

        public Task DisconnectAsync() => this.DisconnectAsync(ChannelMatchers.All());

        public Task DisconnectAsync(IChannelMatcher matcher)
        {
            Contract.Requires(matcher != null);
            var futures = new Dictionary<IChannel, Task>();
            foreach (ConnectionInfo c in this.channels.Values)
            {
                if (matcher.Matches(c.Channel))
                {
                    futures.Add(c.Channel, c.Channel.DisconnectAsync());
                }
            }

            return new DefaultChannelGroupCompletionSource(this, futures /*, this.executor*/).Task;
        }

        public Task CloseAsync() => this.CloseAsync(ChannelMatchers.All());

        public Task CloseAsync(IChannelMatcher matcher)
        {
            Contract.Requires(matcher != null);
            var futures = new Dictionary<IChannel, Task>();
            foreach (ConnectionInfo c in this.channels.Values)
            {
                if (matcher.Matches(c.Channel))
                {
                    futures.Add(c.Channel, c.Channel.CloseAsync());
                }
            }
            return new DefaultChannelGroupCompletionSource(this, futures /*, this.executor*/).Task;
        }

        public Task DeregisterAsync() => this.DeregisterAsync(ChannelMatchers.All());

        public Task DeregisterAsync(IChannelMatcher matcher)
        {
            Contract.Requires(matcher != null);
            var futures = new Dictionary<IChannel, Task>();
            foreach (ConnectionInfo c in this.channels.Values)
            {
                if (matcher.Matches(c.Channel))
                {
                    futures.Add(c.Channel, c.Channel.DeregisterAsync());
                }
            }

            return new DefaultChannelGroupCompletionSource(this, futures /*, this.executor*/).Task;
        }

        public Task NewCloseFuture() => this.NewCloseFuture(ChannelMatchers.All());

        public Task NewCloseFuture(IChannelMatcher matcher)
        {
            Contract.Requires(matcher != null);
            var futures = new Dictionary<IChannel, Task>();
            foreach (ConnectionInfo c in this.channels.Values)
            {
                if (matcher.Matches(c.Channel))
                {
                    futures.Add(c.Channel, c.Channel.CloseCompletion);
                }
            }

            return new DefaultChannelGroupCompletionSource(this, futures /*, this.executor*/).Task;
        }

        private static object SafeDuplicate(object message)
        {
            var buffer = message as IByteBuffer;
            if (buffer != null)
            {
                return buffer.Duplicate().Retain();
            }

            var byteBufferHolder = message as IByteBufferHolder;
            if (byteBufferHolder != null)
            {
                return byteBufferHolder.Duplicate().Retain();
            }

            return ReferenceCountUtil.Retain(message);
        }

        public override string ToString() => $"{this.GetType().Name}(name: {this.Name}, size: {this.Count})";

        public bool Add(IChannel channel)
        {
            bool added = this.channels.TryAdd(channel.Id, new ConnectionInfo(channel));
            if (added)
            {
                channel.CloseCompletion.ContinueWith(x => this.Remove(channel));
            }
            return added;
        }

        public bool Add(ConnectionInfo connection)
        {
            var channel = connection.Channel;
            bool added = this.channels.TryAdd(channel.Id, connection);
            if (added)
            {
                channel.CloseCompletion.ContinueWith(x => this.Remove(channel));
            }
            return added;
        }

        public IChannel[] ToArray()
        {
            return this.channels.Values.Select(c => c.Channel).ToArray();
        }

        public bool Remove(IChannelId channelId)
        {
            ConnectionInfo ch;
            if (this.channels.TryRemove(channelId, out ch))
            {
                return true;
            }

            return false;
        }

        public bool Remove(object o)
        {
            var id = o as IChannelId;
            if (id != null)
            {
                return this.Remove(id);
            }
            else
            {
                var channel = o as IChannel;
                if (channel != null)
                {
                    return this.Remove(channel);
                }
            }
            return false;
        }
    }
}