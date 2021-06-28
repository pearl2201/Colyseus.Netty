using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Handlers.Netty
{
    public class NulEncoder : MessageToByteEncoder<IByteBuffer>
    {

        private IByteBuffer NULL_BUFFER = Unpooled.WrappedBuffer(new byte[] { 0 });


        protected override void Encode(IChannelHandlerContext ctx, IByteBuffer msg, IByteBuffer @out)
        {
            @out.WriteBytes(Unpooled.WrappedBuffer(msg, NULL_BUFFER));
        }

    }