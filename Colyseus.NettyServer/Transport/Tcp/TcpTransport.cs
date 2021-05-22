using System;
using Colyseus.Server.Transport;
using Colyseus.Common;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Console;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LogLevel = DotNetty.Handlers.Logging.LogLevel;
using Colyseus.NettyServer;
using System.Net;

public class NettyTcpTransport : Transport
{

    private IChannel bootstrapChannel;

    private ServerBootstrap bootstrap;


    private int port;

    private string hostname;

    public NettyTcpTransport()
    {
    }

    public override IPAddress Address => new IPAddress();

    public override async Task Listen(int? port, string hostname, string backlog, Action listeningListener)
    {
        var logLevel = LogEventLevel.Information;


        var serverPort = 8080;

        var bossGroup = new MultithreadEventLoopGroup(1); //  accepts an incoming connection
        var workerGroup = new MultithreadEventLoopGroup(); // handles the traffic of the accepted connection once the boss accepts the connection and registers the accepted connection to the worker

        var encoder = new PersonEncoder();
        var decoder = new PersonDecoder();
        var serverHandler = new PersonServerHandler();

        try
        {
            bootstrap = new ServerBootstrap();

            bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100) // maximum queue length for incoming connection
                .Handler(new SeriLoggingHandler(logLevel))
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;

                    pipeline.AddLast(encoder, decoder, serverHandler);
                }));

            bootstrapChannel = await bootstrap.BindAsync(serverPort);

            Console.WriteLine("Let us test the server in a command prompt");
            Console.WriteLine($"\n telnet localhost {serverPort}");
            Console.ReadLine();

           
        }
        finally
        {
            Task.WaitAll(bossGroup.ShutdownGracefullyAsync(), workerGroup.ShutdownGracefullyAsync());
        }
    }

    public override async Task Shutdown()
    {
         await bootstrapChannel.CloseAsync();
    }

    public override void SimulateLatency(int milliseconds)
    {
        throw new NotImplementedException();
    }
}