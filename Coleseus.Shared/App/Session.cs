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


        Object GetId();

        void SetId(Object id);

        void SetAttribute(String key, Object value);

        Object GetAttribute(String key);

        void RemoveAttribute(String key);

        void OnEvent(IEvent @event);

        IEventDispatcher EventDispatcher { get; }


        bool IsWriteable { get; set; }

        /**
		 * A session would not have UDP capability when created. Depending on the
		 * network abilities of the client, it can request UDP communication to be
		 * enabled with the LOGIN_UDP and CONNECT_UDP events of the {@link Events}
		 * class. Once UDP is enabled this flag will be set to true on the session.
		 * 
		 * @return Returns true if the a UDP {@link MessageSender} instance is
		 *         attached to this session, else false.
		 */
        bool IsUDPEnabled { get; set; }

        bool IsShuttingDown { get; }

        DateTime CreationTime { get; }

        DateTime LastReadWriteTime { get; }



        SessionStatus Status { get; set; }

        bool IsConnected();

        void AddHandler(IEventHandler eventHandler);

        void RemoveHandler(IEventHandler eventHandler);

        List<IEventHandler> GetEventHandlers(int eventType);

        void Close();

        Fast UdpSender { get; set; }

        Reliable TcpSender { get; set; }
    }
}
