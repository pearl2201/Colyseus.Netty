using Coleseus.Shared.Communication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Event
{
    public interface ConnectEvent : IEvent
    {
        Reliable getTcpSender();
        void setTcpSender(Reliable tcpSender);
        Fast getUdpSender();
        void setUdpSender(Fast udpSender);
    }
}
