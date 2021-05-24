using Coleseus.Shared.Communication;
using Coleseus.Shared.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.App
{
    public enum SessionStatus
    {
        NOT_CONNECTED, CONNECTING, CONNECTED, CLOSED
    }

    public interface ISession
    {
        /**
	 * session status types
	 */


        Object getId();

        void setId(Object id);

        void setAttribute(String key, Object value);

        Object getAttribute(String key);

        void removeAttribute(String key);

        void onEvent(IEvent @event);

        IEventDispatcher getEventDispatcher();


        bool isWriteable();

        void setWriteable(bool writeable);

        /**
		 * A session would not have UDP capability when created. Depending on the
		 * network abilities of the client, it can request UDP communication to be
		 * enabled with the LOGIN_UDP and CONNECT_UDP events of the {@link Events}
		 * class. Once UDP is enabled this flag will be set to true on the session.
		 * 
		 * @return Returns true if the a UDP {@link MessageSender} instance is
		 *         attached to this session, else false.
		 */
        bool isUDPEnabled();

        /**
		 * A session would not have UDP capability when created. Depending on the
		 * network abilities of the client, it can request UDP communication to be
		 * enabled with the LOGIN_UDP and CONNECT_UDP events of the {@link Events}
		 * class. Once UDP {@link MessageSender} instance is attached to the
		 * session, this method should be called with flag to true to signal that
		 * the session is now UDP enabled.
		 * 
		 * @param isEnabled
		 *            Should be true in most use cases. It is used to signal that
		 *            the UDP {@link MessageSender} has been attached to session.
		 */
        void setUDPEnabled(bool isEnabled);

        bool isShuttingDown();

        long getCreationTime();

        long getLastReadWriteTime();

        void setStatus(SessionStatus status);

        SessionStatus getStatus();

        bool isConnected();

        void addHandler(EventHandler eventHandler);

        void removeHandler(EventHandler eventHandler);

        List<EventHandler> getEventHandlers(int eventType);

        void close();

        void setUdpSender(Fast udpSender);

        Fast getUdpSender();

        void setTcpSender(Reliable tcpSender);

        Reliable getTcpSender();
    }
}
