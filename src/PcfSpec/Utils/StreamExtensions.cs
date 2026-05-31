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

    public static List<byte> ReadUInt8List(this Stream stream, int count)
    {
        var values = new List<byte>(count);
        for (var i = 0; i < count; i++)
        {
            values.Add(stream.ReadUInt8());
        }
        return values;
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

    public static List<sbyte> ReadInt8List(this Stream stream, int count)
    {
        var values = new List<sbyte>(count);
        for (var i = 0; i < count; i++)
        {
            values.Add(stream.ReadInt8());
        }
        return values;
    }

    public static ushort ReadUInt16(this Stream stream, bool msByteFirst = false)
    {
        Span<byte> buffer = stackalloc byte[2];
        stream.ReadExactly(buffer);
        return msByteFirst ? BinaryPrimitives.ReadUInt16BigEndian(buffer) : BinaryPrimitives.ReadUInt16LittleEndian(buffer);
    }

    public static List<ushort> ReadUInt16List(this Stream stream, int count, bool msByteFirst = false)
    {
        var values = new List<ushort>(count);
        for (var i = 0; i < count; i++)
        {
            values.Add(stream.ReadUInt16(msByteFirst));
        }
        return values;
    }

    public static short ReadInt16(this Stream stream, bool msByteFirst = false)
    {
        Span<byte> buffer = stackalloc byte[2];
        stream.ReadExactly(buffer);
        return msByteFirst ? BinaryPrimitives.ReadInt16BigEndian(buffer) : BinaryPrimitives.ReadInt16LittleEndian(buffer);
    }

    public static List<short> ReadInt16List(this Stream stream, int count, bool msByteFirst = false)
    {
        var values = new List<short>(count);
        for (var i = 0; i < count; i++)
        {
            values.Add(stream.ReadInt16(msByteFirst));
        }
        return values;
    }

    public static uint ReadUInt32(this Stream stream, bool msByteFirst = false)
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        return msByteFirst ? BinaryPrimitives.ReadUInt32BigEndian(buffer) : BinaryPrimitives.ReadUInt32LittleEndian(buffer);
    }

    public static List<uint> ReadUInt32List(this Stream stream, int count, bool msByteFirst = false)
    {
        var values = new List<uint>(count);
        for (var i = 0; i < count; i++)
        {
            values.Add(stream.ReadUInt32(msByteFirst));
        }
        return values;
    }

    public static int ReadInt32(this Stream stream, bool msByteFirst = false)
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        return msByteFirst ? BinaryPrimitives.ReadInt32BigEndian(buffer) : BinaryPrimitives.ReadInt32LittleEndian(buffer);
    }

    public static List<int> ReadInt32List(this Stream stream, int count, bool msByteFirst = false)
    {
        var values = new List<int>(count);
        for (var i = 0; i < count; i++)
        {
            values.Add(stream.ReadInt32(msByteFirst));
        }
        return values;
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

    public static int WriteUInt8List(this Stream stream, List<byte> values)
    {
        var size = 0;
        foreach (var value in values)
        {
            size += stream.WriteUInt8(value);
        }
        return size;
    }

    public static int WriteInt8(this Stream stream, sbyte value)
    {
        stream.WriteByte((byte)value);
        return 1;
    }

    public static int WriteInt8List(this Stream stream, List<sbyte> values)
    {
        var size = 0;
        foreach (var value in values)
        {
            size += stream.WriteInt8(value);
        }
        return size;
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

    public static int WriteUInt16List(this Stream stream, List<ushort> values, bool msByteFirst = false)
    {
        var size = 0;
        foreach (var value in values)
        {
            size += stream.WriteUInt16(value, msByteFirst);
        }
        return size;
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

    public static int WriteInt16List(this Stream stream, List<short> values, bool msByteFirst = false)
    {
        var size = 0;
        foreach (var value in values)
        {
            size += stream.WriteInt16(value, msByteFirst);
        }
        return size;
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

    public static int WriteUInt32List(this Stream stream, List<uint> values, bool msByteFirst = false)
    {
        var size = 0;
        foreach (var value in values)
        {
            size += stream.WriteUInt32(value, msByteFirst);
        }
        return size;
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

    public static int WriteInt32List(this Stream stream, List<int> values, bool msByteFirst = false)
    {
        var size = 0;
        foreach (var value in values)
        {
            size += stream.WriteInt32(value, msByteFirst);
        }
        return size;
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
