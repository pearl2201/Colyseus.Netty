﻿using Coleseus.Shared.App;
using Coleseus.Shared.Event;
using Coleseus.Shared.Event.Impl;
using Coleseus.Shared.Service;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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

        private readonly ILogger<WebSocketLoginHandler> _logger;

        private LookupService lookupService;
        protected ReconnectSessionRegistry reconnectRegistry;
        protected UniqueIDGeneratorService idGeneratorService;

        private ObjectMapper jackson;


        protected override void ChannelRead0(IChannelHandlerContext ctx,
                TextWebSocketFrame frame)
        {
            IChannel channel = ctx.Channel;
            string data = frame.Text();
            _logger.LogTrace("From websocket: " + data);
            IEvent @event = JsonConvert.DeserializeObject<DefaultEvent>(data);
            int type = @event.getType();
            if (Events.LOG_IN == type)

            {
                _logger.LogTrace("Login attempt from {}", channel.RemoteAddress);
                List<String> credList = null;
                credList = (List<string>)@event.getSource();
                IPlayer player = lookupPlayer(credList.get(0), credList.get(1));
                handleLogin(player, channel);
                handleGameRoomJoin(player, channel, credList.get(2));
            }
            else if (type == Events.RECONNECT)

            {
                _logger.LogDebug("Reconnect attempt from {}", channel.RemoteAddress);
                IPlayerSession playerSession = lookupSession((string)@event.getSource());
                var task = handleReconnect(playerSession, channel);
                task.
            }

            else

            {
                _logger.LogError(
                        "Invalid event {} sent from remote address {}. "
                                + "Going to close channel {}",
                        new Object[] { @event.GetType(),
                            channel.RemoteAddress, channel
    });
                closeChannelWithLoginFailure(channel);
            }
        }

        public IPlayerSession lookupSession(string reconnectKey)
        {
            IPlayerSession playerSession = (IPlayerSession)reconnectRegistry.getSession(reconnectKey);
            if (null != playerSession)
            {
                synchronized(playerSession){
                    // if its an already active session then do not allow a
                    // reconnect. So the only state in which a client is allowed to
                    // reconnect is if it is "NOT_CONNECTED"
                    if (playerSession.status == SessionStatus.NOT_CONNECTED)
                    {
                        playerSession.status = SessionStatus.CONNECTING;
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
                if (null != playerSession.getTcpSender())
                    playerSession.getTcpSender().close();

                handleReJoin(playerSession, gameRoom, channel);
            }
            else

            {
                // Write future and close channel
                closeChannelWithLoginFailure(channel);
            }
        }

        protected async Task handleReJoin(IPlayerSession playerSession, GameRoom gameRoom, IChannel channel)
        {
            // Set the tcp channel on the session. 
            NettyTCPMessageSender sender = new NettyTCPMessageSender(channel);
            playerSession.setTcpSender(sender);
            // Connect the pipeline to the game room.
            gameRoom.connectSession(playerSession);
            await channel.WriteAndFlushAsync(Events.GAME_ROOM_JOIN_SUCCESS, null);//assumes that the protocol applied will take care of event objects.
            playerSession.isWriteable = true;// TODO remove if unnecessary. It should be done in start event
                                             // Send the re-connect event so that it will in turn send the START event.
            playerSession.onEvent(new ReconnetEvent(sender));
        }

        public IPlayer lookupPlayer(string username, string password)
        {
            Credentials credentials = new SimpleCredentials(username, password);
            IPlayer player = lookupService.playerLookup(credentials);
            if (null == player)
            {
                _logger.LogError("Invalid credentials provided by user: {}", credentials);
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
                closeChannelWithLoginFailure(channel);
            }
        }

        protected async Task closeChannelWithLoginFailure(IChannel channel)
        {
            // Close the connection as soon as the error message is sent.
            await channel.WriteAndFlushAsync(eventToFrame(Events.LOG_IN_FAILURE, null)).addListener(
                        ChannelFutureListener.CLOSE);
        }

        public void handleGameRoomJoin(IPlayer player, IChannel channel, string refKey)
        {
            GameRoom gameRoom = lookupService.gameRoomLookup(refKey);
            if (null != gameRoom)

            {
                IPlayerSession playerSession = gameRoom.createPlayerSession(player);
                String reconnectKey = (String)idGeneratorService
                        .generateFor(playerSession.getClass());
                playerSession.setAttribute(NadronConfig.RECONNECT_KEY, reconnectKey);
                playerSession.setAttribute(NadronConfig.RECONNECT_REGISTRY, reconnectRegistry);
                _logger.LogTrace("Sending GAME_ROOM_JOIN_SUCCESS to channel {}",
                        channel);
                var task = channel.WriteAndFlushAsync(eventToFrame(
                        Events.GAME_ROOM_JOIN_SUCCESS, reconnectKey));
                connectToGameRoom(gameRoom, playerSession, task);
            }
            else

            {
                // Write failure and close channel.
                ChannelFuture future = channel.writeAndFlush(eventToFrame(
                        Events.GAME_ROOM_JOIN_FAILURE, null));
                future.addListener(ChannelFutureListener.CLOSE);
                _logger.LogError(
                        "Invalid ref key provided by client: {}. Channel {} will be closed",
                        refKey, channel);
            }
        }

        public void connectToGameRoom(GameRoom gameRoom,
                 IPlayerSession playerSession, Task future)
        {
            
            future.addListener(new ChannelFutureListener()
            {



            public void operationComplete(ChannelFuture future)





            {
                Channel channel = future.channel();
                _logger.LogTrace(
                        "Sending GAME_ROOM_JOIN_SUCCESS to channel {} completed",
                        channel);
                if (future.isSuccess())
                {
                    // Set the tcp channel on the session.
                    NettyTCPMessageSender tcpSender = new NettyTCPMessageSender(
                            channel);
                    playerSession.setTcpSender(tcpSender);
                    // Connect the pipeline to the game room.
                    gameRoom.connectSession(playerSession);
                    // send the start event to remote client.
                    tcpSender.sendMessage(Events.CreateEvent(null, Events.START));
                    gameRoom.onLogin(playerSession);
                }
                else
                {
                    _logger.LogError("Sending GAME_ROOM_JOIN_SUCCESS message to client was failure, channel will be closed");
                    channel.close();
                }
            }
        });
        }

    protected TextWebSocketFrame eventToFrame(byte opcode, Object payload)
    {
        IEvent @event = Events.CreateEvent(payload, opcode);
        return new TextWebSocketFrame(JsonConvert.SerializeObject(@event));
    }

    public LookupService getLookupService()
    {
        return lookupService;
    }

    public void setLookupService(LookupService lookupService)
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

    public ObjectMapper getJackson()
    {
        return jackson;
    }

    public void setJackson(ObjectMapper jackson)
    {
        this.jackson = jackson;
    }
}
}