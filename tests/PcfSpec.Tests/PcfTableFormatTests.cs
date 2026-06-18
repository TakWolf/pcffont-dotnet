namespace PcfSpec.Tests;

public class PcfTableFormatTests
{
    [Fact]
    public void TestValue1()
    {
        var tableFormat = PcfTableFormat.Of();
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
        var tableFormat = PcfTableFormat.Of(
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

        tableFormat = tableFormat.WithMsByteFirst(true);
        Assert.True(tableFormat.MsByteFirst);

        tableFormat = tableFormat.WithMsByteFirst(false);
        Assert.False(tableFormat.MsByteFirst);
    }

    [Fact]
    public void TestMsBitFirst()
    {
        var tableFormat = PcfTableFormat.Default;
        Assert.False(tableFormat.MsBitFirst);

        tableFormat = tableFormat.WithMsBitFirst(true);
        Assert.True(tableFormat.MsBitFirst);

        tableFormat = tableFormat.WithMsBitFirst(false);
        Assert.False(tableFormat.MsBitFirst);
    }

    [Fact]
    public void TestInkBounds()
    {
        var tableFormat = PcfTableFormat.Default;
        Assert.False(tableFormat.InkBounds);

        tableFormat = tableFormat.WithInkBounds(true);
        Assert.True(tableFormat.InkBounds);

        tableFormat = tableFormat.WithInkBounds(false);
        Assert.False(tableFormat.InkBounds);
    }

    [Fact]
    public void TestCompressedMetrics()
    {
        var tableFormat = PcfTableFormat.Default;
        Assert.False(tableFormat.CompressedMetrics);

        tableFormat = tableFormat.WithCompressedMetrics(true);
        Assert.True(tableFormat.CompressedMetrics);

        tableFormat = tableFormat.WithCompressedMetrics(false);
        Assert.False(tableFormat.CompressedMetrics);
    }

    [Fact]
    public void TestGlyphPad1()
    {
        var tableFormat = PcfTableFormat.Default;
        Assert.Equal(0, tableFormat.GlyphPadIndex);
        Assert.Equal(1u, tableFormat.GlyphPad);

        tableFormat = tableFormat.WithGlyphPadIndex(1);
        Assert.Equal(1, tableFormat.GlyphPadIndex);
        Assert.Equal(2u, tableFormat.GlyphPad);

        tableFormat = tableFormat.WithGlyphPadIndex(2);
        Assert.Equal(2, tableFormat.GlyphPadIndex);
        Assert.Equal(4u, tableFormat.GlyphPad);

        tableFormat = tableFormat.WithGlyphPadIndex(3);
        Assert.Equal(3, tableFormat.GlyphPadIndex);
        Assert.Equal(8u, tableFormat.GlyphPad);

        tableFormat = tableFormat.WithGlyphPadIndex(0);
        Assert.Equal(0, tableFormat.GlyphPadIndex);
        Assert.Equal(1u, tableFormat.GlyphPad);

        Assert.Throws<ArgumentOutOfRangeException>(() => tableFormat.WithGlyphPadIndex(4));
    }

    [Fact]
    public void TestGlyphPad2()
    {
        var tableFormat = PcfTableFormat.Default;
        Assert.Equal(1u, tableFormat.GlyphPad);
        Assert.Equal(0, tableFormat.GlyphPadIndex);

        tableFormat = tableFormat.WithGlyphPad(2);
        Assert.Equal(2u, tableFormat.GlyphPad);
        Assert.Equal(1, tableFormat.GlyphPadIndex);

        tableFormat = tableFormat.WithGlyphPad(4);
        Assert.Equal(4u, tableFormat.GlyphPad);
        Assert.Equal(2, tableFormat.GlyphPadIndex);

        tableFormat = tableFormat.WithGlyphPad(8);
        Assert.Equal(8u, tableFormat.GlyphPad);
        Assert.Equal(3, tableFormat.GlyphPadIndex);

        tableFormat = tableFormat.WithGlyphPad(1);
        Assert.Equal(1u, tableFormat.GlyphPad);
        Assert.Equal(0, tableFormat.GlyphPadIndex);

        var e = Assert.Throws<ArgumentException>(() => tableFormat.WithGlyphPad(16));
        Assert.Equal("glyphPad", e.ParamName);
    }

    [Fact]
    public void TestScanUnit1()
    {
        var tableFormat = PcfTableFormat.Default;
        Assert.Equal(0, tableFormat.ScanUnitIndex);
        Assert.Equal(1u, tableFormat.ScanUnit);

        tableFormat = tableFormat.WithScanUnitIndex(1);
        Assert.Equal(1, tableFormat.ScanUnitIndex);
        Assert.Equal(2u, tableFormat.ScanUnit);

        tableFormat = tableFormat.WithScanUnitIndex(2);
        Assert.Equal(2, tableFormat.ScanUnitIndex);
        Assert.Equal(4u, tableFormat.ScanUnit);

        tableFormat = tableFormat.WithScanUnitIndex(0);
        Assert.Equal(0, tableFormat.ScanUnitIndex);
        Assert.Equal(1u, tableFormat.ScanUnit);

        Assert.Throws<ArgumentOutOfRangeException>(() => tableFormat.WithScanUnitIndex(3));
    }

    [Fact]
    public void TestScanUnit2()
    {
        var tableFormat = PcfTableFormat.Default;
        Assert.Equal(1u, tableFormat.ScanUnit);
        Assert.Equal(0, tableFormat.ScanUnitIndex);

        tableFormat = tableFormat.WithScanUnit(2);
        Assert.Equal(2u, tableFormat.ScanUnit);
        Assert.Equal(1, tableFormat.ScanUnitIndex);

        tableFormat = tableFormat.WithScanUnit(4);
        Assert.Equal(4u, tableFormat.ScanUnit);
        Assert.Equal(2, tableFormat.ScanUnitIndex);

        tableFormat = tableFormat.WithScanUnit(1);
        Assert.Equal(1u, tableFormat.ScanUnit);
        Assert.Equal(0, tableFormat.ScanUnitIndex);

        var e = Assert.Throws<ArgumentException>(() => tableFormat.WithScanUnit(8));
        Assert.Equal("scanUnit", e.ParamName);
    }

    [Fact]
    public void TestEquals()
    {
        var tableFormat1 = PcfTableFormat.Of(
            msByteFirst: true,
            msBitFirst: true,
            inkBoundsOrCompressedMetrics: true,
            glyphPad: 2,
            scanUnit: 4);
        var tableFormat2 = PcfTableFormat.Of(
            msByteFirst: true,
            msBitFirst: true,
            inkBoundsOrCompressedMetrics: true,
            glyphPad: 2,
            scanUnit: 4);
        Assert.Equal(tableFormat1, tableFormat2);
        Assert.Equal(tableFormat1.GetHashCode(), tableFormat2.GetHashCode());
    }
}
