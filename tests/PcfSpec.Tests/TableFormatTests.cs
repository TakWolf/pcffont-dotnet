namespace PcfSpec.Tests;

public class TableFormatTests
{
    [Fact]
    public void TestValue()
    {
        Assert.Equal(0u, new PcfTableFormat().Value);
        Assert.Equal(2u, new PcfTableFormat(glyphPadIndex: 2).Value);
        Assert.Equal(12u, new PcfTableFormat(isMsByteFirst: true, isMsBitFirst: true).Value);
        Assert.Equal(14u, new PcfTableFormat(isMsByteFirst: true, isMsBitFirst: true, glyphPadIndex: 2).Value);
        Assert.Equal(258u, new PcfTableFormat(isInkBoundsOrCompressedMetrics: true, glyphPadIndex: 2).Value);
        Assert.Equal(270u, new PcfTableFormat(isMsByteFirst: true, isMsBitFirst: true, isInkBoundsOrCompressedMetrics: true, glyphPadIndex: 2).Value);
    }

    [Fact]
    public void TestParse()
    {
        var tableFormat = PcfTableFormat.Parse(270u);
        Assert.True(tableFormat.IsMsByteFirst);
        Assert.True(tableFormat.IsMsBitFirst);
        Assert.True(tableFormat.IsInkBoundsOrCompressedMetrics);
        Assert.Equal(2, tableFormat.GlyphPadIndex);
        Assert.Equal(0, tableFormat.ScanUnitIndex);
    }
    
    [Fact]
    public void TestEqual()
    {
        var tableFormat1 = new PcfTableFormat(isMsByteFirst: true);
        var tableFormat2 = new PcfTableFormat(isMsByteFirst: true);
        var tableFormat3 = new PcfTableFormat(glyphPadIndex: 2);
        Assert.Equal(tableFormat1, tableFormat2);
        Assert.NotEqual(tableFormat1, tableFormat3);
    }
}
