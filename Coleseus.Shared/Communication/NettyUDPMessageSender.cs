using Coleseus.Shared.App;
using Coleseus.Shared.Event;
using Coleseus.Shared.Service;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Coleseus.Shared.Communication
{
    /**
  * This class is used to send messages to a remote UDP client or server. An
  * instance of this class will be created by the {@link UDPUpstreamHandler} when
  * a {@link Events#CONNECT} event is received from client. The created instance
  * of this class is then sent as payload of a {@link DefaultNetworkEvent} to the
  * {@link Session}.
  * 
  * 
  * @author Abraham Menacherry
  * 
  */
    public class NettyUDPMessageSender : Fast
    {

        private readonly ILogger<NettyUDPMessageSender> _logger;
        private SocketAddress remoteAddress;
        private IDatagramChannel channel;
        private ISessionRegistryService<SocketAddress> sessionRegistryService;
        private IEventContext eventContext;
        private const DeliveryGuaranty DELIVERY_GUARANTY = DeliveryGuaranty.FAST;

        public NettyUDPMessageSender(SocketAddress remoteAddress,
                IDatagramChannel channel,
                ISessionRegistryService<SocketAddress> sessionRegistryService)
        {
            this.remoteAddress = remoteAddress;
            this.channel = channel;
            this.sessionRegistryService = sessionRegistryService;
            this.eventContext = new EventContextImpl(remoteAddress);
        }


        public Object sendMessage(Object message)
        {
            // TODO this might overwrite valid context, check for better design
            if (message is IEvent)
            {
                ((IEvent)message).setEventContext(eventContext);
            }
            channel.WriteAndFlushAsync(message).GetAwaiter().GetResult();
            return null;
        }


        public DeliveryGuaranty getDeliveryGuaranty()
        {
            return DELIVERY_GUARANTY;
        }


        public void close()
        {
            ISession session = sessionRegistryService.getSession(remoteAddress);
            if (sessionRegistryService.removeSession(remoteAddress))
            {
                _logger.LogDebug("Successfully removed session: {}", session);
            }
            else
            {
                _logger.LogTrace("No udp session found for address: {}", remoteAddress);
            }

        }

        public SocketAddress getRemoteAddress()
        {
            return remoteAddress;
        }

        public IDatagramChannel getChannel()
        {
            return channel;
        }


        public override string ToString()
        {
            String channelId = "UDP Channel: ";
            if (null != channel)
            {
                channelId += channel.ToString();
            }
            else
            {
                channelId += "0";
            }
            String sender = "Netty " + channelId + " RemoteAddress: "
                    + remoteAddress;
            return sender;
        }

        protected ISessionRegistryService<SocketAddress> getSessionRegistryService()
        {
            return sessionRegistryService;
        }

        protected class EventContextImpl : IEventContext
        {
            SocketAddress clientAddress;
            public EventContextImpl(SocketAddress clientAddress)
            {
                this.clientAddress = (SocketAddress)clientAddress;
            }

            public ISession getSession()
            {
                return null;
            }


            public void setSession(ISession session)
            {
            }


            public object getAttachment()
            {
                return clientAddress;
            }


            public void setAttachment(Object attachement)
            {
            }


        }
    }

}
