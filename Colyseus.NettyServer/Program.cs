using Coleseus.Shared.Server;
using Coleseus.Shared.Server.Netty;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Samples.Akka.AspNetCore.Actors;
using Samples.Akka.AspNetCore.Services;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    services.AddHostedService<Worker>();
                    services.AddHostedService<GammeServerWorker>();
                    services.AddSingleton<NettyTCPServer>();
                    services.AddSingleton<NettyUDPServer>();
                    services.AddSingleton<ServerManager,ServerManagerImpl>();
                    // set up a simple service we're going to hash
                    services.AddScoped<IHashService, HashServiceImpl>();

                    // creates instance of IPublicHashingService that can be accessed by ASP.NET
                    services.AddSingleton<IPublicHashingService, AkkaService>();

                    // starts the IHostedService, which creates the ActorSystem and actors
                    services.AddHostedService<AkkaService>(sp => (AkkaService)sp.GetRequiredService<IPublicHashingService>());
                });
    }
}
