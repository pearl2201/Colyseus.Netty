// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Colyseus.NettyServer
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using DotNetty.Buffers;
    using DotNetty.Common.Internal.Logging;
    using DotNetty.Transport.Channels;
    using Microsoft.Extensions.Logging;
    using Serilog.Events;

    /// <summary>
    ///     A <see cref="IChannelHandler" /> that logs all events using a logging framework.
    ///     By default, all events are logged at <tt>DEBUG</tt> level.
    /// </summary>
    public class SeriLoggingHandler : ChannelHandlerAdapter
    {


        protected readonly LogEventLevel InternalLevel;
        protected readonly Serilog.ILogger Logger;

        /// <summary>
        ///     Creates a new instance whose logger name is the fully qualified class
        ///     name of the instance with hex dump enabled.
        /// </summary>
        public SeriLoggingHandler() : base()
        {
            InternalLevel = LogEventLevel.Information;
            Logger = Serilog.Log.Logger;
        }

        /// <summary>
        ///     Creates a new instance whose logger name is the fully qualified class
        ///     name of the instance with hex dump enabled.
        /// </summary>
        public SeriLoggingHandler(LogEventLevel level) : base()
        {
            InternalLevel = level;
            Logger = Serilog.Log.Logger;
        }







        public override bool IsSharable => true;

        /// <summary>
        ///     Returns the <see cref="LogLevel" /> that this handler uses to log
        /// </summary>
        public LogLevel Level { get; }

        public override void ChannelRegistered(IChannelHandlerContext ctx)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "REGISTERED"));
            }
            ctx.FireChannelRegistered();
        }

        public override void ChannelUnregistered(IChannelHandlerContext ctx)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "UNREGISTERED"));
            }
            ctx.FireChannelUnregistered();
        }

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "ACTIVE"));
            }
            ctx.FireChannelActive();
        }

        public override void ChannelInactive(IChannelHandlerContext ctx)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "INACTIVE"));
            }
            ctx.FireChannelInactive();
        }

        public override void ExceptionCaught(IChannelHandlerContext ctx, Exception cause)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "EXCEPTION", cause), cause);
            }
            ctx.FireExceptionCaught(cause);
        }

        public override void UserEventTriggered(IChannelHandlerContext ctx, object evt)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "USER_EVENT", evt));
            }
            ctx.FireUserEventTriggered(evt);
        }

        public override Task BindAsync(IChannelHandlerContext ctx, EndPoint localAddress)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "BIND", localAddress));
            }
            return ctx.BindAsync(localAddress);
        }

        public override Task ConnectAsync(IChannelHandlerContext ctx, EndPoint remoteAddress, EndPoint localAddress)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "CONNECT", remoteAddress, localAddress));
            }
            return ctx.ConnectAsync(remoteAddress, localAddress);
        }

        public override Task DisconnectAsync(IChannelHandlerContext ctx)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "DISCONNECT"));
            }
            return ctx.DisconnectAsync();
        }

        public override Task CloseAsync(IChannelHandlerContext ctx)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "CLOSE"));
            }
            return ctx.CloseAsync();
        }

        public override Task DeregisterAsync(IChannelHandlerContext ctx)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "DEREGISTER"));
            }
            return ctx.DeregisterAsync();
        }

        public override void ChannelRead(IChannelHandlerContext ctx, object message)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "RECEIVED", message));
            }
            ctx.FireChannelRead(message);
        }

        public override void ChannelReadComplete(IChannelHandlerContext ctx)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "RECEIVED_COMPLETE"));
            }
            ctx.FireChannelReadComplete();
        }

        public override void ChannelWritabilityChanged(IChannelHandlerContext ctx)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "WRITABILITY", ctx.Channel.IsWritable));
            }
            ctx.FireChannelWritabilityChanged();
        }

        public override void HandlerAdded(IChannelHandlerContext ctx)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "HANDLER_ADDED"));
            }
        }
        public override void HandlerRemoved(IChannelHandlerContext ctx)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "HANDLER_REMOVED"));
            }
        }

        public override void Read(IChannelHandlerContext ctx)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "READ"));
            }
            ctx.Read();
        }

        public override Task WriteAsync(IChannelHandlerContext ctx, object msg)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "WRITE", msg));
            }
            return ctx.WriteAsync(msg);
        }

        public override void Flush(IChannelHandlerContext ctx)
        {
            if (this.Logger.IsEnabled(this.InternalLevel))
            {
                this.Logger.Write(this.InternalLevel, this.Format(ctx, "FLUSH"));
            }
            ctx.Flush();
        }

        /// <summary>
        ///     Formats an event and returns the formatted message
        /// </summary>
        /// <param name="eventName">the name of the event</param>
        protected virtual string Format(IChannelHandlerContext ctx, string eventName)
        {
            string chStr = ctx.Channel.ToString();
            return new StringBuilder(chStr.Length + 1 + eventName.Length)
                .Append(chStr)
                .Append(' ')
                .Append(eventName)
                .ToString();
        }

        /// <summary>
        ///     Formats an event and returns the formatted message.
        /// </summary>
        /// <param name="eventName">the name of the event</param>
        /// <param name="arg">the argument of the event</param>
        protected virtual string Format(IChannelHandlerContext ctx, string eventName, object arg)
        {
            if (arg is IByteBuffer)
            {
                return this.FormatByteBuffer(ctx, eventName, (IByteBuffer)arg);
            }
            else if (arg is IByteBufferHolder)
            {
                return this.FormatByteBufferHolder(ctx, eventName, (IByteBufferHolder)arg);
            }
            else
            {
                return this.FormatSimple(ctx, eventName, arg);
            }
        }

        /// <summary>
        ///     Formats an event and returns the formatted message.  This method is currently only used for formatting
        ///     <see cref="IChannelHandler.ConnectAsync(IChannelHandlerContext, EndPoint, EndPoint)" />
        /// </summary>
        /// <param name="eventName">the name of the event</param>
        /// <param name="firstArg">the first argument of the event</param>
        /// <param name="secondArg">the second argument of the event</param>
        protected virtual string Format(IChannelHandlerContext ctx, string eventName, object firstArg, object secondArg)
        {
            if (secondArg == null)
            {
                return this.FormatSimple(ctx, eventName, firstArg);
            }
            string chStr = ctx.Channel.ToString();
            string arg1Str = firstArg.ToString();
            string arg2Str = secondArg.ToString();

            var buf = new StringBuilder(
                chStr.Length + 1 + eventName.Length + 2 + arg1Str.Length + 2 + arg2Str.Length);
            buf.Append(chStr).Append(' ').Append(eventName).Append(": ")
                .Append(arg1Str).Append(", ").Append(arg2Str);
            return buf.ToString();
        }

        /// <summary>
        ///     Generates the default log message of the specified event whose argument is a  <see cref="IByteBuffer" />.
        /// </summary>
        string FormatByteBuffer(IChannelHandlerContext ctx, string eventName, IByteBuffer msg)
        {
            string chStr = ctx.Channel.ToString();
            int length = msg.ReadableBytes;
            if (length == 0)
            {
                var buf = new StringBuilder(chStr.Length + 1 + eventName.Length + 4);
                buf.Append(chStr).Append(' ').Append(eventName).Append(": 0B");
                return buf.ToString();
            }
            else
            {
                int rows = length / 16 + (length % 15 == 0 ? 0 : 1) + 4;
                var buf = new StringBuilder(chStr.Length + 1 + eventName.Length + 2 + 10 + 1 + 2 + rows * 80);

                buf.Append(chStr).Append(' ').Append(eventName).Append(": ").Append(length).Append('B').Append('\n');
                ByteBufferUtil.AppendPrettyHexDump(buf, msg);

                return buf.ToString();
            }
        }

        /// <summary>
        ///     Generates the default log message of the specified event whose argument is a <see cref="IByteBufferHolder" />.
        /// </summary>
        string FormatByteBufferHolder(IChannelHandlerContext ctx, string eventName, IByteBufferHolder msg)
        {
            string chStr = ctx.Channel.ToString();
            string msgStr = msg.ToString();
            IByteBuffer content = msg.Content;
            int length = content.ReadableBytes;
            if (length == 0)
            {
                var buf = new StringBuilder(chStr.Length + 1 + eventName.Length + 2 + msgStr.Length + 4);
                buf.Append(chStr).Append(' ').Append(eventName).Append(", ").Append(msgStr).Append(", 0B");
                return buf.ToString();
            }
            else
            {
                int rows = length / 16 + (length % 15 == 0 ? 0 : 1) + 4;
                var buf = new StringBuilder(
                    chStr.Length + 1 + eventName.Length + 2 + msgStr.Length + 2 + 10 + 1 + 2 + rows * 80);

                buf.Append(chStr).Append(' ').Append(eventName).Append(": ")
                    .Append(msgStr).Append(", ").Append(length).Append('B').Append('\n');
                ByteBufferUtil.AppendPrettyHexDump(buf, content);

                return buf.ToString();
            }
        }

        /// <summary>
        ///     Generates the default log message of the specified event whose argument is an arbitrary object.
        /// </summary>
        string FormatSimple(IChannelHandlerContext ctx, string eventName, object msg)
        {
            string chStr = ctx.Channel.ToString();
            string msgStr = msg.ToString();
            var buf = new StringBuilder(chStr.Length + 1 + eventName.Length + 2 + msgStr.Length);
            return buf.Append(chStr).Append(' ').Append(eventName).Append(": ").Append(msgStr).ToString();
        }
    }
}