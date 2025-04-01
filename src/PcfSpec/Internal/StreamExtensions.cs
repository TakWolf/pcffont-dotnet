using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("PcfSpec.Tests")]
namespace PcfSpec.Internal;

internal static class StreamExtensions
{
    public static byte[] ReadBuffer(this Stream stream, long count, bool throwOnEndOfStream = true)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (count == 0)
        {
            return [];
        }
        var buffer = new byte[count];
        var numRead = stream.ReadAtLeast(buffer, buffer.Length, throwOnEndOfStream);
        if (numRead != buffer.Length)
        {
            var copy = new byte[numRead];
            Buffer.BlockCopy(buffer, 0, copy, 0, numRead);
            buffer = copy;
        }
        return buffer;
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

    public static List<sbyte> ReadInt8List(this Stream stream, int length) => [.. Enumerable.Range(0, length).Select(_ => stream.ReadInt8())];

    public static short ReadInt16(this Stream stream, bool msByteFirst = false)
    {
        var buffer = stream.ReadBuffer(2);
        return msByteFirst ? BinaryPrimitives.ReadInt16BigEndian(buffer) : BinaryPrimitives.ReadInt16LittleEndian(buffer);
    }

    public static List<short> ReadInt16List(this Stream stream, int length, bool msByteFirst = false)
    {
        var values = new List<short>();
        var buffer = stream.ReadBuffer(2 * length);
        foreach (var i in Enumerable.Range(0, length))
        {
            var span = buffer.AsSpan(i * 2, 2);
            values.Add(msByteFirst ? BinaryPrimitives.ReadInt16BigEndian(span) : BinaryPrimitives.ReadInt16LittleEndian(span));
        }
        return values;
    }

    public static int ReadInt32(this Stream stream, bool msByteFirst = false)
    {
        var buffer = stream.ReadBuffer(4);
        return msByteFirst ? BinaryPrimitives.ReadInt32BigEndian(buffer) : BinaryPrimitives.ReadInt32LittleEndian(buffer);
    }

    public static List<int> ReadInt32List(this Stream stream, int length, bool msByteFirst = false)
    {
        var values = new List<int>();
        var buffer = stream.ReadBuffer(4 * length);
        foreach (var i in Enumerable.Range(0, length))
        {
            var span = buffer.AsSpan(i * 4, 4);
            values.Add(msByteFirst ? BinaryPrimitives.ReadInt32BigEndian(span) : BinaryPrimitives.ReadInt32LittleEndian(span));
        }
        return values;
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

    public static List<byte> ReadUInt8List(this Stream stream, int length) => [.. Enumerable.Range(0, length).Select(_ => stream.ReadUInt8())];

    public static ushort ReadUInt16(this Stream stream, bool msByteFirst = false)
    {
        var buffer = stream.ReadBuffer(2);
        return msByteFirst ? BinaryPrimitives.ReadUInt16BigEndian(buffer) : BinaryPrimitives.ReadUInt16LittleEndian(buffer);
    }

    public static List<ushort> ReadUInt16List(this Stream stream, int length, bool msByteFirst = false)
    {
        var values = new List<ushort>();
        var buffer = stream.ReadBuffer(2 * length);
        foreach (var i in Enumerable.Range(0, length))
        {
            var span = buffer.AsSpan(i * 2, 2);
            values.Add(msByteFirst ? BinaryPrimitives.ReadUInt16BigEndian(span) : BinaryPrimitives.ReadUInt16LittleEndian(span));
        }
        return values;
    }

    public static uint ReadUInt32(this Stream stream, bool msByteFirst = false)
    {
        var buffer = stream.ReadBuffer(4);
        return msByteFirst ? BinaryPrimitives.ReadUInt32BigEndian(buffer) : BinaryPrimitives.ReadUInt32LittleEndian(buffer);
    }

    public static List<uint> ReadUInt32List(this Stream stream, int length, bool msByteFirst = false)
    {
        var values = new List<uint>();
        var buffer = stream.ReadBuffer(4 * length);
        foreach (var i in Enumerable.Range(0, length))
        {
            var span = buffer.AsSpan(i * 4, 4);
            values.Add(msByteFirst ? BinaryPrimitives.ReadUInt32BigEndian(span) : BinaryPrimitives.ReadUInt32LittleEndian(span));
        }
        return values;
    }

    public static List<byte> ReadBinary(this Stream stream, bool msBitFirst = false)
    {
        var binary = new List<byte>($"{stream.ReadUInt8():b8}".Select(bit => byte.Parse(bit.ToString())));
        if (!msBitFirst)
        {
            binary.Reverse();
        }
        return binary;
    }

    public static List<List<byte>> ReadBinaryList(this Stream stream, int length, bool msBitFirst = false) => [.. Enumerable.Range(0, length).Select(_ => stream.ReadBinary(msBitFirst))];

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

    public static List<string> ReadStringList(this Stream stream, int length) => [.. Enumerable.Range(0, length).Select(_ => stream.ReadString())];

    public static bool ReadBool(this Stream stream) => stream.ReadUInt8() != 0;

    public static int WriteBuffer(this Stream stream, ReadOnlySpan<byte> values)
    {
        stream.Write(values);
        return values.Length;
    }

    public static int WriteInt8(this Stream stream, sbyte value)
    {
        stream.WriteByte((byte)value);
        return 1;
    }

    public static int WriteInt8List(this Stream stream, List<sbyte> values)
    {
        var buffer = new byte[values.Count];
        foreach (var i in Enumerable.Range(0, values.Count))
        {
            buffer[i] = (byte)values[i];
        }
        return stream.WriteBuffer(buffer);
    }

    public static int WriteInt16(this Stream stream, short value, bool msByteFirst = false)
    {
        var buffer = new byte[2];
        if (msByteFirst)
        {
            BinaryPrimitives.WriteInt16BigEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
        }
        return stream.WriteBuffer(buffer);
    }

    public static int WriteInt16List(this Stream stream, List<short> values, bool msByteFirst = false)
    {
        var buffer = new byte[2 * values.Count];
        foreach (var i in Enumerable.Range(0, values.Count))
        {
            var span = buffer.AsSpan(i * 2, 2);
            if (msByteFirst)
            {
                BinaryPrimitives.WriteInt16BigEndian(span, values[i]);
            }
            else
            {
                BinaryPrimitives.WriteInt16LittleEndian(span, values[i]);
            }
        }
        return stream.WriteBuffer(buffer);
    }

    public static int WriteInt32(this Stream stream, int value, bool msByteFirst = false)
    {
        var buffer = new byte[4];
        if (msByteFirst)
        {
            BinaryPrimitives.WriteInt32BigEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
        }
        return stream.WriteBuffer(buffer);
    }

    public static int WriteInt32List(this Stream stream, List<int> values, bool msByteFirst = false)
    {
        var buffer = new byte[4 * values.Count];
        foreach (var i in Enumerable.Range(0, values.Count))
        {
            var span = buffer.AsSpan(i * 4, 4);
            if (msByteFirst)
            {
                BinaryPrimitives.WriteInt32BigEndian(span, values[i]);
            }
            else
            {
                BinaryPrimitives.WriteInt32LittleEndian(span, values[i]);
            }
        }
        return stream.WriteBuffer(buffer);
    }

    public static int WriteUInt8(this Stream stream, byte value)
    {
        stream.WriteByte(value);
        return 1;
    }

    public static int WriteUInt8List(this Stream stream, List<byte> values) => stream.WriteBuffer(values.ToArray());

    public static int WriteUInt16(this Stream stream, ushort value, bool msByteFirst = false)
    {
        var buffer = new byte[2];
        if (msByteFirst)
        {
            BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        }
        return stream.WriteBuffer(buffer);
    }

    public static int WriteUInt16List(this Stream stream, List<ushort> values, bool msByteFirst = false)
    {
        var buffer = new byte[2 * values.Count];
        foreach (var i in Enumerable.Range(0, values.Count))
        {
            var span = buffer.AsSpan(i * 2, 2);
            if (msByteFirst)
            {
                BinaryPrimitives.WriteUInt16BigEndian(span, values[i]);
            }
            else
            {
                BinaryPrimitives.WriteUInt16LittleEndian(span, values[i]);
            }
        }
        return stream.WriteBuffer(buffer);
    }

    public static int WriteUInt32(this Stream stream, uint value, bool msByteFirst = false)
    {
        var buffer = new byte[4];
        if (msByteFirst)
        {
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        }
        return stream.WriteBuffer(buffer);
    }

    public static int WriteUInt32List(this Stream stream, List<uint> values, bool msByteFirst = false)
    {
        var buffer = new byte[4 * values.Count];
        foreach (var i in Enumerable.Range(0, values.Count))
        {
            var span = buffer.AsSpan(i * 4, 4);
            if (msByteFirst)
            {
                BinaryPrimitives.WriteUInt32BigEndian(span, values[i]);
            }
            else
            {
                BinaryPrimitives.WriteUInt32LittleEndian(span, values[i]);
            }
        }
        return stream.WriteBuffer(buffer);
    }

    public static int WriteBinary(this Stream stream, List<byte> value, bool msBitFirst = false)
    {
        if (!msBitFirst)
        {
            value = new List<byte>(value);
            value.Reverse();
        }
        return stream.WriteUInt8(Convert.ToByte(string.Join("", value), 2));
    }

    public static int WriteBinaryList(this Stream stream, List<List<byte>> values, bool msBitFirst = false)
    {
        var count = 0;
        foreach (var value in values)
        {
            count += stream.WriteBinary(value, msBitFirst);
        }
        return count;
    }

    public static int WriteString(this Stream stream, string value) => stream.WriteBuffer(Encoding.UTF8.GetBytes(value)) + stream.WriteNulls(1);

    public static int WriteStringList(this Stream stream, List<string> values)
    {
        var count = 0;
        foreach (var value in values)
        {
            count += stream.WriteString(value);
        }
        return count;
    }

    public static int WriteBool(this Stream stream, bool value) => stream.WriteUInt8(value ? (byte)1 : (byte)0);

    public static int WriteNulls(this Stream stream, long count) => stream.WriteBuffer(new byte[count]);

    public static int AlignToBit32WithNulls(this Stream stream) => stream.WriteNulls(3 - (stream.Position + 3) % 4);
}
