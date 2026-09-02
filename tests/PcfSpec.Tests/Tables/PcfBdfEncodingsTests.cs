using PcfSpec.Errors;
using PcfSpec.Tables;
using PcfSpec.Utils;

namespace PcfSpec.Tests.Tables;

public class PcfBdfEncodingsTests
{
    [Fact]
    public void TestEncodings()
    {
        var encodings = new PcfBdfEncodings();

        encodings[1] = PcfBdfEncodings.NoGlyphIndex;
        Assert.Empty(encodings);
    }

    [Fact]
    public void TestEmptyDumpParse()
    {
        var font = new PcfFont();

        var encodings1 = new PcfBdfEncodings();
        using var stream = new MemoryStream();
        var tableSize = encodings1.Dump(stream, 0, font);
        var header = new PcfHeader(PcfTableType.BdfEncodings, encodings1.TableFormat, tableSize, 0);

        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(encodings1.TableFormat.Value, stream.ReadUInt32());
        Assert.Equal((ushort)0, stream.ReadUInt16());
        Assert.Equal((ushort)0, stream.ReadUInt16());
        Assert.Equal((ushort)0, stream.ReadUInt16());
        Assert.Equal((ushort)0, stream.ReadUInt16());
        Assert.Equal(PcfBdfEncodings.NoEncoding, stream.ReadUInt16());
        Assert.Equal(PcfBdfEncodings.NoGlyphIndex, stream.ReadUInt16());

        var encodings2 = PcfBdfEncodings.Parse(stream, header, font);
        Assert.Equal(encodings1, encodings2);
    }

    [Theory]
    [InlineData(1, 0, 0, 0)]
    [InlineData(0, 0, 1, 0)]
    [InlineData(0, 0x100, 0, 0)]
    [InlineData(0, 0, 0, 0x100)]
    public void TestParseInvalidRange(int minByte2, int maxByte2, int minByte1, int maxByte1)
    {
        using var stream = new MemoryStream();
        stream.WriteUInt32(PcfTableFormat.Default);
        stream.WriteUInt16((ushort)minByte2);
        stream.WriteUInt16((ushort)maxByte2);
        stream.WriteUInt16((ushort)minByte1);
        stream.WriteUInt16((ushort)maxByte1);
        stream.WriteUInt16(PcfBdfEncodings.NoEncoding);
        var header = new PcfHeader(PcfTableType.BdfEncodings, PcfTableFormat.Default, (uint)stream.Position, 0);

        Assert.Throws<PcfParseException>(() => PcfBdfEncodings.Parse(stream, header, new PcfFont()));
    }

    [Fact]
    public void TestCopy()
    {
        var encodings1 = new PcfBdfEncodings(
            new Dictionary<ushort, ushort>
            {
                { 1, 1 },
                { 2, 2 },
                { 3, 3 }
            },
            tableFormat: PcfTableFormat.Create(true, true, true, 2, 4),
            defaultChar: 1);
        var encodings2 = encodings1.Copy();
        var encodings3 = encodings1.DeepCopy();

        Assert.Equal(encodings1, encodings2);
        Assert.Equal(encodings1, encodings3);
        Assert.NotSame(encodings1, encodings2);
        Assert.NotSame(encodings1, encodings3);
    }

    [Fact]
    public void TestEquals()
    {
        var encodings1 = new PcfBdfEncodings(
            new Dictionary<ushort, ushort>
            {
                { 1, 1 },
                { 2, 2 },
                { 3, 3 }
            },
            tableFormat: PcfTableFormat.Create(true, true, true, 2, 4),
            defaultChar: 1);
        var encodings2 = new PcfBdfEncodings(
            new Dictionary<ushort, ushort>
            {
                { 1, 1 },
                { 2, 2 },
                { 3, 3 }
            },
            tableFormat: PcfTableFormat.Create(true, true, true, 2, 4),
            defaultChar: 1);
        Assert.Equal(encodings1, encodings2);
    }
}
