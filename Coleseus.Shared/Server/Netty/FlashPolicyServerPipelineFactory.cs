using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.Server.Netty
{
	/**
 * @author <a href="http://www.waywardmonkeys.com/">Bruce Mitchener</a>
 */
	public class FlashPolicyServerChannelInitalizer extends ChannelInitializer<SocketChannel>
{

	// TODO make this configurable from spring.
	private static final int MAX_IDLE_SECONDS = 60;

	/**
	 * Spring will return the actual prototype bean from its context here. It
	 * uses method lookup here.
	 * 
	 * @return a new instance of the {@link FlashPolicyServerHandler}
	 */
	protected FlashPolicyServerHandler getFlashPolicyServerHandler()
	{
		return null;
	}

	@Override
	protected void initChannel(SocketChannel ch) throws Exception
	{
		ChannelPipeline pipeline = ch.pipeline();
		pipeline.addLast("idleStateCheck", new IdleStateHandler(
				MAX_IDLE_SECONDS, MAX_IDLE_SECONDS, MAX_IDLE_SECONDS));
        pipeline.addLast("decoder", new FlashPolicyServerDecoder());
        pipeline.addLast("handler", getFlashPolicyServerHandler());
	}
}

}
