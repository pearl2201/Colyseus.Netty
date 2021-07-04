using Coleseus.Shared.App;
using Coleseus.Shared.Event;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Handlers.Netty
{
    public class DefaultToServerHandler : SimpleChannelInboundHandler<IEvent>
    {
        private IPlayerSession playerSession;
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<DefaultToServerHandler>();

        public DefaultToServerHandler(IPlayerSession playerSession) :base()
        {
    
            this.playerSession = playerSession;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IEvent msg)
        {
            playerSession.onEvent(msg);
        }


        public override void ExceptionCaught(IChannelHandlerContext ctx, Exception exception)
        {
            _logger.Error("Exception during network communication: {}.", exception);
            IEvent @event = Events.CreateEvent(exception, Events.EXCEPTION);
            playerSession.onEvent(@event);
        }


        public override void ChannelInactive(IChannelHandlerContext ctx)
        {
            _logger.Debug("Netty Channel {} is closed.", ctx.Channel);
            if (!playerSession.isShuttingDown)
            {
                // Should not send close to session, since reconnection/other
                // business logic might be in place.
                IEvent @event = Events.CreateEvent(null, Events.DISCONNECT);
                playerSession.onEvent(@event);
            }
        }


        public override void UserEventTriggered(IChannelHandlerContext ctx, Object evt)
        {
            if (evt is IdleStateEvent)
            {
                _logger.Warning(
                        "Channel {} has been idle, exception event will be raised now: ",
                        ctx.Channel);
                // TODO check if setting payload as non-throwable cause issue?
                IEvent @event = Events.CreateEvent(evt, Events.EXCEPTION);
                playerSession.onEvent(@event);
            }
        }

        public IPlayerSession getPlayerSession()
        {
            return playerSession;
        }

    }

}
