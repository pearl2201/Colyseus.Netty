using Coleseus.Shared.Communication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Event.Impl
{
    public class DefaultConnectEvent : DefaultEvent, ConnectEvent
    {
        private const long serialVersionUID = 1L;

        protected Reliable tcpSender;
        protected Fast udpSender;

        public DefaultConnectEvent(Reliable tcpSender) : this(tcpSender, null)
        {

        }

        public DefaultConnectEvent(Fast udpSender) : this(null, udpSender)
        {

        }

        public DefaultConnectEvent(Reliable tcpSender, Fast udpSender)
        {
            this.tcpSender = tcpSender;
            this.udpSender = udpSender;
        }


        public override int getType()
        {
            return Events.CONNECT;
        }


        public override void setType(int type)
        {
            throw new NotSupportedException(
                    "Type field is final, it cannot be reset");
        }


        public override object getSource()
        {
            return tcpSender;
        }


        public override void setSource(Object source)
        {
            this.tcpSender = (Reliable)source;
        }

        public Reliable getTcpSender()
        {
            return tcpSender;
        }

        public void setTcpSender(Reliable tcpSender)
        {
            this.tcpSender = tcpSender;
        }

        public Fast getUdpSender()
        {
            return udpSender;
        }

        public void setUdpSender(Fast udpSender)
        {
            this.udpSender = udpSender;
        }
    }
}
