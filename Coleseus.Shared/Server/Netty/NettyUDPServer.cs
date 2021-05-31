using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coleseus.Shared.Server.Netty
{

    /**
	 * This class is used for TCP IP communications with client. It uses Netty tcp
	 * server bootstrap for this.
	 * 
	 * @author Abraham Menacherry
	 * 
	 */
    public class NettyUDPServer : AbstractNettyServer
    {
        private readonly ILogger<NettyUDPServer> _logger;

        private Bootstrap serverBootstrap;

        public NettyUDPServer(NettyConfig nettyConfig,
                IChannelHandler channelInitializer) : base(nettyConfig, channelInitializer)
        {

        }


        public override async Task startServer()
        {
         
            try
            {
                serverBootstrap = new Bootstrap();
                Dictionary<ChannelOption<dynamic>, Object> channelOptions = nettyConfig.getChannelOptions();
                if (null != channelOptions)
                {
                    var keySet = channelOptions.Keys;
                    foreach (ChannelOption<dynamic> option in keySet)
                    {
                        serverBootstrap.Option(option, channelOptions[option]);
                    }
                }
                serverBootstrap.Group(nettyConfig.getBossGroup())
                                  .Channel<SocketDatagramChannel>()
                    .Handler(getChannelInitializer());
                IChannel serverChannel = await serverBootstrap.BindAsync(nettyConfig.getSocketAddress());

                ALL_CHANNELS.Add(serverChannel);
            }
            catch (Exception e)
            {
                _logger.LogError("TCP Server start error {}, going to shut down", e);
                await base.stopServer();
                throw e;
            }
        }


        public override TRANSMISSION_PROTOCOL getTransmissionProtocol()
        {
            return TRANSMISSION_PROTOCOL.TCP;
        }


        public void SetChannelInitializer(IChannelHandler initializer)
        {
            this.channelInitializer = initializer;
            serverBootstrap.Handler(initializer);
        }


        public override string ToString()
        {
            return "NettyTCPServer [socketAddress=" + nettyConfig.getSocketAddress()
                    + ", portNumber=" + nettyConfig.getPortNumber() + "]";
        }


    }
}
