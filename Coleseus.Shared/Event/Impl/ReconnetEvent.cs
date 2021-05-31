using Coleseus.Shared.Communication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Event.Impl
{

    public class ReconnetEvent : DefaultConnectEvent
    {


        public ReconnetEvent(Reliable tcpSender) : base(tcpSender, null)
        {

        }

        public override int getType()
        {
            return Events.RECONNECT;
        }
    }
}
