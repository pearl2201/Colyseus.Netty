using Coleseus.Shared.App;
using Coleseus.Shared.Handlers.Netty;
using Coleseus.Shared.Util;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Protocols.Impl
{
    public class StringProtocol : AbstractNettyProtocol
    {
        /**
		 * The maximum size of the incoming message in bytes. The
		 * {@link DelimiterBasedFrameDecoder} will use this value in order to throw
		 * a {@link TooLongFrameException}.
		 */
        int frameSize;
        /**
		 * Flash client expects a nul byte 0x00 to be added as the end byte of any
		 * communication with it. This encoder will add this nul byte to the end of
		 * the message. Could be considered as a message "footer".
		 */
        private NulEncoder nulEncoder;
        /**
		 * Used to decode a netty {@link ByteBuf} (actually a byte array) to a
		 * string.
		 */
        private StringDecoder stringDecoder;
        /**
		 * Used to encode a normal java String to a netty {@link ByteBuf}
		 * (actually a byte array).
		 */
        private StringEncoder stringEncoder;

        public StringProtocol() : base("STRING_PROTOCOL")
        {

        }

        public StringProtocol(int frameSize, NulEncoder nulEncoder,
                StringDecoder stringDecoder, StringEncoder stringEncoder) : base("STRING_PROTOCOL")
        {

            this.frameSize = frameSize;
            this.nulEncoder = nulEncoder;
            this.stringDecoder = stringDecoder;
            this.stringEncoder = stringEncoder;
        }


        public override void applyProtocol(IPlayerSession playerSession)
        {
            IChannelPipeline pipeline = NettyUtils
                    .getPipeLineOfConnection(playerSession);
            // Upstream handlers or encoders (i.e towards server) are added to
            // pipeline now.
            pipeline.AddLast("framer", new DelimiterBasedFrameDecoder(frameSize,
                    Delimiters.NullDelimiter()));
            pipeline.AddLast("stringDecoder", stringDecoder);

            // Downstream handlers (i.e towards client) are added to pipeline now.
            pipeline.AddLast("nulEncoder", nulEncoder);
            pipeline.AddLast("stringEncoder", stringEncoder);

        }

        public int getFrameSize()
        {
            return frameSize;
        }


        public void setFrameSize(int frameSize)
        {
            this.frameSize = frameSize;
        }

        public NulEncoder getNulEncoder()
        {
            return nulEncoder;
        }


        public void setNulEncoder(NulEncoder nulEncoder)
        {
            this.nulEncoder = nulEncoder;
        }

        public StringDecoder getStringDecoder()
        {
            return stringDecoder;
        }


        public void setStringDecoder(StringDecoder stringDecoder)
        {
            this.stringDecoder = stringDecoder;
        }

        public StringEncoder getStringEncoder()
        {
            return stringEncoder;
        }


        public void setStringEncoder(StringEncoder stringEncoder)
        {
            this.stringEncoder = stringEncoder;
        }

    }
}
