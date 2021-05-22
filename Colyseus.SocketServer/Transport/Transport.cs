using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colyseus.Server.Transport
{

    public interface IServer
    {
        public System.Net.IPAddress Address();
    }
    public abstract class Transport
    {
        public IServer server;

        public abstract Transport Listen(int? port, string hostname, string backlog, Action? listeningListener);
        public abstract void Shutdown();

        public abstract void SimulateLatency(int milliseconds);
        public System.Net.IPAddress Address()
        {
            return server.Address();
        }
    }
}
