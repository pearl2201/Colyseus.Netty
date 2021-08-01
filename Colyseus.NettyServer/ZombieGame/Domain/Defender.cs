using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyseus.NettyServer.ZombieGame.Domain
{
	public class Defender
	{
		private World world;

		public void shotgun()
		{
			world.shotgun();
		}

		public World getWorld()
		{
			return world;
		}

		public void setWorld(World world)
		{
			this.world = world;
		}

	}

}
