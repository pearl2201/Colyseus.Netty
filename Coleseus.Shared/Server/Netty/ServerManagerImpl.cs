using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coleseus.Shared.Server.Netty
{
    public class ServerManagerImpl : ServerManager
    {
        private HashSet<AbstractNettyServer> servers;
        private readonly ILogger<ServerManagerImpl> _logger;

        private readonly NettyTCPServer _tcpServer;
        private readonly NettyUDPServer _udpServer;

        public ServerManagerImpl(NettyTCPServer tcpServer, NettyUDPServer udpServer )
        {
            servers = new HashSet<AbstractNettyServer>();
            _tcpServer = tcpServer;
            _udpServer = udpServer;
        }


        public async Task startServers(int tcpPort, int flashPort, int udpPort)
        {

            if (tcpPort > 0)
            {
               
                await _tcpServer.startServer(tcpPort);
                servers.Add(_tcpServer);
            }

            //if (flashPort > 0)
            //{
            //    AbstractNettyServer flashServer = (AbstractNettyServer)AppContext.getBean(AppContext.FLASH_POLICY_SERVER);
            //    await flashServer.startServer(flashPort);
            //    servers.Add(flashServer);
            //}

            if (udpPort > 0)
            {
                
                await _udpServer.startServer(udpPort);
                servers.Add(_udpServer);
            }

        }


        public async Task startServers()
        {
           
            await _tcpServer.startServer();
            servers.Add(_tcpServer);
            //AbstractNettyServer flashServer = (AbstractNettyServer)AppContext.getBean(AppContext.FLASH_POLICY_SERVER);
            //await flashServer.startServer();
            //servers.Add(flashServer);
            
            await _udpServer.startServer();
            servers.Add(_udpServer);
        }



        public async Task stopServers()
        {
            foreach (AbstractNettyServer nettyServer in servers)
            {
                try
                {
                    await nettyServer.stopServer();
                }
                catch (Exception e)
                {
                    _logger.LogError("Unable to stop server {} due to error {}", nettyServer, e);
                    throw e;
                }
            }
        }

    }
}


