using Coleseus.Shared.Event;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coleseus.Shared.Communication
{
    public class NettyTCPMessageSender : Reliable
    {
        private readonly IChannel _channel;
        private DeliveryGuaranty DELIVERY_GUARANTY = DeliveryGuaranty.RELIABLE;
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<NettyTCPMessageSender>();

        public NettyTCPMessageSender(IChannel channel)
        {

            _channel = channel;
            
        }


        public Object sendMessage(Object message)
        {
            return _channel.WriteAndFlushAsync(message);
        }


        public DeliveryGuaranty getDeliveryGuaranty()
        {
            return DELIVERY_GUARANTY;
        }

        public IChannel getChannel()
        {
            return _channel;
        }

        /**
		 * Writes an the {@link Events#DISCONNECT} to the client, flushes
		 * all the pending writes and closes the channel.
		 * 
		 */

        public void close()
        {
            _logger.Debug("Going to close tcp connection");
            IEvent @event = Events.CreateEvent(null, Events.DISCONNECT);
            if (_channel.Active)
            {
                _channel.WriteAsync(@event).Wait();
            }
            else
            {
                _channel.CloseAsync().Wait();
                _logger.Debug("Unable to write the Event {} with type {} to socket",
                        @event, @event.GetType());

            }

        }


        public override string ToString()
        {
            String channelId = "TCP channel: ";
            if (null != _channel)
            {
                channelId += _channel.ToString();
            }
            else
            {
                channelId += "0";
            }
            String sender = "Netty " + channelId;
            return sender;
        }
    }

}

