using Coleseus.Shared.Communication;
using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Event
{
    public interface IEvent
    {
        int getType();

        void setType(int type);

        Object getSource();

        void setSource(Object source);

        IEventContext getEventContext();

        void setEventContext(IEventContext context);

        DateTime getTimeStamp();

        void setTimeStamp(DateTime timeStamp);

        MessageBuffer<IByteBuffer> getSourceBuffer();


        IByteBuffer getBufferData();
    }
}
