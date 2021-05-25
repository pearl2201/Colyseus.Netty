using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Coleseus.Shared.Server.Netty
{

    /**
	 * This class holds configuration information thats useful to start a netty
	 * server. It has information on port numbers, {@link EventLoopGroup}s and
	 * {@link ChannelOption}s.
	 * 
	 * @author Abraham Menacherry
	 * 
	 */
    public class NettyConfig
    {
        private Dictionary<ChannelOption<dynamic>, Object> channelOptions;
        private IEventLoopGroup bossGroup;
        private MultithreadEventLoopGroup workerGroup;
        private int bossThreadCount;
        private int workerThreadCount;
        private IPEndPoint socketAddress;
        private int portNumber = 18090;

        private Mutex mut = new Mutex();

        public Dictionary<ChannelOption<dynamic>, Object> getChannelOptions()
        {

            return channelOptions;
        }

        public void setChannelOptions(
                Dictionary<ChannelOption<dynamic>, Object> channelOptions)
        {
            this.channelOptions = channelOptions;
        }

        public MultithreadEventLoopGroup getBossGroup()
        {
            mut.WaitOne();
            if (null == bossGroup)
            {
                if (0 >= bossThreadCount)
                {
                    bossGroup = new MultithreadEventLoopGroup();
                }
                else
                {
                    bossGroup = new MultithreadEventLoopGroup(bossThreadCount);
                }
            }
            mut.ReleaseMutex();
            return (MultithreadEventLoopGroup)bossGroup;
        }

        public void setBossGroup(MultithreadEventLoopGroup bossGroup)
        {
            this.bossGroup = bossGroup;
        }

        public MultithreadEventLoopGroup getWorkerGroup()
        {
            mut.WaitOne();
            if (null == workerGroup)
            {
                if (0 >= workerThreadCount)
                {
                    workerGroup = new MultithreadEventLoopGroup();
                }
                else
                {
                    workerGroup = new MultithreadEventLoopGroup(workerThreadCount);
                }
            }
            mut.ReleaseMutex();
            return workerGroup;
        }

        public void setWorkerGroup(MultithreadEventLoopGroup workerGroup)
        {
            this.workerGroup = workerGroup;
        }

        public int getBossThreadCount()
        {
            return bossThreadCount;
        }

        public void setBossThreadCount(int bossThreadCount)
        {
            this.bossThreadCount = bossThreadCount;
        }

        public int getWorkerThreadCount()
        {
            return workerThreadCount;
        }

        public void setWorkerThreadCount(int workerThreadCount)
        {
            this.workerThreadCount = workerThreadCount;
        }

        public IPEndPoint getSocketAddress()
        {
            mut.WaitOne();
            if (null == socketAddress)
            {
                socketAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portNumber);
            }
            mut.ReleaseMutex();
            return socketAddress;
        }

        public void setSocketAddress(IPEndPoint socketAddress)
        {
            this.socketAddress = socketAddress;
        }

        public int getPortNumber()
        {
            return portNumber;
        }

        public void setPortNumber(int portNumber)
        {
            this.portNumber = portNumber;
        }

    }

}
