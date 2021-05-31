using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
namespace Coleseus.Shared.Server.Netty
{
    /**
   * An interface specific to the JBoss Netty implementation. It will be
   * implemented by a class that will start up a Netty server at a specified port.
   * 
   * @author Abraham Menacherry
   * 
   */
    public interface NettyServer : IServer 
    {
        /**
         * createServerBootstrap will create a pipeline factory and save it as a
         * class variable. This method can then be used to retrieve that value.
         * 
         * @return Returns the channel pipeline factory that is associated with this
         *         netty server.
         */
        IChannelHandler getChannelInitializer();

        /**
         * Method can be used to set the pipeline factory that is to be used by the
         * netty server.
         * 
         * @param initializer
         *            The factory which will create a pipeline on each incoming
         *            connection.
         */
        void setChannelInitializer(IChannelHandler initializer);

        /**
         * Get the netty configuration associated with this server.
         * 
         * @return returns the configuration instance which is passed in via
         *         constructor.
         */
        NettyConfig getNettyConfig();

    }

}
