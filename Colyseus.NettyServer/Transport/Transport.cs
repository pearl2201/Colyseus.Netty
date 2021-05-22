using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colyseus.Server.Transport
{

 
    public abstract class Transport
    {
       

        public abstract Task Listen(int? port, string hostname, string backlog, Action? listeningListener);
        public abstract Task Shutdown();

        public abstract void SimulateLatency(int milliseconds);
        public abstract System.Net.IPAddress Address {get;}

    }
}
