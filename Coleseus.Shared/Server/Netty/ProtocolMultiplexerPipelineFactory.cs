using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Server.Netty
{
    public class ProtocolMultiplexerPipelineFactory
    {
        // TODO make this configurable from spring.
        private const int MAX_IDLE_SECONDS = 60;
        private int bytesForProtocolCheck;
        private LoginProtocol loginProtocol;


        protected void initChannel(ISocketChannel ch)
        {
            // Create a default pipeline implementation.
            IChannelPipeline pipeline = ch.Pipeline;
            pipeline.AddLast("idleStateCheck", new IdleStateHandler(
                    MAX_IDLE_SECONDS, MAX_IDLE_SECONDS, MAX_IDLE_SECONDS));
            pipeline.AddLast("multiplexer", createProtcolMultiplexerDecoder());
        }

        protected IChannelHandler createProtcolMultiplexerDecoder()
        {
            return new ProtocolMultiplexerDecoder(bytesForProtocolCheck, loginProtocol);
        }

        public int getBytesForProtocolCheck()
        {
            return bytesForProtocolCheck;
        }

        public void setBytesForProtocolCheck(int bytesForProtocolCheck)
        {
            this.bytesForProtocolCheck = bytesForProtocolCheck;
        }

        public LoginProtocol getLoginProtocol()
        {
            return loginProtocol;
        }

        public void setLoginProtocol(LoginProtocol loginProtocol)
        {
            this.loginProtocol = loginProtocol;
        }

    }
}
