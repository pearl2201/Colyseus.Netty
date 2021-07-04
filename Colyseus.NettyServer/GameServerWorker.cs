using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coleseus.Shared.Server;
using Colyseus.Common;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Serilog.Events;
using LogLevel = DotNetty.Handlers.Logging.LogLevel;

namespace Colyseus.NettyServer {
    public class GammeServerWorker : BackgroundService {

        private readonly ServerManager _serverManager;
        private readonly Serilog.ILogger _logger;
        public GammeServerWorker (ServerManager serverManager, ILogger<GammeServerWorker> logger) {
            _serverManager = serverManager;
            _logger = Serilog.Log.ForContext<GammeServerWorker> ();
        }
        async Task RunServerAsync () {

            try {
                await _serverManager.startServers ();
            } catch (Exception e) {
                _logger.Error ("Unable to start servers cleanly: {}", e);
            }

            //var logLevel = LogEventLevel.Information;

            //var serverPort = 8080;

            //var bossGroup = new MultithreadEventLoopGroup(1); //  accepts an incoming connection
            //var workerGroup = new MultithreadEventLoopGroup(); // handles the traffic of the accepted connection once the boss accepts the connection and registers the accepted connection to the worker

            //var encoder = new PersonEncoder();
            //var decoder = new PersonDecoder();
            //var serverHandler = new PersonServerHandler();

            //try
            //{
            //    var bootstrap = new ServerBootstrap();

            //    bootstrap
            //        .Group(bossGroup, workerGroup)
            //        .Channel<TcpServerSocketChannel>()
            //        .Option(ChannelOption.SoBacklog, 100) // maximum queue length for incoming connection
            //        .Handler(new SeriLoggingHandler(logLevel))
            //        .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
            //        {
            //            IChannelPipeline pipeline = channel.Pipeline;

            //            pipeline.AddLast(encoder, decoder, serverHandler);
            //        }));

            //    IChannel bootstrapChannel = await bootstrap.BindAsync(serverPort);

            //    Console.WriteLine("Let us test the server in a command prompt");
            //    Console.WriteLine($"\n telnet localhost {serverPort}");
            //    Console.ReadLine();

            //    await bootstrapChannel.CloseAsync();
            //}
            //finally
            //{
            //    Task.WaitAll(bossGroup.ShutdownGracefullyAsync(), workerGroup.ShutdownGracefullyAsync());
            //}
        }

        protected override async Task ExecuteAsync (CancellationToken stoppingToken) {
            await RunServerAsync ();
        }
    }
}