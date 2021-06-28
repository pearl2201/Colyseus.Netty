using Coleseus.Shared.Event;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Coleseus.Shared.Handlers.Netty
{
    public class UDPEventEncoder : MessageBufferEventEncoder
    {

        protected override void Encode(IChannelHandlerContext ctx, IEvent @event,
                List<Object> @out)
        {
            IByteBuffer data = (IByteBuffer)base.Encode(ctx, @event);
            EndPoint clientAddress = (EndPoint)@event
                    .getEventContext().getAttachment();
            @out.Add(new DatagramPacket(data, clientAddress));
        }

    }
}
