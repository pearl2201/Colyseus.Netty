using Coleseus.Shared.Handlers.Netty;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Server.Netty
{
    public class UDPChannelInitializer : ChannelInitializer<SocketDatagramChannel>
    {
        /**
         * This pipeline will be shared across all the channels. In Netty UDP
         * implementation it does not make sense to have different pipelines for
         * different channels as the protocol is essentially "connection-less"
         */
        IChannelPipeline pipeline;
        private UDPEventEncoder udpEventEncoder;

        // Create a default pipeline implementation.
        private UDPUpstreamHandler upstream;


        public UDPChannelInitializer(UDPUpstreamHandler upstream)
        {
            this.upstream = upstream;
            udpEventEncoder = new UDPEventEncoder();
        }


        protected override void InitChannel(SocketDatagramChannel ch)
        {
            // pipeline is shared across all channels.
            pipeline = ch.Pipeline;
            pipeline.AddLast("upstream", upstream);

            // Downstream handlers - Filter for data which flows from server to
            // client. Note that the last handler added is actually the first
            // handler for outgoing data.
            // TODO since this is not handling datagram packet will it work out of box?
            pipeline.AddLast("udpEventEncoder", udpEventEncoder);

        }

        public void setUpstream(UDPUpstreamHandler upstream)
        {
            this.upstream = upstream;
        }

        public UDPUpstreamHandler getUpstream()
        {
            return upstream;
        }

        public UDPEventEncoder getUdpEventEncoder()
        {
            return udpEventEncoder;
        }

        public void setUdpEventEncoder(UDPEventEncoder udpEventEncoder)
        {
            this.udpEventEncoder = udpEventEncoder;
        }


    }
}
