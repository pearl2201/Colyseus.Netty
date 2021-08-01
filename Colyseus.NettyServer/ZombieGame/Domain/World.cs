using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
namespace Colyseus.NettyServer.ZombieGame.Domain
{

	public class World
	{
		private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<World>();
		private volatile int alive;
		private volatile int undead;

		public bool apocalypse()
		{
			if (alive <= 0)
			{
				return true;
			}
			return false;
		}

		public void report()
		{
			if (alive > 0)
			{
				_logger.Verbose("alive= {} undead= {}", alive, undead);
			}
		}

		public int getAlive()
		{
			return alive;
		}

		public void setAlive(int alive)
		{
			this.alive = alive;
		}

		public int getUndead()
		{
			return undead;
		}

		public void setUndead(int undead)
		{
			this.undead = undead;
		}

		public void shotgun()
		{
			int newUndead = undead - 1;
			_logger.Verbose("Defender update, undead = " + undead + " new undead: " + newUndead);
			undead = newUndead;
		}

		public void eatBrains()
		{
			_logger.Verbose("In eatBrains Alive: {} Undead: {}", alive, undead);
			alive--;
			undead += 2;
			_logger.Verbose("New Alive: {} Undead: {}", alive, undead);
		}

	}

}
