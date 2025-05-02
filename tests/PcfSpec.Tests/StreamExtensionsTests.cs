using PcfSpec.Util;

namespace PcfSpec.Tests;

public class StreamExtensionsTests
{
    [Fact]
    public void TestBytes()
    {
        var stream = new MemoryStream();
        Assert.Equal(11, stream.WriteBytes("Hello World"u8));
        Assert.Equal(11, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal("Hello World"u8, stream.ReadBytes(11));
        Assert.Equal(11, stream.Position);
    }

    [Fact]
    public void TestEof()
    {
        var stream = new MemoryStream();
        stream.WriteBytes("ABC"u8);
        Assert.Throws<EndOfStreamException>(() => stream.ReadBytes(4));
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal("ABC"u8, stream.ReadBytes(4, throwOnEndOfStream: false));
    }

    [Fact]
    public void TestUInt8()
    {
        var stream = new MemoryStream();
        Assert.Equal(1, stream.WriteUInt8(0x00));
        Assert.Equal(1, stream.WriteUInt8(0xFF));
        Assert.Equal(2, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(0x00, stream.ReadUInt8());
        Assert.Equal(0xFF, stream.ReadUInt8());
        Assert.Equal(2, stream.Position);
    }

    [Fact]
    public void TestInt8()
    {
        var stream = new MemoryStream();
        Assert.Equal(1, stream.WriteInt8(-0x80));
        Assert.Equal(1, stream.WriteInt8(0x7F));
        Assert.Equal(2, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(-0x80, stream.ReadInt8());
        Assert.Equal(0x7F, stream.ReadInt8());
        Assert.Equal(2, stream.Position);
    }

    [Fact]
    public void TestUInt16()
    {
        var stream = new MemoryStream();
        Assert.Equal(2, stream.WriteUInt16(0x0000, false));
        Assert.Equal(2, stream.WriteUInt16(0xFFFF, false));
        Assert.Equal(2, stream.WriteUInt16(0x0000, true));
        Assert.Equal(2, stream.WriteUInt16(0xFFFF, true));
        Assert.Equal(8, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(0x0000, stream.ReadUInt16(false));
        Assert.Equal(0xFFFF, stream.ReadUInt16(false));
        Assert.Equal(0x0000, stream.ReadUInt16(true));
        Assert.Equal(0xFFFF, stream.ReadUInt16(true));
        Assert.Equal(8, stream.Position);
    }

    [Fact]
    public void TestInt16()
    {
        var stream = new MemoryStream();
        Assert.Equal(2, stream.WriteInt16(-0x8000, false));
        Assert.Equal(2, stream.WriteInt16(0x7FFF, false));
        Assert.Equal(2, stream.WriteInt16(-0x8000, true));
        Assert.Equal(2, stream.WriteInt16(0x7FFF, true));
        Assert.Equal(8, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(-0x8000, stream.ReadInt16(false));
        Assert.Equal(0x7FFF, stream.ReadInt16(false));
        Assert.Equal(-0x8000, stream.ReadInt16(true));
        Assert.Equal(0x7FFF, stream.ReadInt16(true));
        Assert.Equal(8, stream.Position);
    }

    [Fact]
    public void TestUInt32()
    {
        var stream = new MemoryStream();
        Assert.Equal(4, stream.WriteUInt32(0x00000000u, false));
        Assert.Equal(4, stream.WriteUInt32(0xFFFFFFFFu, false));
        Assert.Equal(4, stream.WriteUInt32(0x00000000u, true));
        Assert.Equal(4, stream.WriteUInt32(0xFFFFFFFFu, true));
        Assert.Equal(16, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(0x00000000u, stream.ReadUInt32(false));
        Assert.Equal(0xFFFFFFFFu, stream.ReadUInt32(false));
        Assert.Equal(0x00000000u, stream.ReadUInt32(true));
        Assert.Equal(0xFFFFFFFFu, stream.ReadUInt32(true));
        Assert.Equal(16, stream.Position);
    }

    [Fact]
    public void TestInt32()
    {
        var stream = new MemoryStream();
        Assert.Equal(4, stream.WriteInt32(-0x80000000, false));
        Assert.Equal(4, stream.WriteInt32(0x7FFFFFFF, false));
        Assert.Equal(4, stream.WriteInt32(-0x80000000, true));
        Assert.Equal(4, stream.WriteInt32(0x7FFFFFFF, true));
        Assert.Equal(16, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(-0x80000000, stream.ReadInt32(false));
        Assert.Equal(0x7FFFFFFF, stream.ReadInt32(false));
        Assert.Equal(-0x80000000, stream.ReadInt32(true));
        Assert.Equal(0x7FFFFFFF, stream.ReadInt32(true));
        Assert.Equal(16, stream.Position);
    }

    [Fact]
    public void TestBinary()
    {
        var stream = new MemoryStream();
        Assert.Equal(1, stream.WriteBinary([1, 1, 1, 1, 0, 0, 0, 0], false));
        Assert.Equal(1, stream.WriteBinary([1, 1, 1, 1, 0, 0, 0, 0], true));
        Assert.Equal(2, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal([1, 1, 1, 1, 0, 0, 0, 0], stream.ReadBinary(false));
        Assert.Equal([1, 1, 1, 1, 0, 0, 0, 0], stream.ReadBinary(true));
        Assert.Equal(2, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal([0, 0, 0, 0, 1, 1, 1, 1], stream.ReadBinary(true));
        Assert.Equal([0, 0, 0, 0, 1, 1, 1, 1], stream.ReadBinary(false));
        Assert.Equal(2, stream.Position);
    }

    [Fact]
    public void TestString()
    {
        var stream = new MemoryStream();
        Assert.Equal(4, stream.WriteString("ABC"));
        Assert.Equal(6, stream.WriteString("12345"));
        Assert.Equal(10, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal("ABC", stream.ReadString());
        Assert.Equal("12345", stream.ReadString());
        Assert.Equal(10, stream.Position);
    }

    [Fact]
    public void TestBool()
    {
        var stream = new MemoryStream();
        Assert.Equal(1, stream.WriteBool(true));
        Assert.Equal(1, stream.WriteBool(false));
        Assert.Equal(2, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.True(stream.ReadBool());
        Assert.False(stream.ReadBool());
        Assert.Equal(2, stream.Position);
    }

    [Fact]
    public void TestAlignTo4Byte()
    {
        var stream = new MemoryStream();
        stream.WriteBytes("abc"u8);
        Assert.Equal(1, stream.AlignTo4ByteWithNulls());
        Assert.Equal(4, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal("abc\0"u8, stream.ReadBytes(4));
    }
}
