using Coleseus.Shared.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Event.Impl
{

    /**
	 * A listener class which will be used by {@link GameRoom} to send
	 * {@link NetworkEvent}s to the connected sessions. When the game room
	 * publishes such events to its channel, this listener will pick it up and
	 * transmit it to the session which in turn will transmit it to the remote
	 * machine/vm.
	 * 
	 * @author Abraham Menacherry
	 * 
	 */
    public class NetworkEventListener : SessionEventHandler
    {

        private const int EVENT_TYPE = Events.NETWORK_MESSAGE;
        private readonly ISession session;

        public NetworkEventListener(ISession session)
        {
            this.session = session;
        }


        public void onEvent(IEvent @event)

        {
            session.onEvent(@event);
        }


        public int getEventType()
        {
            return EVENT_TYPE;
        }


        public ISession getSession()
        {
            return session;
        }


        public void setSession(ISession session)
        {
            throw new Exception(
                    "Session is a final field in this class. "
                            + "It cannot be reset");
        }

    }
}
