namespace PcfSpec.Tests;

public class PcfTableFormatTests
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
        Assert.Equal(4u, tableFormat.GlyphPad);
        Assert.Equal(1u, tableFormat.ScanUnit);
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
    public void TestGlyphPad1()
    {
        var tableFormat = new PcfTableFormat(glyphPadIndex: -1);
        tableFormat.GlyphPadIndex = 0;
        Assert.Equal(1u, tableFormat.GlyphPad);
        tableFormat.GlyphPadIndex = 1;
        Assert.Equal(2u, tableFormat.GlyphPad);
        tableFormat.GlyphPadIndex = 2;
        Assert.Equal(4u, tableFormat.GlyphPad);
        tableFormat.GlyphPadIndex = 3;
        Assert.Equal(8u, tableFormat.GlyphPad);

        tableFormat.GlyphPadIndex = 4;
        Assert.Throws<IndexOutOfRangeException>(() => tableFormat.GlyphPad);
    }

    [Fact]
    public void TestGlyphPad2()
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
        Assert.Equal("glyphPad", e.ParamName);
    }

    [Fact]
    public void TestScanUnit1()
    {
        var tableFormat = new PcfTableFormat(scanUnitIndex: -1);
        tableFormat.ScanUnitIndex = 0;
        Assert.Equal(1u, tableFormat.ScanUnit);
        tableFormat.ScanUnitIndex = 1;
        Assert.Equal(2u, tableFormat.ScanUnit);
        tableFormat.ScanUnitIndex = 2;
        Assert.Equal(4u, tableFormat.ScanUnit);

        tableFormat.ScanUnitIndex = 3;
        Assert.Throws<IndexOutOfRangeException>(() => tableFormat.ScanUnit);
    }

    [Fact]
    public void TestScanUnit2()
    {
        var tableFormat = new PcfTableFormat(scanUnitIndex: -1);
        tableFormat.ScanUnit = 1;
        Assert.Equal(0, tableFormat.ScanUnitIndex);
        tableFormat.ScanUnit = 2;
        Assert.Equal(1, tableFormat.ScanUnitIndex);
        tableFormat.ScanUnit = 4;
        Assert.Equal(2, tableFormat.ScanUnitIndex);

        var e = Assert.Throws<ArgumentException>(() => tableFormat.ScanUnit = 8);
        Assert.Equal("scanUnit", e.ParamName);
    }

    [Fact]
    public void TestCopy()
    {
        var format1 = new PcfTableFormat(
            msByteFirst: true,
            msBitFirst: true,
            inkBoundsOrCompressedMetrics: true,
            glyphPadIndex: 1,
            scanUnitIndex: 2
        );
        var format2 = format1.DeepCopy();
        Assert.Equal(format1.Value, format2.Value);
        Assert.NotSame(format1, format2);
    }
}
