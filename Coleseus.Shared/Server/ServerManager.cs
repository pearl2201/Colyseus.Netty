using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coleseus.Shared.Server
{
    /**
 * A generic interface used to manage a server.
 * @author Abraham Menacherry
 *
 */
    public interface ServerManager
    {
        Task startServers(int tcpPort, int flashPort, int udpPort);

        Task startServers();
        /**
		 * Used to stop the server and manage cleanup of resources. 
		 * 
		 */
        Task stopServers();
    }

}
