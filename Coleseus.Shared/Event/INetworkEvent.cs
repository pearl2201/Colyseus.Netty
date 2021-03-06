using Coleseus.Shared.Communication;
using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Event
{
    /**
   * This interface is specifically used for events that will get transmitted to
   * remote machine/vm. It contains the {@link DeliveryGuaranty} associated with
   * the event so that messages can be transmitted either using TCP or UDP
   * transports based on the guaranty defined. Implementations can use RELIABLE as
   * default.
   * 
   * @author Abraham Menacherry
   * 
   */
    public interface INetworkEvent : Event.IEvent
    {
        DeliveryGuaranty getDeliveryGuaranty();

        void setDeliveryGuaranty(DeliveryGuaranty deliveryGuaranty);
    }
}
