using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        var numRead = stream.ReadAtLeast(buffer, size, throwOnEndOfStream);
        if (numRead != size)
        {
            var newBuffer = new byte[numRead];
            Buffer.BlockCopy(buffer, 0, newBuffer, 0, numRead);
            buffer = newBuffer;
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

    public static byte[] ReadUInt8Array(this Stream stream, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (count == 0)
        {
            return [];
        }

        var values = new byte[count];
        stream.ReadExactly(values);
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

    public static sbyte[] ReadInt8Array(this Stream stream, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (count == 0)
        {
            return [];
        }

        var values = new sbyte[count];
        stream.ReadExactly(MemoryMarshal.AsBytes(values.AsSpan()));
        return values;
    }

    public static ushort ReadUInt16(this Stream stream, bool msByteFirst = false)
    {
        Span<byte> span = stackalloc byte[2];
        stream.ReadExactly(span);
        return msByteFirst ? BinaryPrimitives.ReadUInt16BigEndian(span) : BinaryPrimitives.ReadUInt16LittleEndian(span);
    }

    public static ushort[] ReadUInt16Array(this Stream stream, int count, bool msByteFirst = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (count == 0)
        {
            return [];
        }

        var values = new ushort[count];
        var target = values.AsSpan();
        stream.ReadExactly(MemoryMarshal.AsBytes(target));
        if (msByteFirst != !BitConverter.IsLittleEndian)
        {
            for (var i = 0; i < target.Length; i++)
            {
                target[i] = BinaryPrimitives.ReverseEndianness(target[i]);
            }
        }
        return values;
    }

    public static short ReadInt16(this Stream stream, bool msByteFirst = false)
    {
        Span<byte> span = stackalloc byte[2];
        stream.ReadExactly(span);
        return msByteFirst ? BinaryPrimitives.ReadInt16BigEndian(span) : BinaryPrimitives.ReadInt16LittleEndian(span);
    }

    public static short[] ReadInt16Array(this Stream stream, int count, bool msByteFirst = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (count == 0)
        {
            return [];
        }

        var values = new short[count];
        var target = values.AsSpan();
        stream.ReadExactly(MemoryMarshal.AsBytes(target));
        if (msByteFirst != !BitConverter.IsLittleEndian)
        {
            for (var i = 0; i < target.Length; i++)
            {
                target[i] = BinaryPrimitives.ReverseEndianness(target[i]);
            }
        }
        return values;
    }

    public static uint ReadUInt32(this Stream stream, bool msByteFirst = false)
    {
        Span<byte> span = stackalloc byte[4];
        stream.ReadExactly(span);
        return msByteFirst ? BinaryPrimitives.ReadUInt32BigEndian(span) : BinaryPrimitives.ReadUInt32LittleEndian(span);
    }

    public static uint[] ReadUInt32Array(this Stream stream, int count, bool msByteFirst = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (count == 0)
        {
            return [];
        }

        var values = new uint[count];
        var target = values.AsSpan();
        stream.ReadExactly(MemoryMarshal.AsBytes(target));
        if (msByteFirst != !BitConverter.IsLittleEndian)
        {
            for (var i = 0; i < target.Length; i++)
            {
                target[i] = BinaryPrimitives.ReverseEndianness(target[i]);
            }
        }
        return values;
    }

    public static int ReadInt32(this Stream stream, bool msByteFirst = false)
    {
        Span<byte> span = stackalloc byte[4];
        stream.ReadExactly(span);
        return msByteFirst ? BinaryPrimitives.ReadInt32BigEndian(span) : BinaryPrimitives.ReadInt32LittleEndian(span);
    }

    public static int[] ReadInt32Array(this Stream stream, int count, bool msByteFirst = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (count == 0)
        {
            return [];
        }

        var values = new int[count];
        var target = values.AsSpan();
        stream.ReadExactly(MemoryMarshal.AsBytes(target));
        if (msByteFirst != !BitConverter.IsLittleEndian)
        {
            for (var i = 0; i < target.Length; i++)
            {
                target[i] = BinaryPrimitives.ReverseEndianness(target[i]);
            }
        }
        return values;
    }

    public static string ReadString(this Stream stream)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(256);
        try
        {
            var size = 0;
            while (true)
            {
                var b = stream.ReadUInt8();
                if (b == 0)
                {
                    break;
                }

                if (size == buffer.Length)
                {
                    var newBuffer = ArrayPool<byte>.Shared.Rent(size * 2);
                    Buffer.BlockCopy(buffer, 0, newBuffer, 0, size);
                    ArrayPool<byte>.Shared.Return(buffer);
                    buffer = newBuffer;
                }

                buffer[size] = b;
                size += 1;
            }
            return Encoding.UTF8.GetString(buffer, 0, size);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static bool ReadBool(this Stream stream) => stream.ReadUInt8() != 0;

    public static int WriteBytes(this Stream stream, ReadOnlySpan<byte> buffer)
    {
        if (buffer.IsEmpty)
        {
            return 0;
        }

        stream.Write(buffer);
        return buffer.Length;
    }

    public static int WriteUInt8(this Stream stream, byte value)
    {
        stream.WriteByte(value);
        return 1;
    }

    public static int WriteUInt8Array(this Stream stream, ReadOnlySpan<byte> values)
    {
        if (values.Length == 0)
        {
            return 0;
        }

        stream.Write(values);
        return values.Length;
    }

    public static int WriteInt8(this Stream stream, sbyte value)
    {
        stream.WriteByte((byte)value);
        return 1;
    }

    public static int WriteInt8Array(this Stream stream, ReadOnlySpan<sbyte> values)
    {
        if (values.Length == 0)
        {
            return 0;
        }

        stream.Write(MemoryMarshal.Cast<sbyte, byte>(values));
        return values.Length;
    }

    public static int WriteUInt16(this Stream stream, ushort value, bool msByteFirst = false)
    {
        Span<byte> span = stackalloc byte[2];
        if (msByteFirst)
        {
            BinaryPrimitives.WriteUInt16BigEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt16LittleEndian(span, value);
        }
        return stream.WriteBytes(span);
    }

    public static int WriteUInt16Array(this Stream stream, ReadOnlySpan<ushort> values, bool msByteFirst = false)
    {
        if (values.Length == 0)
        {
            return 0;
        }

        var size = values.Length * 2;

        if (msByteFirst == !BitConverter.IsLittleEndian)
        {
            stream.Write(MemoryMarshal.Cast<ushort, byte>(values));
            return size;
        }

        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            var target = MemoryMarshal.Cast<byte, ushort>(buffer.AsSpan(0, size));
            for (var i = 0; i < values.Length; i++)
            {
                target[i] = BinaryPrimitives.ReverseEndianness(values[i]);
            }

            stream.Write(buffer, 0, size);
            return size;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static int WriteInt16(this Stream stream, short value, bool msByteFirst = false)
    {
        Span<byte> span = stackalloc byte[2];
        if (msByteFirst)
        {
            BinaryPrimitives.WriteInt16BigEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteInt16LittleEndian(span, value);
        }
        return stream.WriteBytes(span);
    }

    public static int WriteInt16Array(this Stream stream, ReadOnlySpan<short> values, bool msByteFirst = false)
    {
        if (values.Length == 0)
        {
            return 0;
        }

        var size = values.Length * 2;

        if (msByteFirst == !BitConverter.IsLittleEndian)
        {
            stream.Write(MemoryMarshal.Cast<short, byte>(values));
            return size;
        }

        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            var target = MemoryMarshal.Cast<byte, short>(buffer.AsSpan(0, size));
            for (var i = 0; i < values.Length; i++)
            {
                target[i] = BinaryPrimitives.ReverseEndianness(values[i]);
            }

            stream.Write(buffer, 0, size);
            return size;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static int WriteUInt32(this Stream stream, uint value, bool msByteFirst = false)
    {
        Span<byte> span = stackalloc byte[4];
        if (msByteFirst)
        {
            BinaryPrimitives.WriteUInt32BigEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt32LittleEndian(span, value);
        }
        return stream.WriteBytes(span);
    }

    public static int WriteUInt32Array(this Stream stream, ReadOnlySpan<uint> values, bool msByteFirst = false)
    {
        if (values.Length == 0)
        {
            return 0;
        }

        var size = values.Length * 4;

        if (msByteFirst == !BitConverter.IsLittleEndian)
        {
            stream.Write(MemoryMarshal.Cast<uint, byte>(values));
            return size;
        }

        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            var target = MemoryMarshal.Cast<byte, uint>(buffer.AsSpan(0, size));
            for (var i = 0; i < values.Length; i++)
            {
                target[i] = BinaryPrimitives.ReverseEndianness(values[i]);
            }

            stream.Write(buffer, 0, size);
            return size;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static int WriteInt32(this Stream stream, int value, bool msByteFirst = false)
    {
        Span<byte> span = stackalloc byte[4];
        if (msByteFirst)
        {
            BinaryPrimitives.WriteInt32BigEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteInt32LittleEndian(span, value);
        }
        return stream.WriteBytes(span);
    }

    public static int WriteInt32Array(this Stream stream, ReadOnlySpan<int> values, bool msByteFirst = false)
    {
        if (values.Length == 0)
        {
            return 0;
        }

        var size = values.Length * 4;

        if (msByteFirst == !BitConverter.IsLittleEndian)
        {
            stream.Write(MemoryMarshal.Cast<int, byte>(values));
            return size;
        }

        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            var target = MemoryMarshal.Cast<byte, int>(buffer.AsSpan(0, size));
            for (var i = 0; i < values.Length; i++)
            {
                target[i] = BinaryPrimitives.ReverseEndianness(values[i]);
            }

            stream.Write(buffer, 0, size);
            return size;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static int WriteString(this Stream stream, string value)
    {
        if (value.Length == 0)
        {
            stream.WriteByte(0);
            return 1;
        }

        var buffer = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(value.Length));
        try
        {
            var size = Encoding.UTF8.GetBytes(value, buffer);
            stream.Write(buffer, 0, size);
            stream.WriteByte(0);
            return size + 1;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static int WriteBool(this Stream stream, bool value) => stream.WriteUInt8(value ? (byte)1 : (byte)0);

    public static int WriteNulls(this Stream stream, int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(size);
        if (size == 0)
        {
            return 0;
        }

        if (size <= 64)
        {
            Span<byte> span = stackalloc byte[size];
            span.Clear();
            stream.Write(span);
            return size;
        }

        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            Array.Clear(buffer, 0, size);
            stream.Write(buffer, 0, size);
            return size;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static int AlignTo4Bytes(this Stream stream) => stream.WriteNulls((int)(3 - (stream.Position + 3) % 4));
}
