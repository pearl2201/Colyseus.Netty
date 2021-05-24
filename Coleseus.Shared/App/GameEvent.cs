using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.App
{
    public interface GameEvent<T, O, E>
    {
        T getPayload();
        void setPayload(T payload);
        PlayerSession getPlayerSession();
        void setPlayerSession(PlayerSession playerSession);
        O getOpCode();
        void setOpcode(O opcode);
        E getEventType();
        void setEventType(E eventType);
        String getEventName();
        void setEventName(string eventName);
        long getTimeStamp();
        void setTimeStamp(DateTime timeStamp);
    }
}
