using System;
using System.Collections.Generic;
using System.Text;

/**
 * A handler which can handle a specific event. Implementations of this class
 * get attaches to the {@link EventDispatcher}.
 * 
 * @author Abraham Menacherry
 * 
 */
namespace Coleseus.Shared.Event
{
    public interface IEventHandler
    {
        /**
	     * On event method which will be used to handle an incoming event dispatched
	     * to it by a {@link EventDispatcher}
	     * 
	     * @param event
	     */
        void onEvent(IEvent @event);

        /**
         * @return Returns the event type which is an integer. Using this event type
         *         the event dispatcher can transmit matching events having the same
         *         event type to this handler instance.
         */
        int getEventType();
    }
}
