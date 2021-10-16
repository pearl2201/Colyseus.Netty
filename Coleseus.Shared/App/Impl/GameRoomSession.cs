using Coleseus.Shared.App;
using Coleseus.Shared.Event;
using Coleseus.Shared.Event.Impl;
using Coleseus.Shared.Protocols;
using Coleseus.Shared.Service;
using Coleseus.Shared.Service.Impl;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Serilog;

namespace Coleseus.Shared.App.Impl
{
    public class GameRoomSessionBuilder : SessionBuilder
    {

        public HashSet<IPlayerSession> sessions;
        public IGame parentGame;
        public String gameRoomName;
        public IProtocol protocol;
        public IGameStateManagerService stateManager;
        public SessionFactory sessionFactory;


        public override void validateAndSetValues()
        {
            id = Guid.NewGuid().ToString();
            if (null == sessionAttributes)
            {
                sessionAttributes = new Dictionary<String, Object>();
            }
            if (null == sessions)
            {
                sessions = new HashSet<IPlayerSession>();
            }

            if (null == stateManager)
            {
                stateManager = new GameStateManager();
            }
            if (null == sessionFactory)
            {
                sessionFactory = Sessions.INSTANCE;
            }
            creationTime = System.DateTime.UtcNow;
        }

        public GameRoomSessionBuilder SetSessions(HashSet<IPlayerSession> sessions)
        {
            this.sessions = sessions;
            return this;
        }

        public GameRoomSessionBuilder SetParentGame(IGame parentGame)
        {
            this.parentGame = parentGame;
            return this;
        }

        public GameRoomSessionBuilder SetGameRoomName(string gameRoomName)
        {
            this.gameRoomName = gameRoomName;
            return this;
        }

        public GameRoomSessionBuilder SetProtocol(IProtocol protocol)
        {
            this.protocol = protocol;
            return this;
        }



        public GameRoomSessionBuilder SetStateManager(
                IGameStateManagerService gameStateManagerService)
        {
            this.stateManager = gameStateManagerService;
            return this;
        }

        public GameRoomSessionBuilder SetSessionFactory(SessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
            return this;
        }
    }




    public abstract class GameRoomSession : DefaultSession, GameRoom
    {
        protected Serilog.ILogger _logger;

        /**
         * The name of the game room, preferably unique across multiple games.
         */
        protected string gameRoomName;
        /**
         * The parent {@link SimpleGame} reference of this game room.
         */
        protected IGame parentGame;
        /**
         * Each game room has separate state manager instances. This variable will
         * manage the state for all the {@link DefaultPlayer}s connected to this game room.
         */
        protected IGameStateManagerService stateManager;

        /**
         * The set of sessions in this object.
         */
        protected HashSet<IPlayerSession> sessions;

        /**
         * Each game room has its own protocol for communication with client.
         */
        protected IProtocol protocol;

        protected SessionFactory sessionFactory;



        private Mutex mute = new Mutex();

        protected GameRoomSession(GameRoomSessionBuilder gameRoomSessionBuilder) : base(gameRoomSessionBuilder)
        {

            this.sessions = gameRoomSessionBuilder.sessions;
            this.parentGame = gameRoomSessionBuilder.parentGame;
            this.gameRoomName = gameRoomSessionBuilder.gameRoomName;
            this.protocol = gameRoomSessionBuilder.protocol;
            this.stateManager = gameRoomSessionBuilder.stateManager;
            this.sessionFactory = gameRoomSessionBuilder.sessionFactory;
            this._logger = Serilog.Log.ForContext<GameRoomSession>();
            if (null == gameRoomSessionBuilder.eventDispatcher)
            {
                EventDispatcher = new ExecutorEventDispatcher();
            }
        }




        public void afterSessionConnect(IPlayerSession playerSession)
        {
            IGameStateManagerService manager = getStateManager();
            if (null != manager)
            {
                Object state = manager.State;
                if (null != state)
                {
                    playerSession.OnEvent(CreateNetworkStateEvent(state));
                }
            }
        }

        public virtual IEvent CreateNetworkStateEvent(Object state)
        {
            return Events.EntireStateEvent(state, Communication.DeliveryGuaranty.RELIABLE);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool disconnectSession(IPlayerSession playerSession)

        {
            bool removeHandlers = this.EventDispatcher.removeHandlersForSession(playerSession);
            //playerSession.getEventDispatcher().clear(); // remove network handlers of the session.
            return (removeHandlers && sessions.Remove(playerSession));
        }


        public void send(Coleseus.Shared.Event.IEvent @event)
        {
            OnEvent(@event);
        }


        public void sendBroadcast(INetworkEvent networkEvent)
        {
            OnEvent(networkEvent);
        }


        public override void Close()
        {
            mute.WaitOne();
            IsShuttingDown = true;
            foreach (IPlayerSession session in sessions)
            {
                session.Close();
            }
            mute.ReleaseMutex();
        }

        public IPlayerSession getSessionInstance(IPlayer player)
        {
            IPlayerSession playerSession = sessionFactory.newPlayerSession(this, player);
            return playerSession;
        }


        public HashSet<IPlayerSession> getSessions()
        {
            return sessions;
        }


        public void setSessions(HashSet<IPlayerSession> sessions)
        {
            this.sessions = sessions;
        }


        public String getGameRoomName()
        {
            return gameRoomName;
        }


        public void setGameRoomName(String gameRoomName)
        {
            this.gameRoomName = gameRoomName;
        }


        public IGame getParentGame()
        {
            return parentGame;
        }


        public void setParentGame(IGame parentGame)
        {
            this.parentGame = parentGame;
        }


        public void setStateManager(IGameStateManagerService stateManager)
        {
            this.stateManager = stateManager;
        }


        public IGameStateManagerService getStateManager()
        {
            return stateManager;
        }


        public IProtocol getProtocol()
        {
            return protocol;
        }


        public void setProtocol(IProtocol protocol)
        {
            this.protocol = protocol;
        }


        public SessionFactory getFactory()
        {
            return sessionFactory;
        }


        public void setFactory(SessionFactory factory)
        {
            this.sessionFactory = factory;
        }




        /**
         * Method which will create and add event handlers of the player session to
         * the Game Room's EventDispatcher.
         * 
         * @param playerSession
         *            The session for which the event handlers are created.
         */
        protected void createAndAddEventHandlers(IPlayerSession playerSession)
        {
            // Create a network event listener for the player session.
            IEventHandler networkEventHandler = new NetworkEventListener(playerSession);
            // Add the handler to the game room's EventDispatcher so that it will
            // pass game room network events to player session session.
            this.EventDispatcher.addHandler(networkEventHandler);
            _logger.Verbose("Added Network handler to "
                    + "EventDispatcher of GameRoom {}, for session: {}", this,
                    playerSession);
        }

        public IPlayerSession createPlayerSession(IPlayer player)
        {
            IPlayerSession playerSession = getSessionInstance(player);
            return playerSession;
        }


        public abstract void onLogin(IPlayerSession playerSession);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool connectSession(IPlayerSession playerSession)

        {
            if (!IsShuttingDown)
            {
                playerSession.Status = (SessionStatus.CONNECTING);
                sessions.Add(playerSession);
                playerSession.setGameRoom(this);
                _logger.Verbose("Protocol to be applied is: {}", protocol.GetType().Name);
                protocol.applyProtocol(playerSession, true);
                createAndAddEventHandlers(playerSession);
                playerSession.Status = (SessionStatus.CONNECTED);
                afterSessionConnect(playerSession);
                return true;
                // TODO send event to all other sessions?
            }
            else
            {
                _logger.Warning("Game Room is shutting down, playerSession {} {}",
                        playerSession, "will not be connected!");
                return false;
            }
        }

    }

}
