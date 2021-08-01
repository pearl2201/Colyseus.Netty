using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyseus.NettyServer.LostDecade
{
    public class LDGameState
    {
        public HashSet<Entity> Entities { get; set; }
        public Entity Monster { get; set; }
        public Entity Hero { get; set; }
        public bool Reset { get; set; }

        public LDGameState()
        {

        }


        public LDGameState(HashSet<Entity> entities, Entity monster, Entity hero)
        {
          
            this.Entities = entities;
            this.Monster = monster;
            this.Hero = hero;
        }

        public void AddEntitiy(Entity hero)
        {
            // only the id will match, but other values maybe different.
            Entities.Remove(hero);
            Entities.Add(hero);
        }
    }
}
