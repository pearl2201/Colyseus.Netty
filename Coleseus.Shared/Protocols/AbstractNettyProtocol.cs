using Coleseus.Shared.App;
using Coleseus.Shared.Util;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Protocols
{
    /**
   * This abstract class defines common methods across all protocols. Individual
   * protocol classes extend this class.
   * 
   * @author Abraham Menacherry
   * 
   */
    public abstract class AbstractNettyProtocol : IProtocol
    {
        /**
		 * The name of the protocol. This is set by the child class to appropriate
		 * value while child class instance is created.
		 */
        String protocolName;

        /**
         * Name of the idle state check handlers which will be removed by protocol
         * manually if required from pipeline.
         */
        public const String IDLE_STATE_CHECK_HANDLER = "idleStateCheck";

        public AbstractNettyProtocol(String protocolName)
        {

            this.protocolName = protocolName;
        }

        public LengthFieldBasedFrameDecoder createLengthBasedFrameDecoder()
        {
            return new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 2, 0, 2);
        }


        public String getProtocolName()
        {
            return protocolName;
        }


        public void applyProtocol(IPlayerSession playerSession,
                bool clearExistingProtocolHandlers)
        {
            if (clearExistingProtocolHandlers)
            {
                IChannelPipeline pipeline = NettyUtils
                        .getPipeLineOfConnection(playerSession);
                NettyUtils.clearPipeline(pipeline);
            }
            applyProtocol(playerSession);
        }

        public abstract void applyProtocol(IPlayerSession playerSession);
    }

}
