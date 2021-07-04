using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coleseus.Shared.Handlers.Netty
{
    /**
  * This class can be used to switch login-protocol based on the incoming bytes
  * sent by a client. So, based on the incoming bytes, it is possible to set SSL
  * enabled, normal HTTP, default nadron protocol, or custom user protocol for
  * allowing client to login to nadron. The appropriate protocol searcher needs
  * to be injected to this class. Since this class is a non-singleton, the
  * protocol searchers and other dependencies should actually be injected to
  * {@link ProtocolMultiplexerChannelInitializer} class and then passed in while
  * instantiating this class.
  * 
  * @author Abraham Menacherry
  * 
  */
    public class ProtocolMultiplexerDecoder : ByteToMessageDecoder
    {


        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<ProtocolMultiplexerDecoder>();

        private LoginProtocol loginProtocol;
        private int bytesForProtocolCheck;

        public ProtocolMultiplexerDecoder(int bytesForProtocolCheck,
                LoginProtocol loginProtocol)
        {
            this.loginProtocol = loginProtocol;
            this.bytesForProtocolCheck = bytesForProtocolCheck;
        }


        protected override void Decode(IChannelHandlerContext ctx, IByteBuffer @in,
                List<Object> @out)
        {
            // Will use the first bytes to detect a protocol.
            if (@in.ReadableBytes < bytesForProtocolCheck)

            {
                return;
            }

            IChannelPipeline pipeline = ctx.Channel.Pipeline;

            if (!loginProtocol.applyProtocol(@in, pipeline))

            {
                byte[] headerBytes = new byte[bytesForProtocolCheck];
                @in.GetBytes(@in.ReaderIndex, headerBytes, 0,
                        bytesForProtocolCheck);
                _logger.Error(
                            "Unknown protocol, discard everything and close the connection {}. Incoming Bytes {}",
                            ctx.Channel,
                            BinaryUtils.getHexString(headerBytes));
                close(@in, ctx).GetAwaiter().GetResult();
            }
            else

            {
                pipeline.Remove(this);
            }
        }

        protected async Task close(IByteBuffer buffer, IChannelHandlerContext ctx)
        {
            buffer.Clear();
            await ctx.CloseAsync();
        }

        public LoginProtocol getLoginProtocol()
        {
            return loginProtocol;
        }

        public int getBytesForProtocolCheck()
        {
            return bytesForProtocolCheck;
        }


    }
}
