﻿using System;
using System.Collections.Generic;
using System.Text;
//using ProtoBuf;

namespace Colyseus.Common.Messages
{
    //[ProtoContract]
    public class IoMessage
    {
        //[ProtoMember(1)]
        public string From;

        //[ProtoMember(2)]
        public string To;

        //[ProtoMember(3)]
        public string ReplyTo;

        //[ProtoMember(4)]
        public byte[] Body;
    }
}
