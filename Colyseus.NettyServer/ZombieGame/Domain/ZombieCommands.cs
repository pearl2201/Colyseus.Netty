using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyseus.NettyServer.ZombieGame.Domain
{
    public enum ZombieCommands : int
    {
        SHOT_GUN = 1, EAT_BRAINS = 2, SELECT_TEAM = 3, APOCALYPSE = 4, UNKNOWN = -1
    }
}
