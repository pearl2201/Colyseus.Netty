using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colyseus.Server.Discovery
{
    public class Node
    {
        public int Port { get; set; }

        public string ProcessId { get; set; }
    }
    public class Discovery
    {
        public  string GetNodeAddress(Node node)
        {
            //const host = process.env.SELF_HOSTNAME || await ip.v4();
            //const port = process.env.SELF_PORT || node.port;
            //return `${ node.processId}/${ host}:${ port}`;
            return "";
        }

        public string RegisterNode(IPresence presence, Node node)
        {
            return "";
        }

        public void UnregisterNode(IPresence presence, Node node)
        {

        }
    }
}
