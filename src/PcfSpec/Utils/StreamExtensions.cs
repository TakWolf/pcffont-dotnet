using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("PcfSpec.Tests")]
namespace PcfSpec.Utils;

internal static class StreamExtensions
{
    public static byte[] ReadBytes(this Stream stream, int size, bool throwOnEndOfStream = true)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(size);
        if (size == 0)
        {
            return [];
        }
        var buffer = new byte[size];
        var numRead = stream.ReadAtLeast(buffer, buffer.Length, throwOnEndOfStream);
        if (numRead != buffer.Length)
        {
            var copy = new byte[numRead];
            Buffer.BlockCopy(buffer, 0, copy, 0, numRead);
            buffer = copy;
        }
        return buffer;
    }

    public static byte ReadUInt8(this Stream stream)
    {
        var b = stream.ReadByte();
        if (b == -1)
        {
            throw new EndOfStreamException("Unable to read beyond the end of the stream.");
        }
        return (byte)b;
    }

    public static sbyte ReadInt8(this Stream stream)
    {
        var b = stream.ReadByte();
        if (b == -1)
        {
            throw new EndOfStreamException("Unable to read beyond the end of the stream.");
        }
        return (sbyte)b;
    }

    public static ushort ReadUInt16(this Stream stream, bool msByteFirst = false)
    {
        Span<byte> buffer = stackalloc byte[2];
        stream.ReadExactly(buffer);
        return msByteFirst ? BinaryPrimitives.ReadUInt16BigEndian(buffer) : BinaryPrimitives.ReadUInt16LittleEndian(buffer);
    }

    public static short ReadInt16(this Stream stream, bool msByteFirst = false)
    {
        Span<byte> buffer = stackalloc byte[2];
        stream.ReadExactly(buffer);
        return msByteFirst ? BinaryPrimitives.ReadInt16BigEndian(buffer) : BinaryPrimitives.ReadInt16LittleEndian(buffer);
    }

    public static uint ReadUInt32(this Stream stream, bool msByteFirst = false)
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        return msByteFirst ? BinaryPrimitives.ReadUInt32BigEndian(buffer) : BinaryPrimitives.ReadUInt32LittleEndian(buffer);
    }

    public static int ReadInt32(this Stream stream, bool msByteFirst = false)
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        return msByteFirst ? BinaryPrimitives.ReadInt32BigEndian(buffer) : BinaryPrimitives.ReadInt32LittleEndian(buffer);
    }

    public static string ReadString(this Stream stream)
    {
        var buffer = new List<byte>();
        while (true)
        {
            var b = stream.ReadUInt8();
            if (b == 0)
            {
                break;
            }
            buffer.Add(b);
        }
        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    public static bool ReadBool(this Stream stream) => stream.ReadUInt8() != 0;

    public static int WriteBytes(this Stream stream, ReadOnlySpan<byte> values)
    {
        stream.Write(values);
        return values.Length;
    }

    public static int WriteUInt8(this Stream stream, byte value)
    {
        stream.WriteByte(value);
        return 1;
    }

    public static int WriteInt8(this Stream stream, sbyte value)
    {
        stream.WriteByte((byte)value);
        return 1;
    }

    public static int WriteUInt16(this Stream stream, ushort value, bool msByteFirst = false)
    {
        Span<byte> buffer = stackalloc byte[2];
        if (msByteFirst)
        {
            BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        }
        return stream.WriteBytes(buffer);
    }

    public static int WriteInt16(this Stream stream, short value, bool msByteFirst = false)
    {
        Span<byte> buffer = stackalloc byte[2];
        if (msByteFirst)
        {
            BinaryPrimitives.WriteInt16BigEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
        }
        return stream.WriteBytes(buffer);
    }

    public static int WriteUInt32(this Stream stream, uint value, bool msByteFirst = false)
    {
        Span<byte> buffer = stackalloc byte[4];
        if (msByteFirst)
        {
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        }
        return stream.WriteBytes(buffer);
    }

    public static int WriteInt32(this Stream stream, int value, bool msByteFirst = false)
    {
        Span<byte> buffer = stackalloc byte[4];
        if (msByteFirst)
        {
            BinaryPrimitives.WriteInt32BigEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
        }
        return stream.WriteBytes(buffer);
    }

    public static int WriteString(this Stream stream, string value) => stream.WriteBytes(Encoding.UTF8.GetBytes(value)) + stream.WriteNulls(1);

    public static int WriteBool(this Stream stream, bool value) => stream.WriteUInt8(value ? (byte)1 : (byte)0);

    public static int WriteNulls(this Stream stream, int size)
    {
        for (var i = 0; i < size; i++)
        {
            stream.WriteByte(0);
        }
        return size;
    }

    public static int AlignTo4ByteWithNulls(this Stream stream) => stream.WriteNulls((int)(3 - (stream.Position + 3) % 4));
}
