namespace PcfSpec.Tests;

public class PcfTableFormatTests
{
    [Fact]
    public void TestValue1()
    {
        var tableFormat = PcfTableFormat.Create();
        Assert.Equal(PcfTableFormat.Default, tableFormat);
        Assert.False(tableFormat.MsByteFirst);
        Assert.False(tableFormat.MsBitFirst);
        Assert.False(tableFormat.InkBounds);
        Assert.False(tableFormat.CompressedMetrics);
        Assert.Equal(0, tableFormat.GlyphPadIndex);
        Assert.Equal(1u, tableFormat.GlyphPad);
        Assert.Equal(0, tableFormat.ScanUnitIndex);
        Assert.Equal(1u, tableFormat.ScanUnit);
    }

    [Fact]
    public void TestValue2()
    {
        var tableFormat = PcfTableFormat.Create(
            msByteFirst: true,
            msBitFirst: true,
            inkBoundsOrCompressedMetrics: true,
            glyphPad: 2,
            scanUnit: 4);
        Assert.Equal(new PcfTableFormat(0b_01_00_10_11_01), tableFormat);
        Assert.True(tableFormat.MsByteFirst);
        Assert.True(tableFormat.MsBitFirst);
        Assert.True(tableFormat.InkBounds);
        Assert.True(tableFormat.CompressedMetrics);
        Assert.Equal(1, tableFormat.GlyphPadIndex);
        Assert.Equal(2u, tableFormat.GlyphPad);
        Assert.Equal(2, tableFormat.ScanUnitIndex);
        Assert.Equal(4u, tableFormat.ScanUnit);
    }

    [Fact]
    public void TestMsByteFirst()
    {
        var tableFormat = PcfTableFormat.Default;
        Assert.False(tableFormat.MsByteFirst);

        tableFormat = tableFormat.With(msByteFirst: true);
        Assert.True(tableFormat.MsByteFirst);

        tableFormat = tableFormat.With(msByteFirst: false);
        Assert.False(tableFormat.MsByteFirst);
    }

    [Fact]
    public void TestMsBitFirst()
    {
        var tableFormat = PcfTableFormat.Default;
        Assert.False(tableFormat.MsBitFirst);

        tableFormat = tableFormat.With(msBitFirst: true);
        Assert.True(tableFormat.MsBitFirst);

        tableFormat = tableFormat.With(msBitFirst: false);
        Assert.False(tableFormat.MsBitFirst);
    }

    [Fact]
    public void TestInkBoundsOrCompressedMetrics()
    {
        var tableFormat = PcfTableFormat.Default;
        Assert.False(tableFormat.InkBounds);
        Assert.False(tableFormat.CompressedMetrics);

        tableFormat = tableFormat.With(inkBoundsOrCompressedMetrics: true);
        Assert.True(tableFormat.InkBounds);
        Assert.True(tableFormat.CompressedMetrics);

        tableFormat = tableFormat.With(inkBoundsOrCompressedMetrics: false);
        Assert.False(tableFormat.InkBounds);
        Assert.False(tableFormat.CompressedMetrics);
    }

    [Fact]
    public void TestGlyphPad()
    {
        var tableFormat = PcfTableFormat.Default;
        Assert.Equal(1u, tableFormat.GlyphPad);
        Assert.Equal(0, tableFormat.GlyphPadIndex);

        tableFormat = tableFormat.With(glyphPad: 2);
        Assert.Equal(2u, tableFormat.GlyphPad);
        Assert.Equal(1, tableFormat.GlyphPadIndex);

        tableFormat = tableFormat.With(glyphPad: 4);
        Assert.Equal(4u, tableFormat.GlyphPad);
        Assert.Equal(2, tableFormat.GlyphPadIndex);

        tableFormat = tableFormat.With(glyphPad: 8);
        Assert.Equal(8u, tableFormat.GlyphPad);
        Assert.Equal(3, tableFormat.GlyphPadIndex);

        tableFormat = tableFormat.With(glyphPad: 1);
        Assert.Equal(1u, tableFormat.GlyphPad);
        Assert.Equal(0, tableFormat.GlyphPadIndex);

        var e = Assert.Throws<ArgumentException>(() => tableFormat.With(glyphPad: 16));
        Assert.Equal("glyphPad", e.ParamName);
    }

    [Fact]
    public void TestScanUnit()
    {
        var tableFormat = PcfTableFormat.Default;
        Assert.Equal(1u, tableFormat.ScanUnit);
        Assert.Equal(0, tableFormat.ScanUnitIndex);

        tableFormat = tableFormat.With(scanUnit: 2);
        Assert.Equal(2u, tableFormat.ScanUnit);
        Assert.Equal(1, tableFormat.ScanUnitIndex);

        tableFormat = tableFormat.With(scanUnit: 4);
        Assert.Equal(4u, tableFormat.ScanUnit);
        Assert.Equal(2, tableFormat.ScanUnitIndex);

        tableFormat = tableFormat.With(scanUnit: 1);
        Assert.Equal(1u, tableFormat.ScanUnit);
        Assert.Equal(0, tableFormat.ScanUnitIndex);

        var e = Assert.Throws<ArgumentException>(() => tableFormat.With(scanUnit: 8));
        Assert.Equal("scanUnit", e.ParamName);
    }

    [Fact]
    public void TestEquals()
    {
        var tableFormat1 = PcfTableFormat.Create(
            msByteFirst: true,
            msBitFirst: true,
            inkBoundsOrCompressedMetrics: true,
            glyphPad: 2,
            scanUnit: 4);
        var tableFormat2 = PcfTableFormat.Create(
            msByteFirst: true,
            msBitFirst: true,
            inkBoundsOrCompressedMetrics: true,
            glyphPad: 2,
            scanUnit: 4);
        Assert.Equal(tableFormat1, tableFormat2);
        Assert.Equal(tableFormat1.GetHashCode(), tableFormat2.GetHashCode());
    }
}
