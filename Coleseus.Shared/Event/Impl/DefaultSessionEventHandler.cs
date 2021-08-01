using Coleseus.Shared.App;
using Coleseus.Shared.Communication;
using Coleseus.Shared.Event;
using Coleseus.Shared.Service;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Event.Impl
{
    /**
  * This class will handle any event that gets published to a
  * {@link Session#onEvent(@event)}. The event dispatcher will route all events
  * to this class's {@link #onEvent(@event)} method. It provides default
  * implementations for common events defined in the server. <b>Note</b> invoking
  * {@link #setSession(Session)} method on this class will result in an
  * {@link UnsupportedOperationException} since the session is a final variable
  * of this class.
  * 
  * @author Abraham Menacherry
  * 
  */
    public class DefaultSessionEventHandler : SessionEventHandler
    {
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<DefaultSessionEventHandler>();

        protected readonly ISession session;

        public DefaultSessionEventHandler(ISession session)
        {
            this.session = session;
        }


        public int getEventType()
        {
            return Events.ANY;
        }


        public void onEvent(IEvent @event)
        {
            doEventHandlerMethodLookup(@event);
        }

        protected void doEventHandlerMethodLookup(IEvent @event)
        {
            switch (@event.getType())
            {
                case Events.SESSION_MESSAGE:
                    onDataIn(@event);
                    break;
                case Events.NETWORK_MESSAGE:
                    onNetworkMessage((INetworkEvent)@event);
                    break;
                case Events.LOG_IN_SUCCESS:
                    onLoginSuccess(@event);
                    break;
                case Events.LOG_IN_FAILURE:
                    onLoginFailure(@event);
                    break;
                case Events.CONNECT:
                    onConnect((ConnectEvent)@event);
                    break;
                case Events.START:
                    onStart(@event);
                    break;
                case Events.STOP:
                    onStart(@event);
                    break;
                case Events.CONNECT_FAILED:
                    onConnectFailed(@event);
                    break;
                case Events.DISCONNECT:
                    onDisconnect(@event);
                    break;
                case Events.CHANGE_ATTRIBUTE:
                    onChangeAttribute((ChangeAttributeEvent)@event);
                    break;
                case Events.EXCEPTION:
                    onException(@event);
                    break;
                case Events.RECONNECT:
                    onReconnect((ConnectEvent)@event);
                    break;
                case Events.LOG_OUT:
                    onLogout(@event);
                    break;
                default:
                    onCustomEvent(@event);
                    break;
            }
        }

        public virtual void onDataIn(IEvent @event)
        {
            if (null != getSession())
            {
                IPlayerSession pSession = (IPlayerSession)getSession();
                INetworkEvent networkEvent = new DefaultNetworkEvent(@event);
                if (pSession.isUDPEnabled)
                {
                    networkEvent.setDeliveryGuaranty(DeliveryGuaranty.FAST);
                }
                pSession.getGameRoom().sendBroadcast(networkEvent);
            }
        }

        protected void onNetworkMessage(INetworkEvent @event)
        {
            ISession session = getSession();
            if (!session.isWriteable)
                return;
            DeliveryGuaranty guaranty = @event.getDeliveryGuaranty();
            if (guaranty == DeliveryGuaranty.FAST)
            {
                Fast udpSender = session.getUdpSender();
                if (null != udpSender)
                {
                    udpSender.sendMessage(@event);
                }
                else
                {
                    _logger.Verbose(
                            "Going to discard event: {} since udpSender is null in session: {}",
                            @event, session);
                }
            }
            else
            {
                session.getTcpSender().sendMessage(@event);
            }
        }

        protected void onLoginSuccess(IEvent @event)
        {
            getSession().getTcpSender().sendMessage(@event);
        }

        protected void onLoginFailure(IEvent @event)
        {
            getSession().getTcpSender().sendMessage(@event);
        }

        protected void onConnect(ConnectEvent @event)
        {
            ISession session = getSession();
            if (null != @event.getTcpSender())
            {
                session.setTcpSender(@event.getTcpSender());
            }

            else
            {
                if (null == getSession().getTcpSender())
                {
                    logNullTcpConnection(@event);
                }
                else
                {
                    session.isUDPEnabled = true;
                    session.setUdpSender(@event.getUdpSender());
                }
            }
        }

        protected void onReconnect(ConnectEvent @event)
        {
            ISession session = getSession();
            // To synchronize with task for closing session in ReconnectRegistry service.
            lock (session)
            {



                ISessionRegistryService<String> reconnectRegistry = ((ISessionRegistryService<String>)session
                        .getAttribute(ColyseusConfig.RECONNECT_REGISTRY));
                if (null != reconnectRegistry && SessionStatus.CLOSED != session.status)
                {
                    reconnectRegistry.removeSession((String)session
                            .getAttribute(ColyseusConfig.RECONNECT_KEY));
                }
            }
            onConnect(@event);
        }

        protected void onStart(IEvent @event)
        {
            getSession().getTcpSender().sendMessage(@event);
        }

        protected void onStop(IEvent @event)
        {
            getSession().getTcpSender().sendMessage(@event);
        }

        protected void onConnectFailed(IEvent @event)
        {

        }

        protected void onDisconnect(IEvent @event)
        {
            _logger.Debug("Received disconnect event in session. ");
            onException(@event);
        }

        protected void onChangeAttribute(ChangeAttributeEvent @event)
        {
            getSession().setAttribute(@event.getKey(), @event.getValue());
        }



        protected void onException(IEvent @event)
        {
            ISession session = getSession();
            session.status = (SessionStatus.NOT_CONNECTED);
            session.isWriteable = false;
            session.isUDPEnabled = false;// will be set to true by udpupstream handler on connect event.
            string reconnectKey = (string)session
                    .getAttribute(ColyseusConfig.RECONNECT_KEY);
            ISessionRegistryService<String> registry = (ISessionRegistryService<string>)session.getAttribute(ColyseusConfig.RECONNECT_REGISTRY);
            if (null != reconnectKey && null != registry)
            {
                // If session is already in registry then do not re-register.
                if (null == registry.getSession(reconnectKey))
                {
                    registry.putSession(
                            reconnectKey, getSession());
                    _logger.Debug("Received exception/disconnect event in session. "
                        + "Going to put session in reconnection registry");
                }
            }
            else
            {
                _logger.Debug("Received exception/disconnect event in session. "
                        + "Going to close session");
                onClose(@event);
            }
        }

        protected void onLogout(IEvent @event)
        {
            onClose(@event);
        }

        protected void onClose(IEvent @event)
        {
            getSession().close();
        }

        protected void onCustomEvent(IEvent @event)
        {

        }

        public ISession getSession()
        {
            return session;
        }

        public void setSession(ISession session)
        {
            throw new InvalidOperationException("Session is a final variable and cannot be reset.");
        }

        private void logNullTcpConnection(IEvent @event)
        {
            _logger.Warning("Discarding {} as TCP connection is not fully "
                    + "established for this {}", @event, getSession());
        }
    }

}
