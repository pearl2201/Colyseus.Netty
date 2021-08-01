using Coleseus.Shared.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Event.Impl
{
    /**
 * This abstract helper class can be used to quickly create a listener which
 * listens for SESSION_MESSAGE events. Child classes need to override the
 * onEvent to plugin the logic.
 * 
 * @author Abraham Menacherry
 * 
 */
    public abstract class SessionMessageHandler : SessionEventHandler
    {

        private readonly ISession session;

        public SessionMessageHandler(ISession session)
        {
            this.session = session;
        }


        public int getEventType()
        {
            return Events.SESSION_MESSAGE;
        }


        public ISession getSession()
        {
            return session;
        }


        public void setSession(ISession session)
        {
            throw new MissingMethodException(
                    "Session instance is final and cannot be reset on this handler");
        }

        public abstract void onEvent(IEvent @event);

    }

}
