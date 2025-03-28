namespace PcfSpec.Tests;

public class TableFormatTests
{
    [Fact]
    public void TestValue()
    {
        Assert.Equal(0u, new PcfTableFormat().Value);
        Assert.Equal(2u, new PcfTableFormat(glyphPadIndex: 2).Value);
        Assert.Equal(12u, new PcfTableFormat(msByteFirst: true, msBitFirst: true).Value);
        Assert.Equal(14u, new PcfTableFormat(msByteFirst: true, msBitFirst: true, glyphPadIndex: 2).Value);
        Assert.Equal(258u, new PcfTableFormat(inkBoundsOrCompressedMetrics: true, glyphPadIndex: 2).Value);
        Assert.Equal(270u, new PcfTableFormat(msByteFirst: true, msBitFirst: true, inkBoundsOrCompressedMetrics: true, glyphPadIndex: 2).Value);
    }

    [Fact]
    public void TestParse()
    {
        var tableFormat = PcfTableFormat.Parse(270u);
        Assert.True(tableFormat.MsByteFirst);
        Assert.True(tableFormat.MsBitFirst);
        Assert.True(tableFormat.InkBoundsOrCompressedMetrics);
        Assert.Equal(2, tableFormat.GlyphPadIndex);
        Assert.Equal(0, tableFormat.ScanUnitIndex);
    }

    [Fact]
    public void TestEquals()
    {
        var tableFormat1 = new PcfTableFormat(msByteFirst: true);
        var tableFormat2 = new PcfTableFormat(msByteFirst: true);
        var tableFormat3 = new PcfTableFormat(glyphPadIndex: 2);
        Assert.Equal(tableFormat1.Value, tableFormat2.Value);
        Assert.NotEqual(tableFormat1.Value, tableFormat3.Value);
    }
}
