using PcfSpec.Tables;

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
