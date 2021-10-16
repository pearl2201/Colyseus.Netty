using Coleseus.Shared.Communication;
using Coleseus.Shared.Event;
using Coleseus.Shared.Event.Impl;
using Coleseus.Shared.Service;
using Coleseus.Shared.Service.Impl;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Coleseus.Shared.App.Impl
{

    /**
	 * The default implementation of the session class. This class is responsible
	 * for receiving and sending events. For receiving it uses the
	 * {@link #onEvent(Event)} method and for sending it uses the
	 * {@link EventDispatcher} fireEvent method. The Method {@link #setId(Object)}
	 * will throw {@link IllegalArgumentException} in this implementation class.
	 * 
	 * @author Abraham Menacherry
	 * 
	 */
    public class DefaultSession : ISession
    {
        /**
         * session id
         */
        protected readonly object id;
        /**
         * event dispatcher
         */
        public IEventDispatcher EventDispatcher { get; protected set; }

        /**
         * session parameters
         */
        protected Dictionary<String, Object> sessionAttributes;

        public DateTime CreationTime { get; protected set; }

        public DateTime LastReadWriteTime { get; set; }

        public SessionStatus Status { get; set; }

        public bool IsWriteable { get; set; }

        /**
         * Life cycle variable to check if the session is shutting down. If it is, then no
         * more incoming events will be accepted.
         */
        public bool IsShuttingDown { get; protected set; }

        public bool IsUDPEnabled { get; set; }

        public Reliable TcpSender { get; set; } = null ;

        public Fast UdpSender { get; set; } = null;

        public DefaultSession(SessionBuilder sessionBuilder)
        {
            // validate variables and provide default values if necessary. Normally
            // done in the builder.build() method, but done here since this class is
            // meant to be overriden and this could be easier.
            sessionBuilder.validateAndSetValues();
            this.id = sessionBuilder.id;
            this.EventDispatcher = sessionBuilder.eventDispatcher;
            this.sessionAttributes = sessionBuilder.sessionAttributes;
            this.CreationTime = sessionBuilder.creationTime;
            this.Status = sessionBuilder.status;
            this.LastReadWriteTime = sessionBuilder.lastReadWriteTime;
            this.IsWriteable = sessionBuilder.isWriteable;
            this.IsShuttingDown = sessionBuilder.isShuttingDown;
            this.IsUDPEnabled = sessionBuilder.isUDPEnabled;
        }



        public void OnEvent(IEvent @event)
        {
            if (!IsShuttingDown)
            {
                EventDispatcher.fireEvent(@event);
            }
        }


        public Object GetId()
        {
            return id;
        }


        public void SetId(Object id)
        {
            throw new ArgumentException("id cannot be set in this implementation, since it is final");
        }





        public void AddHandler(IEventHandler eventHandler)
        {
            EventDispatcher.addHandler(eventHandler);
        }


        public void RemoveHandler(IEventHandler eventHandler)
        {
            EventDispatcher.removeHandler(eventHandler);
        }


        public List<IEventHandler> GetEventHandlers(int eventType)
        {
            return EventDispatcher.getHandlers(eventType);
        }


        public Object GetAttribute(string key)
        {
            return sessionAttributes[key];
        }


        public void RemoveAttribute(String key)
        {
            sessionAttributes.Remove(key);
        }


        public void SetAttribute(String key, Object value)
        {
            sessionAttributes.Add(key, value);
        }








        public bool IsConnected()
        {
            return this.Status == SessionStatus.CONNECTED;
        }



        /**
         * Not synchronized because default implementation does not care whether a
         * duplicated message sender is created.
         * 
         * @see io.nadron.app.Session#isUDPEnabled()
         */


        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Close()
        {
            IsShuttingDown = true;
            EventDispatcher.close();
            if (null != TcpSender)
            {
                TcpSender.close();
                TcpSender = null;
            }
            if (null != UdpSender)
            {
                UdpSender.close();
                UdpSender = null;
            }
            this.Status = SessionStatus.CLOSED;
        }



        public Dictionary<String, Object> getSessionAttributes()
        {
            return sessionAttributes;
        }


        public int hashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result
                    + ((id == null) ? 0 : id.GetHashCode());
            return result;
        }


        public bool equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            DefaultSession other = (DefaultSession)obj;
            if (id == null)
            {
                if (other.id != null)
                    return false;
            }
            else if (!id.Equals(other.id))
                return false;
            return true;
        }


  


  
    }

    /**
      * This class is roughly based on Joshua Bloch's Builder pattern. Since
      * Session class will be extended by child classes, the
      * {@link #validateAndSetValues()} method on this builder is actually called
      * by the {@link DefaultSession} constructor for ease of use. May not be good
      * design though.
      * 
      * @author Abraham, Menacherry
      * 
      */
    public class SessionBuilder
    {

        public Object id { get; set; }
        public IEventDispatcher eventDispatcher { get; set; } = null;
        public Dictionary<String, Object> sessionAttributes { get; set; } = null;
        public DateTime creationTime { get; set; } = DateTime.MinValue;
        public DateTime lastReadWriteTime { get; set; } = DateTime.MinValue;
        public SessionStatus status { get; set; } = SessionStatus.NOT_CONNECTED;
        public bool isWriteable { get; set; } = true;
        public bool isShuttingDown { get; set; } = false;
        public bool isUDPEnabled { get; set; } = false;// By default UDP is not enabled.

        public ISession build()
        {
            return new DefaultSession(this);
        }

        /**
         * This method is used to validate and set the variables to default
         * values if they are not already set before calling build. This method
         * is invoked by the constructor of SessionBuilder. <b>Important!</b>
         * Builder child classes which override this method need to call
         * super.validateAndSetValues(), otherwise you could get runtime NPE's.
         */
        public virtual void validateAndSetValues()
        {
            if (id == null)
            {
                id = Guid.NewGuid().ToString();
            }
            if (null == eventDispatcher)
            {
                eventDispatcher = new ExecutorEventDispatcher();
            }
            if (null == sessionAttributes)
            {
                sessionAttributes = new Dictionary<string, object>();
            }
            creationTime = DateTime.UtcNow;
        }


    }


}
