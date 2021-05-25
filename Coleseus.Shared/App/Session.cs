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

        IEventDispatcher eventDispatcher { get; }


        bool isWriteable { get; set; }

        /**
		 * A session would not have UDP capability when created. Depending on the
		 * network abilities of the client, it can request UDP communication to be
		 * enabled with the LOGIN_UDP and CONNECT_UDP events of the {@link Events}
		 * class. Once UDP is enabled this flag will be set to true on the session.
		 * 
		 * @return Returns true if the a UDP {@link MessageSender} instance is
		 *         attached to this session, else false.
		 */
        bool isUDPEnabled { get; set; }

        bool isShuttingDown { get; }

        DateTime creationTime { get; }

        DateTime lastReadWriteTime { get; }

      

        SessionStatus status { get; set; }

        bool isConnected();

        void addHandler(IEventHandler eventHandler);

        void removeHandler(IEventHandler eventHandler);

        List<IEventHandler> getEventHandlers(int eventType);

        void close();

        void setUdpSender(Fast udpSender);

        Fast getUdpSender();

        void setTcpSender(Reliable tcpSender);

        Reliable getTcpSender();
    }
}
