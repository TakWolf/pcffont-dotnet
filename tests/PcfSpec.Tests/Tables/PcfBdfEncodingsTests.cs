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
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            defaultChar: 1);
        var encodings2 = encodings1.Copy();

        Assert.Equal(encodings1, encodings2);
        Assert.NotSame(encodings1, encodings2);
        Assert.Same(encodings1.TableFormat, encodings2.TableFormat);
    }

    [Fact]
    public void TestDeepCopy()
    {
        var encodings1 = new PcfBdfEncodings(
            new Dictionary<ushort, ushort>
            {
                { 1, 1 },
                { 2, 2 },
                { 3, 3 }
            },
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            defaultChar: 1);
        var encodings2 = encodings1.DeepCopy();

        Assert.Equal(encodings1, encodings2);
        Assert.NotSame(encodings1, encodings2);
        Assert.NotSame(encodings1.TableFormat, encodings2.TableFormat);
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
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            defaultChar: 1);
        var encodings2 = new PcfBdfEncodings(
            new Dictionary<ushort, ushort>
            {
                { 1, 1 },
                { 2, 2 },
                { 3, 3 }
            },
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            defaultChar: 1);
        Assert.Equal(encodings1, encodings2);
    }
}
