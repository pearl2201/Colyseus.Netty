using Coleseus.Shared.Communication;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using System.Net;
using Coleseus.Shared.App;

namespace Coleseus.Shared.Util
{
    public class NettyUtils
    {
        private readonly ILogger _logger = Serilog.Log.Logger;

        public const string NETTY_CHANNEL = "NETTY_CHANNEL";

        public static IChannelPipeline getPipeLineOfConnection(
                NettyTCPMessageSender messageSender)
        {
            if (null != messageSender)
            {
                IChannel channel = messageSender.getChannel();
                IChannelPipeline pipeline = channel.Pipeline;
                return pipeline;
            }
            return null;
        }

        public static IChannelPipeline getPipeLineOfConnection(
                IPlayerSession playerSession)
        {
            return getPipeLineOfConnection((NettyTCPMessageSender)playerSession.getTcpSender());
        }

        /**
		 * A utility method to clear the netty pipeline of all handlers.
		 * 
		 * @param pipeline
		 */
        public static void clearPipeline(IChannelPipeline pipeline)
        {
            if (null == pipeline)
            {
                return;
            }
            try
            {
                int counter = 0;

                while (pipeline.First() != null)
                {
                    pipeline.RemoveFirst();
                    counter++;
                }
                Log.Verbose("Removed {} handlers from pipeline", counter);
            }
            catch (Exception e)
            {
                // all elements removed.
            }
        }

        public static IByteBuffer createBufferForOpcode(int opcode)
        {
            IByteBuffer buffer = Unpooled.Buffer(1);
            buffer.WriteByte(opcode);
            return buffer;
        }

        /**
		 * This method will read multiple strings of the buffer and return them as a
		 * string array. It internally uses the readString(IByteBuffer buffer) to
		 * accomplish this task. The strings are read back in the order they are
		 * written.
		 * 
		 * @param buffer
		 *            The buffer containing the strings, with each string being a
		 *            strlength-strbytes combination.
		 * @param numOfStrings
		 *            The number of strings to be read. Should not be negative or 0
		 * @return the strings read from the buffer as an array.
		 */
        public static String[] readStrings(IByteBuffer buffer, int numOfStrings)
        {

            string[] strings = new string[numOfStrings];
            for (int i = 0; i < numOfStrings; i++)
            {
                string theStr = readString(buffer);
                if (null == theStr) break;
                strings[i] = theStr;
            }
            return strings;
        }

        /**
		 * This method will first read an unsigned short to find the length of the
		 * string and then read the actual string based on the length. This method
		 * will also reset the reader index to end of the string
		 * 
		 * @param buffer
		 *            The Netty buffer containing at least one unsigned short
		 *            followed by a string of similar length.
		 * @return Returns the String or throws {@link IndexOutOfBoundsException} if
		 *         the length is greater than expected.
		 */
        public static String readString(IByteBuffer buffer)
        {
            String str = null;
            if (null != buffer && buffer.ReadableBytes > 2)
            {
                int length = buffer.ReadUnsignedShort();
                str = readString(buffer, length);
            }
            return str;
        }

        /**
		 * Read a string from a channel buffer with the specified length. It resets
		 * the reader index of the buffer to the end of the string.
		 * 
		 * @param buffer
		 *            The Netty buffer containing the String.
		 * @param length
		 *            The number of bytes in the String.
		 * @return Returns the read string.
		 */
        public static String readString(IByteBuffer buffer, int length)
        {
            string str = null;

            try
            {
                IByteBuffer stringBuffer = buffer.ReadSlice(length);
                byte[] bytes = stringBuffer.Array;
                str = Encoding.ASCII.GetString(bytes);
            }
            catch (Exception e)
            {
                Log.Error(
                        "Error occurred while trying to read string from buffer: {}",
                        e);
            }
            return str;
        }

        /**
		 * Writes multiple strings to a IByteBuffer with the length of the string
		 * preceding its content. So if there are two string <code>Hello</code> and
		 * <code>World</code> then the channel buffer returned would contain <Length
		 * of Hello><Hello as UTF-8 binary><Length of world><World as UTF-8 binary>
		 * 
		 * @param msgs
		 *            The messages to be written.
		 * @return {@link IByteBuffer} with format
		 *         length-stringbinary-length-stringbinary
		 */
        public static IByteBuffer writeStrings(string[] msgs)
        {
            IByteBuffer buffer = null;
            foreach (string msg in msgs)
            {
                if (null == buffer)
                {
                    buffer = WriteString(msg);
                }
                else
                {
                    IByteBuffer theBuffer = WriteString(msg);
                    if (null != theBuffer)
                    {
                        buffer = Unpooled.WrappedBuffer(buffer, theBuffer);
                    }
                }
            }
            return buffer;
        }

        /**
		 * Creates a channel buffer of which the first 2 bytes contain the length of
		 * the string in bytes and the remaining is the actual string in binary
		 * UTF-8 format.
		 * 
		 * @param msg
		 *            The string to be written.
		 * @return Returns the IByteBuffer instance containing the encoded string
		 */
        public static IByteBuffer WriteString(string msg)
        {
            IByteBuffer buffer = null;
            try
            {
                IByteBuffer stringBuffer = null;
                byte[] bytes = Encoding.ASCII.GetBytes(msg);
                stringBuffer = Unpooled.CopiedBuffer(bytes);
                int length = stringBuffer.ReadableBytes;
                IByteBuffer lengthBuffer = Unpooled.Buffer(2);
                lengthBuffer.WriteShort(length);



                buffer = Unpooled.WrappedBuffer(lengthBuffer, stringBuffer);
            }
            catch (Exception e)
            {
                Log.Error("Error occurred while trying to write string buffer: {}",
                        e);
            }
            return buffer;
        }

        public static V readObject<T, V>(IByteBuffer buffer, Func<IByteBuffer, V> decoder)
        {
            int length = 0;
            if (null != buffer && buffer.ReadableBytes > 2)
            {
                length = buffer.ReadUnsignedShort();
            }
            else
            {
                return default(V);
            }
            IByteBuffer objBuffer = buffer.ReadSlice(length);
            V obj = default(V);
            try
            {
                obj = decoder(objBuffer);
            }
            catch (Exception e)
            {
                Log.Error("Error occurred while trying to read object from buffer: {}", e);
            }
            return obj;
        }

        public static IByteBuffer writeObject<V>(
                Func<V, IByteBuffer> converter, V obj)
        {
            IByteBuffer buffer = null;
            try
            {
                IByteBuffer objectBuffer = converter(obj);
                int length = objectBuffer.ReadableBytes;
                IByteBuffer lengthBuffer = Unpooled.Buffer(2);
                lengthBuffer.WriteShort(length);
                buffer = Unpooled.WrappedBuffer(lengthBuffer,
                        objectBuffer);
            }
            catch (Exception e)
            {
                Log.Error("Error occurred while writing object to buffer: {}", e);
            }
            return buffer;
        }

        /**
		 * Read a socket address from a buffer. The socket address will be provided
		 * as two strings containing host and port.
		 * 
		 * @param buffer
		 *            The buffer containing the host and port as string.
		 * @return The InetSocketAddress object created from host and port or null
		 *         in case the strings are not there.
		 */
        public static IPEndPoint readSocketAddress(IByteBuffer buffer)
        {
            String remoteHost = NettyUtils.readString(buffer);
            int remotePort = 0;
            if (buffer.ReadableBytes >= 4)
            {
                remotePort = buffer.ReadInt();
            }
            else
            {
                return null;
            }
            IPEndPoint remoteAddress = null;
            if (null != remoteHost)
            {
                IPAddress address = IPAddress.Parse(remoteHost);
                remoteAddress = new IPEndPoint(address, remotePort);
            }
            return remoteAddress;
        }

        public static IByteBuffer writeSocketAddress(IPEndPoint socketAddress)
        {
            string host = socketAddress.Address.ToString();
            int port = socketAddress.Port;
            IByteBuffer hostName = WriteString(host);
            IByteBuffer portNum = Unpooled.Buffer(4);
            portNum.WriteInt(port);
            IByteBuffer socketAddressBuffer = Unpooled.WrappedBuffer(hostName, portNum);
            return socketAddressBuffer;
        }

        /**
		 * converts a IByteBuffer to byte array.
		 * @param buf
		 * @param isReadDestroy if true then the reader index of the IByteBuffer will be modified. 
		 * @return
		 */
        public static byte[] toByteArray(IByteBuffer buf, bool isReadDestroy)
        {
            byte[] arr = new byte[buf.ReadableBytes];
            if (isReadDestroy)
            {
                buf.ReadBytes(arr);
            }
            else
            {
                buf.GetBytes(buf.ReaderIndex, arr);
            }
            return arr;
        }
    }
}
