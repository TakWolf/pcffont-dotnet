namespace PcfSpec.Tests;

public class PcfFontConfigTests
{
    [Fact]
    public void TestToTableFormat()
    {
        Assert.Equal(new PcfTableFormat(), new PcfFontConfig().ToTableFormat());
        Assert.Equal(new PcfTableFormat(
            msByteFirst: true,
            msBitFirst: true,
            glyphPadIndex: 1,
            scanUnitIndex: 2), new PcfFontConfig(
            msByteFirst: true,
            msBitFirst: true,
            glyphPadIndex: 1,
            scanUnitIndex: 2).ToTableFormat());
    }

    [Fact]
    public void TestGlyphPad1()
    {
        var config = new PcfFontConfig(glyphPadIndex: -1);
        config.GlyphPadIndex = 0;
        Assert.Equal(1u, config.GlyphPad);
        config.GlyphPadIndex = 1;
        Assert.Equal(2u, config.GlyphPad);
        config.GlyphPadIndex = 2;
        Assert.Equal(4u, config.GlyphPad);
        config.GlyphPadIndex = 3;
        Assert.Equal(8u, config.GlyphPad);

        config.GlyphPadIndex = 4;
        Assert.Throws<IndexOutOfRangeException>(() => config.GlyphPad);
    }

    [Fact]
    public void TestGlyphPad2()
    {
        var config = new PcfFontConfig(glyphPadIndex: -1);
        config.GlyphPad = 1;
        Assert.Equal(0, config.GlyphPadIndex);
        config.GlyphPad = 2;
        Assert.Equal(1, config.GlyphPadIndex);
        config.GlyphPad = 4;
        Assert.Equal(2, config.GlyphPadIndex);
        config.GlyphPad = 8;
        Assert.Equal(3, config.GlyphPadIndex);

        var e = Assert.Throws<ArgumentException>(() => config.GlyphPad = 16);
        Assert.Equal("glyphPad", e.ParamName);
    }

    [Fact]
    public void TestScanUnit1()
    {
        var config = new PcfFontConfig(scanUnitIndex: -1);
        config.ScanUnitIndex = 0;
        Assert.Equal(1u, config.ScanUnit);
        config.ScanUnitIndex = 1;
        Assert.Equal(2u, config.ScanUnit);
        config.ScanUnitIndex = 2;
        Assert.Equal(4u, config.ScanUnit);

        config.ScanUnitIndex = 3;
        Assert.Throws<IndexOutOfRangeException>(() => config.ScanUnit);
    }

    [Fact]
    public void TestScanUnit2()
    {
        var config = new PcfFontConfig(scanUnitIndex: -1);
        config.ScanUnit = 1;
        Assert.Equal(0, config.ScanUnitIndex);
        config.ScanUnit = 2;
        Assert.Equal(1, config.ScanUnitIndex);
        config.ScanUnit = 4;
        Assert.Equal(2, config.ScanUnitIndex);

        var e = Assert.Throws<ArgumentException>(() => config.ScanUnit = 8);
        Assert.Equal("scanUnit", e.ParamName);
    }

    [Fact]
    public void TestCopy()
    {
        var config1 = new PcfFontConfig(
            fontAscent: 1,
            fontDescent: 2,
            defaultChar: 3,
            drawRightToLeft: true,
            msByteFirst: true,
            msBitFirst: true,
            glyphPadIndex: 1,
            scanUnitIndex: 2);
        var config2 = config1.Copy();
        var config3 = config1.DeepCopy();

        Assert.Equal(config1, config2);
        Assert.Equal(config1, config3);
        Assert.NotSame(config1, config2);
        Assert.NotSame(config1, config3);
    }

    [Fact]
    public void TestEquals()
    {
        var config1 = new PcfFontConfig(
            fontAscent: 1,
            fontDescent: 2,
            defaultChar: 3,
            drawRightToLeft: true,
            msByteFirst: true,
            msBitFirst: true,
            glyphPadIndex: 1,
            scanUnitIndex: 2);
        var config2 = new PcfFontConfig(
            fontAscent: 1,
            fontDescent: 2,
            defaultChar: 3,
            drawRightToLeft: true,
            msByteFirst: true,
            msBitFirst: true,
            glyphPadIndex: 1,
            scanUnitIndex: 2);
        Assert.Equal(config1, config2);
    }
}
