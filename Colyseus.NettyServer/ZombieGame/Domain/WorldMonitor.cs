using Coleseus.Shared.App;
using Coleseus.Shared.App.Impl;
using Coleseus.Shared.Communication;
using Coleseus.Shared.Event;
using Colyseus.NettyServer.ZombieGame.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyseus.NettyServer.ZombieGame.Domain
{
    public class WorldMonitor : AbstractScheduleTask
    {
        private World world;
        private GameRoom room;

        private Object id;

        public WorldMonitor(World world, GameRoom room)
        {
            this.world = world;
            this.room = room;
            id = Guid.NewGuid();
        }

        public World getWorld()
        {
            return world;
        }

        public void setWorld(World world)
        {
            this.world = world;
        }

        public GameRoom getRoom()
        {
            return room;
        }

        public void setRoom(GameRoom room)
        {
            this.room = room;
        }

        public override void Execute()
        {
            if (world.apocalypse())
            {
                // Send it to all players
                Console.WriteLine("Apocalypse is here");
                INetworkEvent networkEvent = Events.NetworkEvent(Messages.apocalypse());
                room.sendBroadcast(networkEvent);
            }
            else
            {
                INetworkEvent networkEvent = null;

                NettyMessageBuffer buffer = new NettyMessageBuffer();
                buffer.writeInt(world.getAlive());
                networkEvent = Events.NetworkEvent(buffer, DeliveryGuaranty.FAST);

                room.sendBroadcast(networkEvent);
            }

            world.report();
        }
    }
}
