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

        
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<NettyUDPMessageSender>();
        private EndPoint remoteAddress;
        private IDatagramChannel channel;
        private ISessionRegistryService<EndPoint> sessionRegistryService;
        private IEventContext eventContext;
        private const DeliveryGuaranty DELIVERY_GUARANTY = DeliveryGuaranty.FAST;

        public NettyUDPMessageSender(EndPoint remoteAddress,
                IDatagramChannel channel,
                ISessionRegistryService<EndPoint> sessionRegistryService)
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
                _logger.Debug("Successfully removed session: {}", session);
            }
            else
            {
                _logger.Verbose("No udp session found for address: {}", remoteAddress);
            }

        }

        public EndPoint getRemoteAddress()
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

        protected ISessionRegistryService<EndPoint> getSessionRegistryService()
        {
            return sessionRegistryService;
        }

        protected class EventContextImpl : IEventContext
        {
            EndPoint clientAddress;
            public EventContextImpl(EndPoint clientAddress)
            {
                this.clientAddress = (EndPoint)clientAddress;
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
