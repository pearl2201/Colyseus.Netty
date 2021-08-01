using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyseus.NettyServer.LostDecade
{
    public class Entity
    {
        public String Id { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Speed { get; set; }
        public int Key { get; set; }

        public bool Press { get; set; }

        /**
		 * Is it a monster or hero?
		 */
        public string Type { get; set; }

        /**
		 * Only heroes will have it.
		 */
        public int Score { get; set; }

        public const string MONSTER = "MONSTER";
        public const string HERO = "HERO";
    }
}
