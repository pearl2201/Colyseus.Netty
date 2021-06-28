using Coleseus.Shared.Event;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Handlers.Netty
{
    public class EventEncoder : MessageToMessageEncoder<IEvent>
    {


        protected override void Encode(IChannelHandlerContext ctx, IEvent @event,
                List<Object> @out)
        {
            IByteBuffer opcode = ctx.Allocator.Buffer(1);
            opcode.WriteByte(@event.getType());
            if (null != @event.getSource())
            {
                IByteBuffer data = (IByteBuffer)@event.getSource();
                IByteBuffer compositeBuffer = Unpooled.WrappedBuffer(opcode, data);
                @out.Add(compositeBuffer);
            }
            else
            {
                @out.Add(opcode);
            }

        }

    }
