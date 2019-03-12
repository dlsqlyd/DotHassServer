using DotHass.Server.Abstractions.Channels;
using DotNetty.Buffers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Text;

namespace DotHass.Server.Abstractions.Message
{
    public class MessageFactory<TRequestMessage, TResponseMessage> : IMessageFactory where TRequestMessage : IRequestMessage where TResponseMessage : IResponseMessage
    {
        private readonly IServiceProvider serviceProvider;

        public MessageFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IResponseMessage CreateHandShake(ConnectionInfo connection)
        {
            return Create((int)HttpStatusCode.OK, (int)ContractType.HandShake, 0, Encoding.UTF8.GetBytes(connection.ToString()));
        }

        public IResponseMessage Create(params object[] parameters)
        {
            return ActivatorUtilities.CreateInstance<TResponseMessage>(this.serviceProvider, parameters);
        }

        public IRequestMessage Parse(IByteBuffer data)
        {
            IRequestMessage message = ActivatorUtilities.CreateInstance<TRequestMessage>(this.serviceProvider);
            if (message.Parse(data) == false)
            {
                return default(TRequestMessage);
            }
            return message;
        }

        public IRequestMessage Parse(string uri)
        {
            IRequestMessage message = ActivatorUtilities.CreateInstance<TRequestMessage>(this.serviceProvider);
            if (message.Parse(uri) == false)
            {
                return default(TRequestMessage);
            }
            return message;
        }
    }
}