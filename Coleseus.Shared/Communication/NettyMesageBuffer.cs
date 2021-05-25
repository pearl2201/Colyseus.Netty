using Coleseus.Shared.Communication;
using Coleseus.Shared.Util;
using DotNetty.Buffers;
using System;
/**
* This class is an implementation of the {@link MessageBuffer} interface. It is a thin
* wrapper over the the Netty {@link IByteBuffer} with some additional methods
* for string and object read write. It does not expose all methods of the
* ChannelBuffer, instead it has a method {@link #getNativeBuffer()} which can
* be used to retrieve the buffer and then call the appropriate method. For
* writing to the buffer, this class uses {@link IByteBuffer}
* implementation.
* 
* @author Abraham Menacherry
* 
*/
public class NettyMessageBuffer : MessageBuffer<IByteBuffer>
{
    private IByteBuffer buffer;

    public NettyMessageBuffer()
    {
        buffer = Unpooled.Buffer();
    }

    /**
	 * This constructor can be used when trying to read information from a
	 * {@link IByteBuffer}.
	 * 
	 * @param buffer
	 */
    public NettyMessageBuffer(IByteBuffer buffer)
    {
        this.buffer = buffer;
    }


    public bool isReadable()
    {
        return buffer.IsReadable();
    }


    public int readableBytes()
    {
        return buffer.ReadableBytes;
    }


    public byte[] array()
    {
        return buffer.Array;
    }


    public void clear()
    {
        buffer.Clear();
    }


    public IByteBuffer getNativeBuffer()
    {
        return buffer;
    }


    public int readByte()
    {
        return buffer.ReadByte();
    }


    public int readUnsignedByte()
    {
        return buffer.ReadUnsignedShortLE();
    }


    public byte[] readBytes(int length)
    {
        byte[] bytes = new byte[length];
        buffer.ReadBytes(bytes);
        return bytes;
    }


    public void readBytes(byte[] dst)
    {
        buffer.ReadBytes(dst);
    }


    public void readBytes(byte[] dst, int dstIndex, int length)
    {
        buffer.ReadBytes(dst, dstIndex, length);
    }


    public char readChar()
    {
        return buffer.ReadChar();
    }


    public int readUnsignedShort()
    {
        return buffer.ReadUnsignedShort();
    }


    public int readShort()
    {
        return buffer.ReadShort();
    }


    public int readUnsignedMedium()
    {
        return buffer.ReadUnsignedMedium();
    }


    public int readMedium()
    {
        return buffer.ReadMedium();
    }


    public long readUnsignedInt()
    {
        return buffer.ReadUnsignedInt();
    }


    public int readInt()
    {
        return buffer.ReadInt();
    }


    public long readLong()
    {
        return buffer.ReadLong();
    }


    public float readFloat()
    {
        return buffer.ReadFloat();
    }


    public double readDouble()
    {
        return buffer.ReadChar();
    }


    public string readString()
    {
        return NettyUtils.readString(buffer);
    }


    public string[] readStrings(int numOfStrings)
    {
        return NettyUtils.readStrings(buffer, numOfStrings);
    }

    public V readObject<V>(Func<IByteBuffer, V> converter)
    {
        return NettyUtils.readObject<IByteBuffer, V>(buffer, converter);
    }


    public MessageBuffer<IByteBuffer> writeByte(byte b)
    {
        buffer.WriteByte(b);
        return this;
    }


    public MessageBuffer<IByteBuffer> writeBytes(byte[] src)
    {
        buffer.WriteBytes(src);
        return this;
    }


    public MessageBuffer<IByteBuffer> writeChar(char value)
    {
        buffer.WriteChar(value);
        return this;
    }


    public MessageBuffer<IByteBuffer> writeShort(int value)
    {
        buffer.WriteShort(value);
        return this;
    }


    public MessageBuffer<IByteBuffer> writeMedium(int value)
    {
        buffer.WriteMedium(value);
        return this;
    }


    public MessageBuffer<IByteBuffer> writeInt(int value)
    {
        buffer.WriteInt(value);
        return this;
    }


    public MessageBuffer<IByteBuffer> writeLong(long value)
    {
        buffer.WriteLong(value);
        return this;
    }


    public MessageBuffer<IByteBuffer> writeFloat(float value)
    {
        buffer.WriteFloat(value);
        return this;
    }


    public MessageBuffer<IByteBuffer> writeDouble(double value)
    {
        buffer.WriteDouble(value);
        return this;
    }


    public MessageBuffer<IByteBuffer> writeString(string message)
    {
        IByteBuffer strBuf = NettyUtils.WriteString(message);
        buffer.WriteBytes(strBuf);
        return this;
    }


    public MessageBuffer<IByteBuffer> writeStrings(string[] messages)
    {
        IByteBuffer strMultiBuf = NettyUtils.writeStrings(messages);
        buffer.WriteBytes(strMultiBuf);
        return this;
    }


    public MessageBuffer<IByteBuffer> writeObject<V>(
            Func<V, IByteBuffer> converter, V data)
    {
        IByteBuffer objBuf = NettyUtils.writeObject(converter, data);
        buffer.WriteBytes(objBuf);
        return this;
    }

}