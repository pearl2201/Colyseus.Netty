using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Colyseus.Common {
    public class PersonDecoder : MessageToMessageDecoder<IByteBuffer> {
        readonly Encoding _encoding;

        public PersonDecoder () : this (Encoding.GetEncoding (0)) { }

        public PersonDecoder (Encoding encoding) {
            _encoding = encoding ??
                throw new NullReferenceException ("encoding");
        }

        protected override void Decode (IChannelHandlerContext ctx, IByteBuffer message, List<object> output) {

            if (message.IoBufferCount < 12) {
                return;
            }

            int cmd = message.ReadInt ();
            int version = message.ReadInt ();
            int len = message.ReadInt ();
            if (message.IoBufferCount < 12 + len) {
                message.DiscardReadBytes ();
            }
            var rawData = new byte[len];
            message.ReadBytes (rawData, 13, len);

            //Match match = Regex.Match(text, @"^(?<Name>\w+)\|(?<Age>\d{1,3})[\r\n]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            bool verify = true;
            byte[] decodedData = rawData;
            if (verify) {

<<<<<<< HEAD
                output.Add (new ColyseusMessage () { Cmd = cmd, Version = version, Len = len, RawData = decodedData });
=======
                output.Add (new ColyseusMessage () { Cmd = cmd, Version = version, Len = len, RawData = rawData });
>>>>>>> 40b1c90824edfea1c764b751e0a46fdb0a7d1df1
            }
        }

        public override bool IsSharable => true;
    }
}