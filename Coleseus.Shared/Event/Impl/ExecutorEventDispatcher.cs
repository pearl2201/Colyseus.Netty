using Coleseus.Shared.App;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coleseus.Shared.Event.Impl
{
    public class ExecutorEventDispatcher : IEventDispatcher
    {
        private readonly ILogger _logger = Serilog.Log.ForContext<ExecutorEventDispatcher>();
        private Dictionary<int, List<IEventHandler>> handlersByEventType;
        private List<IEventHandler> genericHandlers;
        private bool isShuttingDown;
        private readonly Object locker = new Object();

        public ExecutorEventDispatcher() : this(new Dictionary<int, List<IEventHandler>>(2),
                    new List<IEventHandler>())
        {

        }

        public ExecutorEventDispatcher(
                Dictionary<int, List<IEventHandler>> handlersByEventType,
                List<IEventHandler> genericHandlers)
        {
            this.handlersByEventType = handlersByEventType;
            this.genericHandlers = genericHandlers;
            this.isShuttingDown = false;
        }


        public void addHandler(IEventHandler eventHandler)
        {
            int eventType = eventHandler.getEventType();
            lock (locker)
            {
                if (eventType == Events.ANY)
                {
                    genericHandlers.Add(eventHandler);
                }
                else
                {
                    if (!handlersByEventType.TryGetValue(eventType, out var handlers))
                    {
                        handlers = new List<IEventHandler>();
                        this.handlersByEventType.Add(eventType, handlers);
                    }

                    handlers.Add(eventHandler);
                }
            }
        }


        public List<IEventHandler> getHandlers(int eventType)
        {
            return handlersByEventType[eventType];
        }


        public void removeHandler(IEventHandler eventHandler)
        {
            int eventType = eventHandler.getEventType();
            lock (locker)
            {
                if (eventType == Events.ANY)
                {
                    genericHandlers.Remove(eventHandler);
                }
                else
                {
                    if (handlersByEventType.TryGetValue(eventType, out var handlers))
                    {
                        handlers.Remove(eventHandler);
                        // Remove the reference if there are no listeners left.
                        if (handlers.Count == 0)
                        {
                            handlersByEventType.Add(eventType, null);
                        }
                    }
                }
            }

        }


        public void removeHandlersForEvent(int eventType)
        {
            lock (locker)
            {
                if (handlersByEventType.TryGetValue(eventType, out var handlers))
                {
                    handlers.Clear();
                }
            }
        }


        public bool removeHandlersForSession(ISession session)
        {
            List<IEventHandler> removeList = new List<IEventHandler>();
            var eventHandlersList = handlersByEventType
                    .Values;
            foreach (List<IEventHandler> handlerList in eventHandlersList)
            {
                if (null != handlerList)
                {
                    foreach (IEventHandler handler in handlerList)
                    {
                        if (handler is SessionEventHandler)
                        {
                            SessionEventHandler sessionHandler = (SessionEventHandler)handler;
                            if (sessionHandler.Session.Equals(session))
                            {
                                removeList.Add(handler);
                            }
                        }
                    }
                }
            }
            foreach (IEventHandler handler in removeList)
            {
                removeHandler(handler);
            }
            return (removeList.Count > 0);
        }


        public void clear()
        {
            lock (locker)
            {
                if (null != handlersByEventType)
                {
                    handlersByEventType.Clear();
                }
                if (null != genericHandlers)
                {
                    genericHandlers.Clear();
                }
            }
        }


        public void fireEvent(IEvent @event)
        {
            bool isShuttingDown = false;
            lock (locker)


            {
                isShuttingDown = this.isShuttingDown;
            }
            if (!isShuttingDown)
            {
                Task.Run(() =>
                {
                    foreach (IEventHandler handler in genericHandlers)
                    {
                        handler.onEvent(@event);
                    }

                    if (handlersByEventType.TryGetValue(@event.getType(), out var handlers))
                    {
                        foreach (IEventHandler handler in handlers.ToArray())
                        {
                            handler.onEvent(@event);
                        }
                    }

                });

            }

            else
            {
                _logger.Error("Discarding event: " + @event + " as dispatcher is shutting down");
            }

        }


        public void close()
        {
            lock (locker)
            {
                isShuttingDown = true;
                genericHandlers.Clear();
                handlersByEventType.Clear();
            }

        }
    }
}
