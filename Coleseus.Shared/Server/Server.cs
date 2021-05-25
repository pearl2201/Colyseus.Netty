using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Coleseus.Shared.Server
{


    public enum TRANSMISSION_PROTOCOL
    {
        TCP,
        UDP
    }

    public interface IServer
    {


        TRANSMISSION_PROTOCOL getTransmissionProtocol();

        Task startServer();

        Task startServer(int port);

        Task startServer(IPEndPoint socketAddress);

        Task stopServer();

        IPEndPoint getSocketAddress();
    }
}
