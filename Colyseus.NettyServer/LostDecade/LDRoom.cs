using Coleseus.Shared.App;
using Coleseus.Shared.App.Impl;
using Coleseus.Shared.Communication;
using Coleseus.Shared.Event;
using Coleseus.Shared.Event.Impl;
using Coleseus.Shared.Service;
using DotNetty.Buffers;
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

            AddHandler(new GameSessionHandler(this));
        }

        public override void onLogin(IPlayerSession playerSession)
        {
            playerSession.AddHandler(new LDRoomPlayerSessionHandler(playerSession));

            Entity hero = createHero(playerSession);
            LDGameState state = (LDGameState)getStateManager().State;
            state.Entities.Add(hero);
            var buffer = new LDGameState(state.Entities,
                state.Monster, hero);

            sendBroadcast(Events.EntireStateEvent(buffer));


        }


        private Entity createHero(IPlayerSession playerSession)
        {
            Entity hero = new Entity()
            {
                Id = (string)playerSession.GetId(),
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
                            Session, null));
                    // pass the event to the game room
                    ((IPlayerSession)Session).getGameRoom().send(@event);
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
                Entity hero = ((LDGameState)@event.getSource()).Hero;
                ISession session = @event.getEventContext().getSession();
                update(hero, session);
            }

            private void update(Entity hero, ISession session)
            {
                bool isTouching = (hero.X <= monster.Y + 32)
                        && (hero.Y <= monster.Y + 32)
                        && (monster.X <= hero.Y + 32)
                        && (monster.Y <= hero.Y + 32);

                LDGameState state = (LDGameState)room.getStateManager().State;
                hero.Id = ((String)session.GetId());
                if (isTouching)
                {
                    hero.Score += 1;
                    state.AddEntitiy(hero);
                    Reset();
                }
                else
                {
                    state.AddEntitiy(hero);
                }

                // The state of only one hero is updated, no need to send every
                // hero's state.
                // A possible optimization here is not to broadcast state in case
                // the hero has not moved.
                room.sendBroadcast(Events.EntireStateEvent(new LDGameState(null,
                        monster, hero),DeliveryGuaranty.RELIABLE));
            }

            /**
         * When the hero and monster are touching each other it means hero
         * caught the monster, the game board needs to be reset. This method
         * will put the monster in a random position and all the heroes at the
         * center of the board. Also, it will send reset flag as
         * <code>true</code> to clients so that they can reset their own
         * screens.
         */
            private void Reset()
            {
                HashSet<Entity> entities = ((LDGameState)room.getStateManager()
                    .State).Entities;
                foreach (Entity entity in entities)
                {
                    entity.Y = (canvasHeight / 2);
                    entity.X = (canvasWidth / 2);
                }
                monster.Y = (getRandomPos(canvasWidth));
                monster.X = (getRandomPos(canvasHeight));
                // no need to send the entities here since client will do resetting on its own.
                LDGameState ldGameState = new LDGameState(null, monster, null);
                ldGameState.Reset = (true);
                room.sendBroadcast(Events.EntireStateEvent(ldGameState));
            }
        }
    }





}
