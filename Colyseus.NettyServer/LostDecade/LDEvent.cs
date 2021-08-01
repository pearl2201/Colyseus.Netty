using Coleseus.Shared.Event.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyseus.NettyServer.LostDecade
{
    public class LDEvent : DefaultEvent
    {
        private LDGameState source;


        public override LDGameState getSource()
        {
            return source;
        }

        public void setSource(LDGameState source)
        {
            this.source = source;
        }
    }
}
