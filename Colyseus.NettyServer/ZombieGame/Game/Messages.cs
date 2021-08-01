using Colyseus.NettyServer.ZombieGame.Domain;
using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyseus.NettyServer.ZombieGame.Game
{
    public class Messages
    {
		public static IByteBuffer apocalypse()
		{
			IByteBuffer buffer = Unpooled.Buffer(4);
			int cmd = (int)ZombieCommands.APOCALYPSE;
			buffer.WriteInt(cmd);
			return buffer;
		}
	}
}
