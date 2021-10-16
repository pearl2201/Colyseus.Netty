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

        public ISession Session { get; set; }

        public SessionMessageHandler(ISession session)
        {
            Session = session;
        }


        public int getEventType()
        {
            return Events.SESSION_MESSAGE;
        }




        public abstract void onEvent(IEvent @event);

    }

}
