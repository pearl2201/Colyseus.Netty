using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Colyseus.Common
{
    public class PersonEncoder : MessageToMessageEncoder<ColyseusMessage>
    {
        readonly Encoding _encoding;

        public PersonEncoder() : this(Encoding.GetEncoding(0))
        { }

        public PersonEncoder(Encoding encoding)
        {
            _encoding = encoding ?? throw new NullReferenceException("encoding");
        }

        protected override void Encode(IChannelHandlerContext ctx, ColyseusMessage person, List<object> output)
        {
            if (person is null)
                return;
            //ByteBuf
            //var message = $"{person.Name}|{person.Age}" + Environment.NewLine;

            //output.Add(ByteBufferUtil.EncodeString(ctx.Allocator, message, _encoding));
        }

        public override bool IsSharable => true;
    }
}
