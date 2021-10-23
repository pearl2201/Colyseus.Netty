using Coleseus.Shared.Communication;
using Coleseus.Shared.Event;
using Coleseus.Shared.Event.Impl;
using Coleseus.Shared.Handlers.Netty;
using Coleseus.Shared.Util;
using DotNetty.Buffers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyseus.NettyServer.LostDecade
{
    public class LDEvent<T> : DefaultNetworkEvent
    {


        private ushort opCode;

        public LDEvent(ushort opCode) : base()
        {
            this.opCode = opCode;
        }

        public LDEvent(ushort opCode, IEvent @event, DeliveryGuaranty deliveryGuaranty) : base(@event, deliveryGuaranty)

        {
            this.opCode = opCode;
        }

        /**
         * Copy constructor which will take values from the event and set it on this
         * instance. It will disregard the type of the event and set it to
         * {@link Events#NETWORK_MESSAGE}. {@link DeliveryGuarantyOptions} is set to
         * RELIABLE.
         * 
         * @param event
         *            The instance from which payload, create time etc will be
         *            copied
         */
        public LDEvent(ushort opCode, IEvent @event) : this(opCode, @event, DeliveryGuaranty.RELIABLE)
        {
            this.opCode = opCode;
        }




        public override ushort NetworkPackageId => opCode;
    }

    public class LDUpdateHeroPositionEvent : IDataBufferSchema
    {

        public float X { get; set; }
        public float Y { get; set; }

        public MessageBuffer<IByteBuffer> ToMessageBuffer()
        {
            var info = JsonConvert.SerializeObject(this);
            MessageBuffer<IByteBuffer> messageBuffer = new NettyMessageBuffer();
            messageBuffer.writeString(info);

            return messageBuffer;
        }

        public static LDUpdateHeroPositionEvent FromMessageBuffer(MessageBuffer<IByteBuffer> messageBuffer)
        {
            var info = messageBuffer.readString();
            var state = JsonConvert.DeserializeObject<LDUpdateHeroPositionEvent>(info);
            return state;
        }
    }

}
