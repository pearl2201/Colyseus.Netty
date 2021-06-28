using Coleseus.Shared.App;
using Coleseus.Shared.Handlers.Netty;
using Coleseus.Shared.Util;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Protocols.Impl
{
    public class ChannelBufferProtocol : AbstractNettyProtocol
    {
        private readonly ILogger<ChannelBufferProtocol> _logger;
        /**
         * Utility handler provided by netty to add the length of the outgoing
         * message to the message as a header.
         */
        private LengthFieldPrepender lengthFieldPrepender;
        private EventDecoder eventDecoder;
        private EventEncoder eventEncoder;

        public ChannelBufferProtocol() : base("CHANNEL_BUFFER_PROTOCOL")
        {

        }


        public override void applyProtocol(IPlayerSession playerSession)
        {
            _logger.LogTrace("Going to apply {} on session: {}", getProtocolName(),
                    playerSession);

            IChannelPipeline pipeline = NettyUtils
                    .getPipeLineOfConnection(playerSession);
            // Upstream handlers or encoders (i.e towards server) are added to
            // pipeline now.
            pipeline.AddLast("lengthDecoder", createLengthBasedFrameDecoder());
            pipeline.AddLast("eventDecoder", eventDecoder);
            pipeline.AddLast("eventHandler", new DefaultToServerHandler(
                    playerSession,_logger));

            // Downstream handlers - Filter for data which flows from server to
            // client. Note that the last handler added is actually the first
            // handler for outgoing data.
            pipeline.AddLast("lengthFieldPrepender", lengthFieldPrepender);
            pipeline.AddLast("eventEncoder", eventEncoder);
        }

        public LengthFieldPrepender getLengthFieldPrepender()
        {
            return lengthFieldPrepender;
        }

        public void setLengthFieldPrepender(LengthFieldPrepender lengthFieldPrepender)
        {
            this.lengthFieldPrepender = lengthFieldPrepender;
        }

        public EventDecoder getEventDecoder()
        {
            return eventDecoder;
        }

        public void setEventDecoder(EventDecoder eventDecoder)
        {
            this.eventDecoder = eventDecoder;
        }

        public EventEncoder getEventEncoder()
        {
            return eventEncoder;
        }

        public void setEventEncoder(EventEncoder eventEncoder)
        {
            this.eventEncoder = eventEncoder;
        }

    }
}
