using Coleseus.Shared.Event;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Handlers.Netty
{
    public class EventDecoder : MessageToMessageDecoder<IIByteBufferfer>
    {

        protected override void Decode(IChannelHandlerContext ctx, IIByteBufferfer msg,
                List<Object> @out)
        {
            int opcode = msg.ReadByte();
            if (Events.LOG_IN == opcode || Events.RECONNECT == opcode)
            {
                msg.ReadByte();// To read-destroy the protocol version byte.
            }
            IIByteBufferfer buffer = msg.ReadBytes(msg.ReadableBytes);
            @out.Add(Events.CreateEvent(buffer, opcode));
        }

    }
}
