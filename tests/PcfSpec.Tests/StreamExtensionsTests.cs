using PcfSpec.Internal;

namespace PcfSpec.Tests;

public class StreamExtensionsTests
{
    private static readonly Random Random = new();

    [Fact]
    public void TestByte()
    {
        var stream = new MemoryStream();
        var count = 0;
        count += stream.WriteBuffer("Hello World"u8);
        count += stream.WriteNulls(4);
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal("Hello World"u8, stream.ReadBuffer(11));
        Assert.Equal(new byte[4], stream.ReadBuffer(4));
        Assert.Equal(count, stream.Position);
    }

    [Fact]
    public void TestEof()
    {
        var stream = new MemoryStream();
        Assert.Throws<EndOfStreamException>(() => stream.ReadBuffer(1));
        stream.ReadBuffer(1, throwOnEndOfStream: false);
    }

    [Fact]
    public void TestInt8()
    {
        var values = Enumerable.Range(0, 20).Select(_ => (sbyte)Random.Next()).ToList();

        var stream = new MemoryStream();
        var count = 0;
        foreach (var value in values)
        {
            count += stream.WriteInt8(value);
        }
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(values, stream.ReadInt8List(values.Count));
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = stream.WriteInt8List(values);
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        foreach (var value in values)
        {
            Assert.Equal(value, stream.ReadInt8());
        }
        Assert.Equal(count, stream.Position);
    }

    [Fact]
    public void TestUInt8()
    {
        var values = Enumerable.Range(0, 20).Select(_ => (byte)Random.Next()).ToList();

        var stream = new MemoryStream();
        var count = 0;
        foreach (var value in values)
        {
            count += stream.WriteUInt8(value);
        }
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(values, stream.ReadUInt8List(values.Count));
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = stream.WriteUInt8List(values);
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        foreach (var value in values)
        {
            Assert.Equal(value, stream.ReadUInt8());
        }
        Assert.Equal(count, stream.Position);
    }

    [Fact]
    public void TestInt16()
    {
        var values = Enumerable.Range(0, 20).Select(_ => (short)Random.Next()).ToList();

        var stream = new MemoryStream();
        var count = 0;
        foreach (var value in values)
        {
            count += stream.WriteInt16(value, true);
        }
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(values, stream.ReadInt16List(values.Count, true));
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = stream.WriteInt16List(values, true);
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        foreach (var value in values)
        {
            Assert.Equal(value, stream.ReadInt16(true));
        }
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = 0;
        foreach (var value in values)
        {
            count += stream.WriteInt16(value, false);
        }
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(values, stream.ReadInt16List(values.Count, false));
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = stream.WriteInt16List(values, false);
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        foreach (var value in values)
        {
            Assert.Equal(value, stream.ReadInt16(false));
        }
        Assert.Equal(count, stream.Position);
    }

    [Fact]
    public void TestUInt16()
    {
        var values = Enumerable.Range(0, 20).Select(_ => (ushort)Random.Next()).ToList();

        var stream = new MemoryStream();
        var count = 0;
        foreach (var value in values)
        {
            count += stream.WriteUInt16(value, true);
        }
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(values, stream.ReadUInt16List(values.Count, true));
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = stream.WriteUInt16List(values, true);
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        foreach (var value in values)
        {
            Assert.Equal(value, stream.ReadUInt16(true));
        }
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = 0;
        foreach (var value in values)
        {
            count += stream.WriteUInt16(value, false);
        }
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(values, stream.ReadUInt16List(values.Count, false));
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = stream.WriteUInt16List(values, false);
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        foreach (var value in values)
        {
            Assert.Equal(value, stream.ReadUInt16(false));
        }
        Assert.Equal(count, stream.Position);
    }

    [Fact]
    public void TestInt32()
    {
        var values = Enumerable.Range(0, 20).Select(_ => Random.Next()).ToList();

        var stream = new MemoryStream();
        var count = 0;
        foreach (var value in values)
        {
            count += stream.WriteInt32(value, true);
        }
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(values, stream.ReadInt32List(values.Count, true));
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = stream.WriteInt32List(values, true);
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        foreach (var value in values)
        {
            Assert.Equal(value, stream.ReadInt32(true));
        }
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = 0;
        foreach (var value in values)
        {
            count += stream.WriteInt32(value, false);
        }
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(values, stream.ReadInt32List(values.Count, false));
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = stream.WriteInt32List(values, false);
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        foreach (var value in values)
        {
            Assert.Equal(value, stream.ReadInt32(false));
        }
        Assert.Equal(count, stream.Position);
    }

    [Fact]
    public void TestUInt32()
    {
        var values = Enumerable.Range(0, 20).Select(_ => (uint)Random.Next()).ToList();

        var stream = new MemoryStream();
        var count = 0;
        foreach (var value in values)
        {
            count += stream.WriteUInt32(value, true);
        }
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(values, stream.ReadUInt32List(values.Count, true));
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = stream.WriteUInt32List(values, true);
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        foreach (var value in values)
        {
            Assert.Equal(value, stream.ReadUInt32(true));
        }
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = 0;
        foreach (var value in values)
        {
            count += stream.WriteUInt32(value, false);
        }
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(values, stream.ReadUInt32List(values.Count, false));
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = stream.WriteUInt32List(values, false);
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        foreach (var value in values)
        {
            Assert.Equal(value, stream.ReadUInt32(false));
        }
        Assert.Equal(count, stream.Position);
    }

    [Fact]
    public void TestBinary()
    {
        var stream = new MemoryStream();
        var count = 0;
        count += stream.WriteBinary([1, 1, 1, 1, 0, 0, 0, 0], true);
        count += stream.WriteBinary([1, 1, 1, 1, 0, 0, 0, 0], false);
        count += stream.WriteBinaryList([
            [1, 1, 1, 1, 0, 0, 0, 0],
            [0, 0, 0, 0, 1, 1, 1, 1]
        ], true);
        count += stream.WriteBinaryList([
            [1, 1, 1, 1, 0, 0, 0, 0],
            [0, 0, 0, 0, 1, 1, 1, 1]
        ], false);
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal([1, 1, 1, 1, 0, 0, 0, 0], stream.ReadBinary(true));
        Assert.Equal([1, 1, 1, 1, 0, 0, 0, 0], stream.ReadBinary(false));
        Assert.Equal([
            [1, 1, 1, 1, 0, 0, 0, 0],
            [0, 0, 0, 0, 1, 1, 1, 1]
        ], stream.ReadBinaryList(2, true));
        Assert.Equal([
            [1, 1, 1, 1, 0, 0, 0, 0],
            [0, 0, 0, 0, 1, 1, 1, 1]
        ], stream.ReadBinaryList(2, false));
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal([0, 0, 0, 0, 1, 1, 1, 1], stream.ReadBinary(false));
        Assert.Equal([0, 0, 0, 0, 1, 1, 1, 1], stream.ReadBinary(true));
        Assert.Equal([
            [0, 0, 0, 0, 1, 1, 1, 1],
            [1, 1, 1, 1, 0, 0, 0, 0]
        ], stream.ReadBinaryList(2, false));
        Assert.Equal([
            [0, 0, 0, 0, 1, 1, 1, 1],
            [1, 1, 1, 1, 0, 0, 0, 0]
        ], stream.ReadBinaryList(2, true));
        Assert.Equal(count, stream.Position);
    }

    [Fact]
    public void TestString()
    {
        List<string> values = ["ABC", "DEF", "12345", "67890"];

        var stream = new MemoryStream();
        var count = 0;
        foreach (var value in values)
        {
            count += stream.WriteString(value);
        }
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(values, stream.ReadStringList(values.Count));
        Assert.Equal(count, stream.Position);

        stream = new MemoryStream();
        count = stream.WriteStringList(values);
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        foreach (var value in values)
        {
            Assert.Equal(value, stream.ReadString());
        }
        Assert.Equal(count, stream.Position);
    }

    [Fact]
    public void TestBool()
    {
        var stream = new MemoryStream();
        var count = 0;
        count += stream.WriteBool(true);
        count += stream.WriteBool(false);
        Assert.Equal(count, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.True(stream.ReadBool());
        Assert.False(stream.ReadBool());
        Assert.Equal(count, stream.Position);
    }

    [Fact]
    public void TestAlignTo4Byte()
    {
        var stream = new MemoryStream();
        stream.WriteBuffer("abc"u8);
        Assert.Equal(1, stream.AlignTo4ByteWithNulls());
        Assert.Equal(4, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal("abc\0"u8, stream.ReadBuffer(4));
    }
}
