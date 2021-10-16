using AutoMapper;
using Coleseus.Shared.App;
using Coleseus.Shared.Communication;
using Coleseus.Shared.Event;
using Coleseus.Shared.Event.Impl;
using Coleseus.Shared.Service;
using Coleseus.Shared.Service.Impl;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coleseus.Shared.Handlers.Netty
{
    /**
   * This login handler will parse incoming login events to get the
   * {@link Credentials} and lookup {@link Player} and {@link GameRoom} objects.
   * It kicks of the session creation process and will then send the
   * {@link Events#START} event object to websocket client.
   * 
   * @author Abraham Menacherry
   * 
   */

    public class WebSocketLoginHandler : SimpleChannelInboundHandler<TextWebSocketFrame>
    {

        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<WebSocketLoginHandler>();

        private ILookupService lookupService;
        protected ReconnectSessionRegistry reconnectRegistry;
        protected UniqueIDGeneratorService idGeneratorService;

        private IMapper jackson;


        protected override void ChannelRead0(IChannelHandlerContext ctx,
                TextWebSocketFrame frame)
        {
            IChannel channel = ctx.Channel;
            string data = frame.Text();
            _logger.Verbose("From websocket: " + data);
            IEvent @event = JsonConvert.DeserializeObject<DefaultEvent>(data);
            int type = @event.getType();
            if (Events.LOG_IN == type)

            {
                _logger.Verbose("Login attempt from {}", channel.RemoteAddress);
                List<String> credList = null;
                credList = (List<string>)@event.getSource();
                IPlayer player = lookupPlayer(credList[0], credList[1]);
                handleLogin(player, channel).Wait();
                handleGameRoomJoin(player, channel, credList[2]);
            }
            else if (type == Events.RECONNECT)

            {
                _logger.Debug("Reconnect attempt from {}", channel.RemoteAddress);
                IPlayerSession playerSession = lookupSession((string)@event.getSource());
                var task = handleReconnect(playerSession, channel);

            }

            else

            {
                _logger.Error(
                        "Invalid event {} sent from remote address {}. "
                                + "Going to close channel {}",
                        new Object[] { @event.GetType(),
                            channel.RemoteAddress, channel
    });
                closeChannelWithLoginFailure(channel).Wait();
            }
        }

        public IPlayerSession lookupSession(string reconnectKey)
        {
            IPlayerSession playerSession = (IPlayerSession)reconnectRegistry.getSession(reconnectKey);
            if (null != playerSession)
            {
                lock (playerSession)
                {
                    // if its an already active session then do not allow a
                    // reconnect. So the only state in which a client is allowed to
                    // reconnect is if it is "NOT_CONNECTED"
                    if (playerSession.Status == SessionStatus.NOT_CONNECTED)
                    {
                        playerSession.Status = SessionStatus.CONNECTING;
                    }
                    else
                    {
                        playerSession = null;
                    }
                }
            }
            return playerSession;
        }

        protected async Task handleReconnect(IPlayerSession playerSession, IChannel channel)
        {
            if (null != playerSession)

            {
                await channel.WriteAndFlushAsync(eventToFrame(Events.LOG_IN_SUCCESS, null));
                GameRoom gameRoom = playerSession.getGameRoom();
                gameRoom.disconnectSession(playerSession);
                if (null != playerSession.TcpSender)
                    playerSession.TcpSender.close();

                handleReJoin(playerSession, gameRoom, channel).Wait();
            }
            else

            {
                // Write future and close channel
                closeChannelWithLoginFailure(channel).Wait();
            }
        }

        protected async Task handleReJoin(IPlayerSession playerSession, GameRoom gameRoom, IChannel channel)
        {
            // Set the tcp channel on the session. 
            NettyTCPMessageSender sender = new NettyTCPMessageSender(channel);
            playerSession.TcpSender = sender;
            // Connect the pipeline to the game room.
            gameRoom.connectSession(playerSession);
            await channel.WriteAndFlushAsync(Events.GAME_ROOM_JOIN_SUCCESS);//assumes that the protocol applied will take care of event objects.
            playerSession.IsWriteable = true;// TODO remove if unnecessary. It should be done in start event
                                             // Send the re-connect event so that it will in turn send the START event.
            playerSession.OnEvent(new ReconnetEvent(sender));
        }

        public IPlayer lookupPlayer(string username, string password)
        {
            Credentials credentials = new SimpleCredentials(username, password);
            IPlayer player = lookupService.playerLookup(credentials);
            if (null == player)
            {
                _logger.Error("Invalid credentials provided by user: {}", credentials);
            }
            return player;
        }

        public async Task handleLogin(IPlayer player, IChannel channel)
        {
            if (null != player)

            {
                await channel.WriteAndFlushAsync(eventToFrame(Events.LOG_IN_SUCCESS, null));
            }
            else

            {
                // Write future and close channel
                closeChannelWithLoginFailure(channel).Wait();
            }
        }

        protected async Task closeChannelWithLoginFailure(IChannel channel)
        {
            // Close the connection as soon as the error message is sent.
            await channel.WriteAndFlushAsync(eventToFrame(Events.LOG_IN_FAILURE, null));
        }

        public void handleGameRoomJoin(IPlayer player, IChannel channel, string refKey)
        {
            GameRoom gameRoom = lookupService.gameRoomLookup(refKey);
            if (null != gameRoom)

            {
                IPlayerSession playerSession = gameRoom.createPlayerSession(player);
                String reconnectKey = (String)idGeneratorService
                        .generateFor(playerSession.GetType());
                playerSession.SetAttribute(ColyseusConfig.RECONNECT_KEY, reconnectKey);
                playerSession.SetAttribute(ColyseusConfig.RECONNECT_REGISTRY, reconnectRegistry);
                _logger.Verbose("Sending GAME_ROOM_JOIN_SUCCESS to channel {}",
                        channel);
                var task = channel.WriteAndFlushAsync(eventToFrame(
                        Events.GAME_ROOM_JOIN_SUCCESS, reconnectKey));
                connectToGameRoom(gameRoom, playerSession, task, channel);
            }
            else

            {
                // Write failure and close channel.
                channel.WriteAndFlushAsync(eventToFrame(
                        Events.GAME_ROOM_JOIN_FAILURE, null)).Wait();
                _logger.Error(
                        "Invalid ref key provided by client: {}. Channel {} will be closed",
                        refKey, channel);
            }
        }

        public void connectToGameRoom(GameRoom gameRoom,
                 IPlayerSession playerSession, Task future, IChannel channel)
        {
            future.ContinueWith((x) =>
            {

                _logger.Verbose(
                        "Sending GAME_ROOM_JOIN_SUCCESS to channel {} completed",
                        channel);
                if (x.Status == TaskStatus.RanToCompletion)
                {
                    // Set the tcp channel on the session.
                    NettyTCPMessageSender tcpSender = new NettyTCPMessageSender(
                            channel);
                    playerSession.TcpSender = tcpSender;
                    // Connect the pipeline to the game room.
                    gameRoom.connectSession(playerSession);
                    // send the start event to remote client.
                    tcpSender.sendMessage(Events.CreateEvent(null, Events.START));
                    gameRoom.onLogin(playerSession);
                }
                else
                {
                    _logger.Error("Sending GAME_ROOM_JOIN_SUCCESS message to client was failure, channel will be closed");
                    channel.CloseAsync().Wait();
                }
            });


        }

        protected TextWebSocketFrame eventToFrame(byte opcode, Object payload)
        {
            IEvent @event = Events.CreateEvent(payload, opcode);
            return new TextWebSocketFrame(JsonConvert.SerializeObject(@event));
        }

        public ILookupService getLookupService()
        {
            return lookupService;
        }

        public void setLookupService(ILookupService lookupService)
        {
            this.lookupService = lookupService;
        }

        public ReconnectSessionRegistry getReconnectRegistry()
        {
            return reconnectRegistry;
        }

        public void setReconnectRegistry(ReconnectSessionRegistry reconnectRegistry)
        {
            this.reconnectRegistry = reconnectRegistry;
        }

        public UniqueIDGeneratorService getIdGeneratorService()
        {
            return idGeneratorService;
        }

        public void setIdGeneratorService(UniqueIDGeneratorService idGeneratorService)
        {
            this.idGeneratorService = idGeneratorService;
        }

        public IMapper getJackson()
        {
            return jackson;
        }

        public void setJackson(IMapper jackson)
        {
            this.jackson = jackson;
        }
    }
}
