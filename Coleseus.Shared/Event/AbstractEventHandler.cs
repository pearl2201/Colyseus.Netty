using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Event
{
    /**
   * Abstract event handler is a helper class which must be overriden by classes
   * which need to implement the {@link EventHandler} interface. The
   * {@link #onEvent(Event)} method needs to be implemented by such classes.
   * 
   * @author Abraham Menacherry
   * 
   */
    public abstract class AbstractEventHandler : IEventHandler
    {
        private readonly int EVENT_TYPE;

        public AbstractEventHandler(int eventType)
        {
            this.EVENT_TYPE = eventType;
        }


        public int getEventType()
        {
            return EVENT_TYPE;
        }

        public abstract void onEvent(IEvent @event);

    }
}
