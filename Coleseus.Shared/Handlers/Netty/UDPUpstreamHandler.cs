using Coleseus.Shared.App;
using Coleseus.Shared.Communication;
using Coleseus.Shared.Event;
using Coleseus.Shared.Service;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Coleseus.Shared.Handlers.Netty
{
    public class UDPUpstreamHandler : SimpleChannelInboundHandler<DatagramPacket>
    {
        
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<UDPUpstreamHandler>();
        private const string UDP_CONNECTING = "UDP_CONNECTING";
        private ISessionRegistryService<EndPoint> udpSessionRegistry;
        private MessageBufferEventDecoder messageBufferEventDecoder;

        public UDPUpstreamHandler(ISessionRegistryService<EndPoint> udpSessionRegistry, MessageBufferEventDecoder messageBufferEventDecoder) : base()
        {
            this.udpSessionRegistry = udpSessionRegistry;
            this.messageBufferEventDecoder = messageBufferEventDecoder;
        }


        protected override void ChannelRead0(IChannelHandlerContext ctx,
                DatagramPacket packet)
        {
            // Get the session using the remoteAddress.
            EndPoint remoteAddress = packet.Sender;
            ISession session = udpSessionRegistry.getSession(remoteAddress);
            if (null != session)

            {
                IByteBuffer buffer = packet.Content;
                IEvent @event = (IEvent)messageBufferEventDecoder
                        .decode(null, buffer);

                // If the session's UDP has not been connected yet then send a
                // CONNECT event.
                if (!session.IsUDPEnabled)
                {
                    if (null == session.GetAttribute(UDP_CONNECTING)
                            || (!(Boolean)session.GetAttribute(UDP_CONNECTING)))
                    {
                        session.SetAttribute(UDP_CONNECTING, true);
                        @event = getUDPConnectEvent(@event, remoteAddress,

                                (SocketDatagramChannel)ctx.Channel);
                        // Pass the connect event on to the session
                        session.OnEvent(@event);
                    }
                    else
                    {
                        _logger.Information("Going to discard UDP Message Event with type {} "
                                + "the UDP MessageSender is not initialized fully",
                                @event.getType());
                    }
                }

                else if (@event.getType() == Events.CONNECT)

                {
                    // Duplicate connect just discard.
                    _logger.Verbose("Duplicate CONNECT {} received in UDP channel, "
                            + "for session: {} going to discard", @event, session);
                }

                else

                {
                    // Pass the original event on to the session
                    session.OnEvent(@event);
                }
            }

            else
            {
                _logger.Verbose(
                        "Packet received from unknown source address: {}, going to discard",
                        remoteAddress);
            }
        }

        public IEvent getUDPConnectEvent(IEvent @event, EndPoint remoteAddress,
                SocketDatagramChannel udpChannel)
        {
            _logger.Debug("Incoming udp connection remote address : {}",
                    remoteAddress);

            if (@event.getType() != Events.CONNECT)
            {
                _logger.Information("UDP Event with type {} will get converted to a CONNECT "
                        + "event since the UDP MessageSender is not initialized till now",
                                @event.getType());
            }
            Fast messageSender = new NettyUDPMessageSender(remoteAddress, udpChannel, udpSessionRegistry);
            IEvent connectEvent = Events.connectEvent(messageSender);

            return connectEvent;
        }

        public ISessionRegistryService<EndPoint> getUdpSessionRegistry()
        {
            return udpSessionRegistry;
        }

        public void setUdpSessionRegistry(
                ISessionRegistryService<EndPoint> udpSessionRegistry)
        {
            this.udpSessionRegistry = udpSessionRegistry;
        }

        public MessageBufferEventDecoder getMessageBufferEventDecoder()
        {
            return messageBufferEventDecoder;
        }

        public void setMessageBufferEventDecoder(
                MessageBufferEventDecoder messageBufferEventDecoder)
        {
            this.messageBufferEventDecoder = messageBufferEventDecoder;
        }


    }
}
