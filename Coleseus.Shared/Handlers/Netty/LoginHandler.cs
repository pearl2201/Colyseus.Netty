using Coleseus.Shared.App;
using Coleseus.Shared.Event;
using Coleseus.Shared.Server.Netty;
using Coleseus.Shared.Service;
using Coleseus.Shared.Service.Impl;
using Coleseus.Shared.Util;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Coleseus.Shared.Handlers.Netty
{

    public class LoginHandler : SimpleChannelInboundHandler<IEvent>
    {

        private readonly ILogger<LoginHandler> _logger;

        protected ILookupService lookupService;
        protected ISessionRegistryService<SocketAddress> udpSessionRegistry;
        protected ReconnectSessionRegistry reconnectRegistry;
        protected UniqueIDGeneratorService idGeneratorService;

        /**
         * Used for book keeping purpose. It will count all open channels. Currently
         * closed channels will not lead to a decrement.
         */
        private AtomicInteger CHANNEL_COUNTER = new AtomicInteger(0);


        public void channelRead0(IChannelHandlerContext ctx,
                IEvent @event)
        {
            IByteBuffer buffer = (IByteBuffer)@event.getSource();
            IChannel channel = ctx.Channel;
            int type = @event.getType();
            if (Events.LOG_IN == type)

            {
                _logger.LogDebug("Login attempt from {}", channel.RemoteAddress);
                IPlayer player = lookupPlayer(buffer, channel);
                handleLogin(player, ctx, buffer);
            }
            else if (Events.RECONNECT == type)

            {
                _logger.LogDebug("Reconnect attempt from {}", channel.RemoteAddress);
                String reconnectKey = NettyUtils.readString(buffer);
                IPlayerSession playerSession = lookupSession(reconnectKey);
                handleReconnect(playerSession, ctx, buffer);
            }

            else

            {
                _logger.LogError("Invalid @event {} sent from remote address {}. "
                        + "Going to close channel {}",
                        new Object[] { @event.getType(), channel.RemoteAddress,
                            channel});
                closeChannelWithLoginFailure(channel);
            }
        }


        public async Task exceptionCaught(IChannelHandlerContext ctx, Throwable cause)


        {
            IChannel channel = ctx.Channel;
            _logger.LogError(
                        "Exception {} occurred during log in process, going to close channel {}",
                        cause, channel);
            await channel.CloseAsync();
        }



        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            base.ChannelActive(ctx);
            AbstractNettyServer.ALL_CHANNELS.add(ctx.Channel);
            _logger.LogDebug("Added Channel {} as the {}th open channel", ctx
                    .Channel, CHANNEL_COUNTER.incrementAndGet());
        }



        public IPlayer lookupPlayer(IByteBuffer buffer, IChannel channel)
        {
            Credentials credentials = new SimpleCredentials(buffer);
            IPlayer player = lookupService.playerLookup(credentials);
            if (null == player)
            {
                _logger.LogError("Invalid credentials provided by user: {}", credentials);
            }
            return player;
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

        public void handleLogin(IPlayer player, IChannelHandlerContext ctx, IByteBuffer buffer)
        {
            if (null != player)
            {
                ctx.Channel.WriteAsync(NettyUtils
                        .createBufferForOpcode(Events.LOG_IN_SUCCESS)).Wait();
                handleGameRoomJoin(player, ctx, buffer);
            }
            else
            {
                // Write future and close channel
                closeChannelWithLoginFailure(ctx.Channel);
            }
        }

        protected void handleReconnect(IPlayerSession playerSession, IChannelHandlerContext ctx, IByteBuffer buffer)
        {
            if (null != playerSession)
            {
                ctx.write(NettyUtils
                        .createBufferForOpcode(Events.LOG_IN_SUCCESS));
                GameRoom gameRoom = playerSession.getGameRoom();
                gameRoom.disconnectSession(playerSession);
                if (null != playerSession.getTcpSender())
                    playerSession.getTcpSender().close();

                if (null != playerSession.getUdpSender())
                    playerSession.getUdpSender().close();

                handleReJoin(playerSession, gameRoom, ctx.Channel, buffer);
            }
            else
            {
                // Write future and close channel
                closeChannelWithLoginFailure(ctx.Channel);
            }
        }

        /**
         * Helper method which will close the channel after writing
         * {@link Events#LOG_IN_FAILURE} to remote connection.
         * 
         * @param channel
         *            The tcp connection to remote machine that will be closed.
         */
        private async Task closeChannelWithLoginFailure(IChannel channel)
        {
            await channel.WriteAndFlushAsync(NettyUtils
                      .createBufferForOpcode(Events.LOG_IN_FAILURE));

        }

        public void handleGameRoomJoin(IPlayer player, IChannelHandlerContext ctx, IByteBuffer buffer)
        {
            String refKey = NettyUtils.readString(buffer);
            IChannel channel = ctx.Channel;
            GameRoom gameRoom = lookupService.gameRoomLookup(refKey);
            if (null != gameRoom)
            {
                IPlayerSession playerSession = gameRoom.createPlayerSession(player);
                String reconnectKey = (String)idGeneratorService
                        .generateFor(playerSession.GetType());
                playerSession.setAttribute(NadronConfig.RECONNECT_KEY, reconnectKey);
                playerSession.setAttribute(NadronConfig.RECONNECT_REGISTRY, reconnectRegistry);
                _logger.Debug("Sending GAME_ROOM_JOIN_SUCCESS to channel {}", channel);
                IByteBuffer reconnectKeyBuffer = Unpooled.wrappedBuffer(NettyUtils.createBufferForOpcode(Events.GAME_ROOM_JOIN_SUCCESS),
                                NettyUtils.writeString(reconnectKey));
                ChannelFuture future = channel.writeAndFlush(reconnectKeyBuffer);
                connectToGameRoom(gameRoom, playerSession, future);
                loginUdp(playerSession, buffer);
            }
            else
            {
                // Write failure and close channel.
                ChannelFuture future = channel.writeAndFlush(NettyUtils.createBufferForOpcode(Events.GAME_ROOM_JOIN_FAILURE));
                future.addListener(ChannelFutureListener.CLOSE);
                LOG.error("Invalid ref key provided by client: {}. Channel {} will be closed", refKey, channel);
            }
        }

        protected void handleReJoin(PlayerSession playerSession, GameRoom gameRoom, Channel channel,
                IByteBuffer buffer)
        {
            LOG.trace("Going to clear pipeline");
            // Clear the existing pipeline
            NettyUtils.clearPipeline(channel.pipeline());
            // Set the tcp channel on the session. 
            NettyTCPMessageSender sender = new NettyTCPMessageSender(channel);
            playerSession.setTcpSender(sender);
            // Connect the pipeline to the game room.
            gameRoom.connectSession(playerSession);
            playerSession.setWriteable(true);// TODO remove if unnecessary. It should be done in start @event
                                             // Send the re-connect @event so that it will in turn send the START @event.
            playerSession.onEvent(new ReconnetEvent(sender));
            loginUdp(playerSession, buffer);
        }

        public void connectToGameRoom(final GameRoom gameRoom, final PlayerSession playerSession, ChannelFuture future)
        {
            future.addListener(new ChannelFutureListener()
        {
            @Override

            public void operationComplete(ChannelFuture future)



                    throws Exception



            {
                Channel channel = future.channel();
                LOG.trace("Sending GAME_ROOM_JOIN_SUCCESS to channel {} completed", channel);
                if (future.isSuccess())
                {
                    LOG.trace("Going to clear pipeline");
                    // Clear the existing pipeline
                    NettyUtils.clearPipeline(channel.pipeline());
                    // Set the tcp channel on the session. 
                    NettyTCPMessageSender tcpSender = new NettyTCPMessageSender(channel);
                    playerSession.setTcpSender(tcpSender);
                    // Connect the pipeline to the game room.
                    gameRoom.connectSession(playerSession);
                    // send the start @event to remote client.
                    tcpSender.sendMessage(Events.@event(null, Events.START));
                    gameRoom.onLogin(playerSession);
                }
                else
                {
                    LOG.error("GAME_ROOM_JOIN_SUCCESS message sending to client was failure, channel will be closed");
                    channel.close();
                }
            }
        });
	}

    /**
     * This method adds the player session to the
     * {@link SessionRegistryService}. The key being the remote udp address of
     * the client and the session being the value.
     * 
     * @param playerSession
     * @param buffer
     *            Used to read the remote address of the client which is
     *            attempting to connect via udp.
     */
    protected void loginUdp(PlayerSession playerSession, IByteBuffer buffer)
    {
        InetSocketAddress remoteAddress = NettyUtils.readSocketAddress(buffer);
        if (null != remoteAddress)
        {
            udpSessionRegistry.putSession(remoteAddress, playerSession);
        }
    }

    public LookupService getLookupService()
    {
        return lookupService;
    }

    public void setLookupService(LookupService lookupService)
    {
        this.lookupService = lookupService;
    }

    public UniqueIDGeneratorService getIdGeneratorService()
    {
        return idGeneratorService;
    }

    public void setIdGeneratorService(UniqueIDGeneratorService idGeneratorService)
    {
        this.idGeneratorService = idGeneratorService;
    }

    public SessionRegistryService<SocketAddress> getUdpSessionRegistry()
    {
        return udpSessionRegistry;
    }

    public void setUdpSessionRegistry(
            SessionRegistryService<SocketAddress> udpSessionRegistry)
    {
        this.udpSessionRegistry = udpSessionRegistry;
    }

    public ReconnectSessionRegistry getReconnectRegistry()
    {
        return reconnectRegistry;
    }

    public void setReconnectRegistry(ReconnectSessionRegistry reconnectRegistry)
    {
        this.reconnectRegistry = reconnectRegistry;
    }

}

}
