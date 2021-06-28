using Coleseus.Shared.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Event.Impl
{
    public class JetlangEventDispatcher: IEventDispatcher
    {
        public void addHandler(IEventHandler eventHandler)
        {
            throw new NotImplementedException();
        }

        public List<IEventHandler> getHandlers(int eventType)
        {
            throw new NotImplementedException();
        }

        public void removeHandler(IEventHandler eventHandler)
        {
            throw new NotImplementedException();
        }

        public void removeHandlersForEvent(int eventType)
        {
            throw new NotImplementedException();
        }

        public bool removeHandlersForSession(ISession session)
        {
            throw new NotImplementedException();
        }

        public void clear()
        {
            throw new NotImplementedException();
        }

        public void fireEvent(IEvent @event)
        {
            throw new NotImplementedException();
        }

        public void close()
        {
            throw new NotImplementedException();
        }
    }
}
