using Colyseus.Common;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;

using Serilog;

namespace Colyseus.NettyServer
{
    public class PersonServerHandler : SimpleChannelInboundHandler<Person>
    {
       

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            // Detect when client disconnects
            ctx.Channel.CloseCompletion.ContinueWith((x) => Log.Information("Channel Closed"));
        }

        // The Channel is closed hence the connection is closed
        public override void ChannelInactive(IChannelHandlerContext ctx) => Log.Information("Client disconnected");

        protected override void ChannelRead0(IChannelHandlerContext ctx, Person person)
        {
            Log.Information("Received message: " + person);
            person.Name = person.Name.ToUpper();
            ctx.WriteAndFlushAsync(person);
        }

        public override bool IsSharable => true;
    }
}
