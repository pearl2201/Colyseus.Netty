using Coleseus.Shared.App;
using Coleseus.Shared.Communication;
using Coleseus.Shared.Event;
using Coleseus.Shared.Event.Impl;
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
using System.Threading;
using System.Threading.Tasks;

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
        private int CHANNEL_COUNTER = 0;


        protected override void ChannelRead0(IChannelHandlerContext ctx,
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
                closeChannelWithLoginFailure(channel).Wait();
            }
        }


        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)

        {
            IChannel channel = context.Channel;
            _logger.LogError(
                        "Exception {} occurred during log in process, going to close channel {}",
                        exception, channel);
            channel.CloseAsync().Wait();
        }



        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            base.ChannelActive(ctx);
            AbstractNettyServer.ALL_CHANNELS.Add(ctx.Channel);
            _logger.LogDebug("Added Channel {} as the {}th open channel", ctx
                    .Channel, Interlocked.Increment(ref CHANNEL_COUNTER));
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
                lock (playerSession)
                {
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
                closeChannelWithLoginFailure(ctx.Channel).Wait();
            }
        }

        protected void handleReconnect(IPlayerSession playerSession, IChannelHandlerContext ctx, IByteBuffer buffer)
        {
            if (null != playerSession)
            {
                ctx.WriteAndFlushAsync(NettyUtils
                        .createBufferForOpcode(Events.LOG_IN_SUCCESS)).Wait();
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
                closeChannelWithLoginFailure(ctx.Channel).Wait();
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
                playerSession.setAttribute(ColyseusConfig.RECONNECT_KEY, reconnectKey);
                playerSession.setAttribute(ColyseusConfig.RECONNECT_REGISTRY, reconnectRegistry);
                _logger.LogDebug("Sending GAME_ROOM_JOIN_SUCCESS to channel {}", channel);
                IByteBuffer reconnectKeyBuffer = Unpooled.WrappedBuffer(NettyUtils.createBufferForOpcode(Events.GAME_ROOM_JOIN_SUCCESS),
                                NettyUtils.WriteString(reconnectKey));
                Task future = channel.WriteAndFlushAsync(reconnectKeyBuffer);
                connectToGameRoom(gameRoom, playerSession, future, channel);
                loginUdp(playerSession, buffer);
            }
            else
            {
                // Write failure and close channel.
                channel.WriteAndFlushAsync(NettyUtils.createBufferForOpcode(Events.GAME_ROOM_JOIN_FAILURE));

                _logger.LogError("Invalid ref key provided by client: {}. Channel {} will be closed", refKey, channel);
            }
        }

        protected void handleReJoin(IPlayerSession playerSession, GameRoom gameRoom, IChannel channel,
                IByteBuffer buffer)
        {
            _logger.LogTrace("Going to clear pipeline");
            // Clear the existing pipeline
            NettyUtils.clearPipeline(channel.Pipeline);
            // Set the tcp channel on the session. 
            NettyTCPMessageSender sender = new NettyTCPMessageSender(channel);
            playerSession.setTcpSender(sender);
            // Connect the pipeline to the game room.
            gameRoom.connectSession(playerSession);
            playerSession.isWriteable = true;// TODO remove if unnecessary. It should be done in start @event
                                             // Send the re-connect @event so that it will in turn send the START @event.
            playerSession.onEvent(new ReconnetEvent(sender));
            loginUdp(playerSession, buffer);
        }

        public void connectToGameRoom(GameRoom gameRoom, IPlayerSession playerSession, Task future, IChannel channel)
        {
            future.ContinueWith((x) =>
            {
                //Channel channel = future.channel();
                _logger.LogTrace("Sending GAME_ROOM_JOIN_SUCCESS to channel {} completed", channel);
                if (future.Status == TaskStatus.RanToCompletion)
                {
                    _logger.LogTrace("Going to clear pipeline");
                    // Clear the existing pipeline
                    NettyUtils.clearPipeline(channel.Pipeline);
                    // Set the tcp channel on the session. 
                    NettyTCPMessageSender tcpSender = new NettyTCPMessageSender(channel);
                    playerSession.setTcpSender(tcpSender);
                    // Connect the pipeline to the game room.
                    gameRoom.connectSession(playerSession);
                    // send the start @event to remote client.
                    tcpSender.sendMessage(Events.CreateEvent(null, Events.START));
                    gameRoom.onLogin(playerSession);
                }
                else
                {
                    _logger.LogError("GAME_ROOM_JOIN_SUCCESS message sending to client was failure, channel will be closed");
                    channel.CloseAsync().Wait();
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
        protected void loginUdp(IPlayerSession playerSession, IByteBuffer buffer)
        {
            IPEndPoint remoteAddress = NettyUtils.readSocketAddress(buffer);
            if (null != remoteAddress)
            {
                udpSessionRegistry.putSession(remoteAddress.Serialize(), playerSession);
            }
        }

        public ILookupService getLookupService()
        {
            return lookupService;
        }

        public void setLookupService(ILookupService lookupService)
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

        public ISessionRegistryService<SocketAddress> getUdpSessionRegistry()
        {
            return udpSessionRegistry;
        }

        public void setUdpSessionRegistry(
                ISessionRegistryService<SocketAddress> udpSessionRegistry)
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
