using Coleseus.Shared.Handlers.Netty;
using Coleseus.Shared.Server;
using Coleseus.Shared.Server.Netty;
using Coleseus.Shared.Service;
using Coleseus.Shared.Service.Impl;
using DotNetty.Codecs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Samples.Akka.AspNetCore.Actors;
using Samples.Akka.AspNetCore.Services;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Colyseus.NettyServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()

                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<ILookupService, SimpleLookupService>();
                    services.AddSingleton<ISessionRegistryService<EndPoint>, SessionRegistry<EndPoint>>();
                    services.AddSingleton<NettyConfig>();
                    services.AddSingleton<SimpleUniqueIdGenerator>();
                    services.AddSingleton<ReconnectSessionRegistry>();

                    services.AddSingleton<ProtocolMultiplexerChannelInitializer>(sp =>
                    {
                        Console.WriteLine("Construct a program");
                        var lookupService = sp.GetRequiredService<ILookupService>();
                        var taskManagerService = sp.GetRequiredService<TaskManagerService>();
                        var lengthFieldPrepender = new LengthFieldPrepender(2, false);
                        var udpSessionRegistry = new SessionRegistry<SocketAddress>();
                        var idGeneratorService = sp.GetRequiredService<SimpleUniqueIdGenerator>();
                        var reconnectSessionRegistry = sp.GetRequiredService<ReconnectSessionRegistry>();


                        var eventDecoder = new EventDecoder();
                        var loginHandler = new LoginHandler();
                        loginHandler.setIdGeneratorService(idGeneratorService);
                        loginHandler.setLookupService(lookupService);
                        loginHandler.setReconnectRegistry(reconnectSessionRegistry);
                        loginHandler.setUdpSessionRegistry(udpSessionRegistry);

                        var websocketLoginHandler = new WebSocketLoginHandler();
                        websocketLoginHandler.setIdGeneratorService(idGeneratorService);
                        websocketLoginHandler.setLookupService(lookupService);
                        websocketLoginHandler.setReconnectRegistry(reconnectSessionRegistry);
                        CompositeProtocol loginProtocol = new CompositeProtocol();
                        List<LoginProtocol> loginProtocols = new List<LoginProtocol>()
                        {
                            new DefaultNadProtocol(eventDecoder, loginHandler,lengthFieldPrepender),
                            new HTTPProtocol(websocketLoginHandler)
                        };
                        loginProtocol.setProtocols(loginProtocols);
                        return new ProtocolMultiplexerChannelInitializer(5, loginProtocol);
                    });
                    services.AddSingleton<MessageBufferEventDecoder>();
                    services.AddSingleton<UDPEventEncoder>();
                    services.AddSingleton<UDPUpstreamHandler>();
                    services.AddSingleton<UDPChannelInitializer>();
                    services.AddSingleton<NettyTCPServer>();
                    services.AddSingleton<NettyUDPServer>();
                    services.AddSingleton<ServerManager, ServerManagerImpl>();
                    services.AddScoped<IHashService, HashServiceImpl>();
                    services.AddSingleton<TaskManagerService, SimpleTaskManagerService>();
                    services.AddSingleton<IPublicHashingService, AkkaService>();
                    services.AddHostedService<AkkaService>(sp => (AkkaService)sp.GetRequiredService<IPublicHashingService>());

                    //services.AddQuartz(q =>
                    //{
                    //    // your configuration here
                    //});

                    //// Quartz.Extensions.Hosting hosting
                    //services.AddQuartzHostedService(options =>
                    //{
                    //    // when shutting down we want jobs to complete gracefully
                    //    options.WaitForJobsToComplete = true;
                    //});

                    services.AddHostedService<Worker>();
                    services.AddHostedService<GammeServerWorker>();
                });
    }
}
