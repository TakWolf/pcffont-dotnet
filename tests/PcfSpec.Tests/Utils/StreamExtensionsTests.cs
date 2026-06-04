using PcfSpec.Utils;

namespace PcfSpec.Tests.Utils;

public class StreamExtensionsTests
{
    [Fact]
    public void TestBytes()
    {
        using var stream = new MemoryStream();
        Assert.Equal(11, stream.WriteBytes("Hello World"u8));
        Assert.Equal(11, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal("Hello World"u8, stream.ReadBytes(11));
        Assert.Equal(11, stream.Position);
    }

    [Fact]
    public void TestEof()
    {
        using var stream = new MemoryStream();
        stream.WriteBytes("ABC"u8);
        Assert.Throws<EndOfStreamException>(() => stream.ReadBytes(4));
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal("ABC"u8, stream.ReadBytes(4, throwOnEndOfStream: false));
    }

    [Fact]
    public void TestUInt8()
    {
        using var stream = new MemoryStream();
        Assert.Equal(1, stream.WriteUInt8(0x00));
        Assert.Equal(1, stream.WriteUInt8(0xFF));
        Assert.Equal(2, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(0x00, stream.ReadUInt8());
        Assert.Equal(0xFF, stream.ReadUInt8());
        Assert.Equal(2, stream.Position);
    }

    [Fact]
    public void TestUInt8Array()
    {
        using var stream = new MemoryStream();
        Assert.Equal(2, stream.WriteUInt8Array([0x00, 0xFF]));
        Assert.Equal(2, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal([0x00, 0xFF], stream.ReadUInt8Array(2));
        Assert.Equal(2, stream.Position);
    }

    [Fact]
    public void TestInt8()
    {
        using var stream = new MemoryStream();
        Assert.Equal(1, stream.WriteInt8(-0x80));
        Assert.Equal(1, stream.WriteInt8(0x7F));
        Assert.Equal(2, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(-0x80, stream.ReadInt8());
        Assert.Equal(0x7F, stream.ReadInt8());
        Assert.Equal(2, stream.Position);
    }

    [Fact]
    public void TestInt8Array()
    {
        using var stream = new MemoryStream();
        Assert.Equal(2, stream.WriteInt8Array([-0x80, 0x7F]));
        Assert.Equal(2, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal([-0x80, 0x7F], stream.ReadInt8Array(2));
        Assert.Equal(2, stream.Position);
    }

    [Fact]
    public void TestUInt16()
    {
        using var stream = new MemoryStream();
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
    public void TestUInt16Array()
    {
        using var stream = new MemoryStream();
        Assert.Equal(4, stream.WriteUInt16Array([0x0000, 0xFFFF], false));
        Assert.Equal(4, stream.WriteUInt16Array([0x0000, 0xFFFF], true));
        Assert.Equal(8, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal([0x0000, 0xFFFF], stream.ReadUInt16Array(2, false));
        Assert.Equal([0x0000, 0xFFFF], stream.ReadUInt16Array(2, true));
        Assert.Equal(8, stream.Position);
    }

    [Fact]
    public void TestInt16()
    {
        using var stream = new MemoryStream();
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
    public void TestInt16Array()
    {
        using var stream = new MemoryStream();
        Assert.Equal(4, stream.WriteInt16Array([-0x8000, 0x7FFF], false));
        Assert.Equal(4, stream.WriteInt16Array([-0x8000, 0x7FFF], true));
        Assert.Equal(8, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal([-0x8000, 0x7FFF], stream.ReadInt16Array(2, false));
        Assert.Equal([-0x8000, 0x7FFF], stream.ReadInt16Array(2, true));
        Assert.Equal(8, stream.Position);
    }

    [Fact]
    public void TestUInt32()
    {
        using var stream = new MemoryStream();
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
    public void TestUInt32Array()
    {
        using var stream = new MemoryStream();
        Assert.Equal(8, stream.WriteUInt32Array([0x00000000u, 0xFFFFFFFFu], false));
        Assert.Equal(8, stream.WriteUInt32Array([0x00000000u, 0xFFFFFFFFu], true));
        Assert.Equal(16, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal([0x00000000u, 0xFFFFFFFFu], stream.ReadUInt32Array(2, false));
        Assert.Equal([0x00000000u, 0xFFFFFFFFu], stream.ReadUInt32Array(2, true));
        Assert.Equal(16, stream.Position);
    }

    [Fact]
    public void TestInt32()
    {
        using var stream = new MemoryStream();
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
    public void TestInt32Array()
    {
        using var stream = new MemoryStream();
        Assert.Equal(8, stream.WriteInt32Array([-0x80000000, 0x7FFFFFFF], false));
        Assert.Equal(8, stream.WriteInt32Array([-0x80000000, 0x7FFFFFFF], true));
        Assert.Equal(16, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal([-0x80000000, 0x7FFFFFFF], stream.ReadInt32Array(2, false));
        Assert.Equal([-0x80000000, 0x7FFFFFFF], stream.ReadInt32Array(2, true));
        Assert.Equal(16, stream.Position);
    }

    [Fact]
    public void TestString()
    {
        using var stream = new MemoryStream();
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
        using var stream = new MemoryStream();
        Assert.Equal(1, stream.WriteBool(true));
        Assert.Equal(1, stream.WriteBool(false));
        Assert.Equal(2, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.True(stream.ReadBool());
        Assert.False(stream.ReadBool());
        Assert.Equal(2, stream.Position);
    }

    [Fact]
    public void TestAlignTo4Bytes()
    {
        using var stream = new MemoryStream();
        stream.WriteBytes("abc"u8);
        Assert.Equal(1, stream.AlignTo4Bytes());
        Assert.Equal(4, stream.Position);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal("abc\0"u8, stream.ReadBytes(4));
    }
}
