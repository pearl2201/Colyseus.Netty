using Coleseus.Shared.App;
using Coleseus.Shared.App.Impl;
using Coleseus.Shared.Event;
using Coleseus.Shared.Event.Impl;
using Coleseus.Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyseus.NettyServer.LostDecade
{
    public class LDRoom : GameRoomSession
    {
        private const int canvasWidth = 512;
        private const int canvasHeight = 480;

        public LDRoom(GameRoomSessionBuilder builder) : base(builder)
        {

            addHandler(new GameSessionHandler(this));
        }

        public override void onLogin(IPlayerSession playerSession)
        {
            playerSession.addHandler(new LDRoomPlayerSessionHandler(playerSession));

            Entity hero = createHero(playerSession);
            LDGameState state = (LDGameState)getStateManager().State;
            state.Entities.Add(hero);
            sendBroadcast(Events.NetworkEvent(new LDGameState(state.Entities,
                state.Monster, hero)));
        }

        private Entity createHero( IPlayerSession playerSession)
        {
            Entity hero = new Entity()
            {
                Id = (string)playerSession.getId(),
                Score = 0,
                Type = Entity.HERO,
                Y = canvasHeight / 2,
                X = canvasWidth / 2,
                Speed = 256
            };
            return hero;
        }

        public static Entity createMonster()
        {
            Entity monster = new Entity();
            monster.Type = (Entity.MONSTER);
            monster.X = (getRandomPos(canvasWidth));
            monster.Y = (getRandomPos(canvasHeight));
            return monster;
        }
        private static int getRandomPos(int axisVal)
        {
            Random r = new Random();
            long round = (long)Math.Round((decimal)(32 + (r.Next() * (axisVal - 64))));
            return (int)round;
        }

    }

    public class LDRoomPlayerSessionHandler : DefaultSessionEventHandler
    {

        public LDRoomPlayerSessionHandler(IPlayerSession session) : base(session)
        {

        }

        public override void onDataIn(IEvent @event)
        {
            if (null != @event.getSource())
            {
                // Pass the player session in the event context so that the
                // game room knows which player session send the message.
                @event.setEventContext(new DefaultEventContext(
                        session, null));
                // pass the event to the game room
                ((IPlayerSession)session).getGameRoom().send(@event);
            }
        }
    }


    public class GameSessionHandler : SessionMessageHandler
    {
        private Entity monster;
        private GameRoom room;
        public GameSessionHandler(GameRoomSession session) : base(session)
        {
            this.room = session;
            IGameStateManagerService manager = room.getStateManager();
            LDGameState state = (LDGameState)manager.State;

            state = new LDGameState();
            state.Entities = (new HashSet<Entity>());
            state.Monster = LDRoom.createMonster();
            manager.State = (state); // set it back on the room
            this.monster = state.Monster;
        }
        public override void onEvent(IEvent @event)
        {
            throw new NotImplementedException();
        }

        private void update(Entity hero, ISession session)
        {

        }

        private void Reset()
        {
        }
    }
}
