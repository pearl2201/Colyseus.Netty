using Coleseus.Shared.Event;
using Coleseus.Shared.Event.Impl;
using Coleseus.Shared.Protocols;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Coleseus.Shared.App.Impl
{
    /**
 * This implementation of the {@link PlayerSession} interface is used to both
 * receive and send messages to a particular player using the
 * {@link #onEvent(io.nadron.event.Event)}. Broadcasts from the
 * {@link GameRoom} are directly patched to the {@link EventDispatcher} which
 * listens on the room's {@link MemoryChannel} for events and in turn publishes
 * them to the listeners.
 * 
 * @author Abraham Menacherry
 * 
 */
    public class DefaultPlayerSession : DefaultSession, IPlayerSession
    {
        /**
		 * Each session belongs to a Player. This variable holds the reference.
		 */
        protected readonly IPlayer player;

        /**
         * Each incoming connection is made to a game room. This reference holds the
         * association to the game room.
         */
        protected GameRoom parentGameRoom;
        /**
         * This variable holds information about the type of binary communication
         * protocol to be used with this session.
         */
        protected IProtocol protocol;

        public DefaultPlayerSession(PlayerSessionBuilder playerSessionBuilder) : base(playerSessionBuilder)
        {

            this.player = playerSessionBuilder.player;
            this.parentGameRoom = playerSessionBuilder.parentGameRoom;
            this.protocol = playerSessionBuilder.protocol;
        }




        public IPlayer getPlayer()
        {
            return player;
        }

        public GameRoom getGameRoom()
        {
            return parentGameRoom;
        }

        public void setGameRoom(GameRoom gameRoom)
        {
            this.parentGameRoom = gameRoom;
        }


        public IProtocol getProtocol()
        {
            return protocol;
        }


        public void setProtocol(IProtocol protocol)
        {
            this.protocol = protocol;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void close()
        {
            if (!isShuttingDown)
            {
                base.close();
                parentGameRoom.disconnectSession(this);
            }
        }


        public void sendToGameRoom(IEvent @event)
        {
            parentGameRoom.send(@event);
        }


        public String toString()
        {
            return "PlayerSession [id=" + id + "player=" + player
                    + ", parentGameRoom=" + parentGameRoom + ", protocol="
                    + protocol + ", isShuttingDown=" + isShuttingDown + "]";
        }
    }

    public class PlayerSessionBuilder : SessionBuilder
    {

        public IPlayer player { get; set; }
        public GameRoom parentGameRoom { get; set; }
        public IProtocol protocol { get; set; }

        public IPlayerSession build()
        {
            return new DefaultPlayerSession(this);
        }



        public PlayerSessionBuilder SetParentGameRoom(GameRoom parentGameRoom)
        {
            if (null == parentGameRoom)
            {
                throw new ArgumentException(
                        "GameRoom instance is null, session will not be constructed");
            }
            this.parentGameRoom = parentGameRoom;
            return this;
        }


        public override void validateAndSetValues()
        {
            if (null == eventDispatcher)
            {
                eventDispatcher = EventDispatchers.newJetlangEventDispatcher(
                        parentGameRoom, LaneStrategies.GROUP_BY_ROOM);
            }
            base.validateAndSetValues();
        }

        public PlayerSessionBuilder SetProtocol(IProtocol protocol)
        {
            this.protocol = protocol;
            return this;
        }
    }

}
