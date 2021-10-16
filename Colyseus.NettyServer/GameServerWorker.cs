using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coleseus.Shared.App;
using Coleseus.Shared.App.Impl;
using Coleseus.Shared.Protocols.Impl;
using Coleseus.Shared.Server;
using Coleseus.Shared.Service;
using Colyseus.Common;
using Colyseus.NettyServer.LostDecade;
using Colyseus.NettyServer.ZombieGame.Domain;
using Colyseus.NettyServer.ZombieGame.Game;
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

namespace Colyseus.NettyServer
{
    public class GammeServerWorker : BackgroundService
    {

        private readonly ServerManager _serverManager;
        private readonly Serilog.ILogger _logger;
        private readonly TaskManagerService _taskManagerService;
        private readonly ILookupService _loopupService;
        public GammeServerWorker(ServerManager serverManager, TaskManagerService taskManagerService, ILookupService loopupService)
        {
            _serverManager = serverManager;
            _logger = Serilog.Log.ForContext<GammeServerWorker>();
            _taskManagerService = taskManagerService;
            _loopupService = loopupService;
        }
        async Task RunServerAsync()
        {

            try
            {
                await _serverManager.startServers();
            }
            catch (Exception e)
            {
                _logger.Error("Unable to start servers cleanly: {}", e);
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

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            StartGames();
            _taskManagerService.Start();
            return base.StartAsync(cancellationToken);
        }

        IGame zombieGame()
        {
            IGame game = new SimpleGame(1, "Zombie");
            return game;
        }

        World world()
        {
            World world = new World();
            world.setAlive(2000000000);
            world.setUndead(1);
            return world;
        }


        Defender defender()
        {
            Defender defender = new Defender();
            defender.setWorld(world());
            return defender;
        }

        Zombie zombie()
        {
            Zombie zombie = new Zombie();
            zombie.setWorld(world());
            return zombie;
        }

        public void StartGames()
        {
            var protocol = new MessageBufferProtocol();
            protocol.setLengthFieldPrepender(new DotNetty.Codecs.LengthFieldPrepender(2, 2));
            protocol.setMessageBufferEventDecoder(new Coleseus.Shared.Handlers.Netty.MessageBufferEventDecoder());
            protocol.setMessageBufferEventEncoder(new Coleseus.Shared.Handlers.Netty.MessageBufferEventEncoder());
            World world = new World();
            world.setAlive(2000000000);
            world.setUndead(1);

            List<GameRoom> roomList = new List<GameRoom>(2);
            for (int i = 1; i <= 2; i++)
            {
                GameRoomSessionBuilder sessionBuilder = new GameRoomSessionBuilder();
                sessionBuilder.SetParentGame(zombieGame())
                        .SetGameRoomName("Zombie_ROOM_" + i)
                        .SetProtocol(protocol);
                ZombieRoom room = new ZombieRoom(sessionBuilder);
                room.setDefender(defender());
                room.setZombie(zombie());
                roomList.Add(room);
                ScheduleTask monitor1 = new WorldMonitor(world, room)
                {
                    InitialDelay = TimeSpan.FromMilliseconds(i * 1000),
                    TaskRunAtStart = true,
                    TaskTimeSpan = TimeSpan.FromMilliseconds(5000),
                };
                _taskManagerService.AddTask(monitor1);
            }

            Dictionary<String, GameRoom> refKeyGameRoomMap = new Dictionary<String, GameRoom>();
            List<GameRoom> zombieRooms = roomList;
            foreach (GameRoom room in zombieRooms)
            {
                refKeyGameRoomMap.Add(room.getGameRoomName(), room);
            }
            //refKeyGameRoomMap.Add("Zombie_ROOM_1_REF_KEY_2", zombieRoom2());
            //refKeyGameRoomMap.Add("LDGameRoom", ldGameRoom());
            //refKeyGameRoomMap.Add("LDGameRoomForNettyClient", ldGameRoomForNettyClient());
            {
                GameRoomSessionBuilder sessionBuilder = new GameRoomSessionBuilder();
                sessionBuilder.SetParentGame(new SimpleGame(2, "LDGame")).SetGameRoomName("LDGameRoom")
                        .SetProtocol(protocol);
                LDRoom ldroom = new LDRoom(sessionBuilder);
                refKeyGameRoomMap.Add("LDGameRoom", ldroom);
            }
        


            _loopupService.setGameRoomLookup(refKeyGameRoomMap);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RunServerAsync();
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _taskManagerService.Stop();
            _serverManager.stopServers();
            return base.StopAsync(cancellationToken);
        }
    }
}