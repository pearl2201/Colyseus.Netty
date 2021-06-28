using Coleseus.Shared.Communication;
using Coleseus.Shared.Event;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Handlers.Netty
{
    public class MessageBufferEventEncoder : MessageToMessageEncoder<IEvent>
    {


        protected override void Encode(IChannelHandlerContext ctx, IEvent @event,
            List<Object> @out)
        {
            @out.Add(Encode(ctx, @event));
        }

        /**
		 * Encode is separated out so that child classes can still reuse this
		 * functionality.
		 * 
		 * @param ctx
		 * @param event
		 *            The event to be encoded into {@link ByteBuf}. It will be
		 *            converted to 'opcode'-'payload' format.
		 * @return If only opcode is specified a single byte {@link ByteBuf} is
		 *         returned, otherwise a byte buf with 'opcode'-'payload' format is
		 *         returned.
		 */
        protected IByteBuffer Encode(IChannelHandlerContext ctx, IEvent @event)

        {
            IByteBuffer msg = null;
            if (null != @event.getSource())
            {


                MessageBuffer<IByteBuffer> msgBuffer = (MessageBuffer<IByteBuffer>)@event.getSource();
                IByteBuffer data = msgBuffer.getNativeBuffer();
                IByteBuffer opcode = ctx.Allocator.Buffer(1);
                opcode.WriteByte(@event.getType());
                msg = Unpooled.WrappedBuffer(opcode, data);
            }
            else
            {
                msg = ctx.Allocator.Buffer(1);
                msg.WriteByte(@event.getType());
            }
            return msg;
        }

    }

}
