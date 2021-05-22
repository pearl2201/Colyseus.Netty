using Colyseus.Server.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colyseus.Server.Serializer
{
    public interface ISerializer<T>
    {
        string Id { get; set; }
        void Reset(dynamic data);
        dynamic GetFullState(Client client);
        bool ApplyPatches(Client[] clients, T state);
        int[] handshake();
    }
}
