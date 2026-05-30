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
        Assert.True(tableFormat.InkBounds);
        Assert.True(tableFormat.CompressedMetrics);
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

    [Fact]
    public void TestGlyphPad()
    {
        var tableFormat = new PcfTableFormat(glyphPadIndex: -1);
        tableFormat.GlyphPad = 1;
        Assert.Equal(0, tableFormat.GlyphPadIndex);
        tableFormat.GlyphPad = 2;
        Assert.Equal(1, tableFormat.GlyphPadIndex);
        tableFormat.GlyphPad = 4;
        Assert.Equal(2, tableFormat.GlyphPadIndex);
        tableFormat.GlyphPad = 8;
        Assert.Equal(3, tableFormat.GlyphPadIndex);

        var e = Assert.Throws<ArgumentException>(() => tableFormat.GlyphPad = 16);
        Assert.Equal(nameof(PcfTableFormat.GlyphPad), e.ParamName);
        Assert.Equal($"{nameof(PcfTableFormat.GlyphPad)} must be one of [1, 2, 4, 8]. (Parameter '{nameof(PcfTableFormat.GlyphPad)}')", e.Message);
    }

    [Fact]
    public void TestScanUnit()
    {
        var tableFormat = new PcfTableFormat(scanUnitIndex: -1);
        tableFormat.ScanUnit = 1;
        Assert.Equal(0, tableFormat.ScanUnitIndex);
        tableFormat.ScanUnit = 2;
        Assert.Equal(1, tableFormat.ScanUnitIndex);
        tableFormat.ScanUnit = 4;
        Assert.Equal(2, tableFormat.ScanUnitIndex);

        var e = Assert.Throws<ArgumentException>(() => tableFormat.ScanUnit = 8);
        Assert.Equal(nameof(PcfTableFormat.ScanUnit), e.ParamName);
        Assert.Equal($"{nameof(PcfTableFormat.ScanUnit)} must be one of [1, 2, 4]. (Parameter '{nameof(PcfTableFormat.ScanUnit)}')", e.Message);
    }

    [Fact]
    public void TestBitmapsSizeConfigs()
    {
        var tableFormat = new PcfTableFormat();
        tableFormat.GlyphPad = 1;
        Assert.Equal([16, 32, 64, 128], tableFormat.BitmapsSizeConfigs(16));
        tableFormat.GlyphPad = 2;
        Assert.Equal([8, 16, 32, 64], tableFormat.BitmapsSizeConfigs(16));
        tableFormat.GlyphPad = 4;
        Assert.Equal([4, 8, 16, 32], tableFormat.BitmapsSizeConfigs(16));
        tableFormat.GlyphPad = 8;
        Assert.Equal([2, 4, 8, 16], tableFormat.BitmapsSizeConfigs(16));
    }
}
