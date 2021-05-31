using Coleseus.Shared.Service;
using DotNetty.Common.Concurrency;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Coleseus.Shared.Server.Netty
{
    public abstract class AbstractNettyServer : NettyServer
    {
        private readonly ILogger<AbstractNettyServer> _logger;
        public static IChannelGroup ALL_CHANNELS = new DefaultChannelGroup(new SingleThreadEventExecutor("TcpNettyServer", TimeSpan.FromSeconds(10)));
        protected IGameAdminService gameAdminService;
        protected NettyConfig nettyConfig;
        protected IChannelHandler channelInitializer;

        public AbstractNettyServer(NettyConfig nettyConfig,
               IChannelHandler channelInitializer)
        {
            this.nettyConfig = nettyConfig;
            this.channelInitializer = channelInitializer;
        }


        public abstract Task startServer();

        public async Task startServer(int port)
        {
            nettyConfig.setPortNumber(port);
            nettyConfig.setSocketAddress(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
            await startServer();
        }


        public async Task startServer(IPEndPoint socketAddress)
        {
            nettyConfig.setSocketAddress(socketAddress);
            await startServer();
        }


        public async Task stopServer()
        {
            _logger.LogDebug("In stopServer method of class: {}", this.GetType()
                    .Name);
            ;
            try

            {
                await ALL_CHANNELS.CloseAsync();
            }
            catch (Exception e)

            {
                _logger.LogError(
                        "Execption occurred while waiting for channels to close: {}",
                        e);
            }
            finally

            {
                // TODO move this part to spring.
                if (null != nettyConfig.getBossGroup())
                {
                    await nettyConfig.getBossGroup().ShutdownGracefullyAsync();
                }
                if (null != nettyConfig.getWorkerGroup())
                {
                    await nettyConfig.getWorkerGroup().ShutdownGracefullyAsync();
                }
                gameAdminService.shutdown();
            }
        }


        public IChannelHandler getChannelInitializer()
        {
            return channelInitializer;
        }


        public NettyConfig getNettyConfig()
        {
            return nettyConfig;
        }

        protected IEventLoopGroup getBossGroup()
        {
            return nettyConfig.getBossGroup();
        }

        protected IEventLoopGroup getWorkerGroup()
        {
            return nettyConfig.getWorkerGroup();
        }

        public IGameAdminService getGameAdminService()
        {
            return gameAdminService;
        }

        public void setGameAdminService(IGameAdminService gameAdminService)
        {
            this.gameAdminService = gameAdminService;
        }


        public IPEndPoint getSocketAddress()
        {
            return nettyConfig.getSocketAddress();
        }


        public String toString()
        {
            return "NettyServer [socketAddress=" + nettyConfig.getSocketAddress()
                    + ", portNumber=" + nettyConfig.getPortNumber() + "]";
        }



        public abstract TRANSMISSION_PROTOCOL getTransmissionProtocol();

        public void setChannelInitializer(IChannelHandler initializer)
        {
            this.channelInitializer = initializer;
        }
    }
}
