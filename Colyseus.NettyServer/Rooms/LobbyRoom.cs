using Colyseus.Server.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colyseus.Server.Rooms
{
    public class LobbyRoom<TState, TMetadata> : Room<TState, TMetadata>
    {
        public LobbyRoom(IPresence presence): base(presence)
        {
           
        }

        public override Task<dynamic> OnCreate(dynamic options)
        {
            throw new NotImplementedException();
        }

        public override Task<dynamic> OnJoin(Client client, dynamic options, dynamic auth)
        {
            throw new NotImplementedException();
        }

        public override Task<dynamic> OnLeave(Client client, bool consented)
        {
            throw new NotImplementedException();
        }
    }
}
