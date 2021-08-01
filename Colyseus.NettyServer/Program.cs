using Coleseus.Shared.Handlers.Netty;
using Coleseus.Shared.Server;
using Coleseus.Shared.Server.Netty;
using Coleseus.Shared.Service;
using Coleseus.Shared.Service.Impl;
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
                    services.AddSingleton<ProtocolMultiplexerChannelInitializer>();
                    services.AddSingleton<MessageBufferEventDecoder>();
                    services.AddSingleton<UDPEventEncoder>();
                    services.AddSingleton<UDPUpstreamHandler>();
                    services.AddSingleton<UDPChannelInitializer>();
                    services.AddSingleton<NettyTCPServer>();
                    services.AddSingleton<NettyUDPServer>();
                    services.AddSingleton<ServerManager,ServerManagerImpl>();
                    services.AddScoped<IHashService, HashServiceImpl>();
                    services.AddSingleton<TaskManagerService, SimpleTaskManagerService>();
                    services.AddSingleton<IPublicHashingService, AkkaService>();
                    services.AddHostedService<AkkaService>(sp => (AkkaService)sp.GetRequiredService<IPublicHashingService>());

                    services.AddQuartz(q =>
                    {
                        // your configuration here
                    });

                    // Quartz.Extensions.Hosting hosting
                    services.AddQuartzHostedService(options =>
                    {
                        // when shutting down we want jobs to complete gracefully
                        options.WaitForJobsToComplete = true;
                    });

                    services.AddHostedService<Worker>();
                    services.AddHostedService<GammeServerWorker>();
                });
    }
}
