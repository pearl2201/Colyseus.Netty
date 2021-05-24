using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Communication
{
    public class NettyTCPMessageSender : Reliable
    {
        private readonly IChannel _channel;
        private DeliveryGuaranty DELIVERY_GUARANTY = DeliveryGuarantyOptions.RELIABLE;
        private readonly ILogger<NettyTCPMessageSender> _logger;

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
            _logger.LogDebug("Going to close tcp connection");
            Event event = Events.event (null, Events.DISCONNECT);
		if (_channel.Active)
		{
			channel.write(event).addListener(ChannelFutureListener.CLOSE);
		}
		else
		{
			channel.close();
			_logger.LogDebug("Unable to write the Event {} with type {} to socket",
					event, event.getType());

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

