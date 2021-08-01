using Coleseus.Shared.App;
using Coleseus.Shared.App.Impl;
using Colyseus.NettyServer.ZombieGame.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyseus.NettyServer.ZombieGame.Game
{
    public class ZombieRoom : GameRoomSession
    {

        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<ZombieRoom>();
        private Defender defender;
        private Zombie zombie;

        public ZombieRoom(GameRoomSessionBuilder sessionBuilder) : base(sessionBuilder)
        {

        }

        public ZombieRoom(GameRoomSessionBuilder sessionBuilder, World world, Defender defender, Zombie zombie) : base(sessionBuilder)
        {

            this.defender = defender;
            this.zombie = zombie;
        }


        public override void onLogin(IPlayerSession playerSession)
        {
            SessionHandler listener = new SessionHandler(playerSession, defender, zombie,
                    IAM.ZOMBIE);
            playerSession.addHandler(listener);
            _logger.Verbose("Added event listener in Zombie Room");
        }

        public Defender getDefender()
        {
            return defender;
        }

        public void setDefender(Defender defender)
        {
            this.defender = defender;
        }

        public Zombie getZombie()
        {
            return zombie;
        }

        public void setZombie(Zombie zombie)
        {
            this.zombie = zombie;
        }
    }

}
