namespace PcfSpec.Tests;

public class PcfFontConfigTests
{
    [Fact]
    public void TestToTableFormat()
    {
        Assert.Equal(PcfTableFormat.Default, new PcfFontConfig().ToTableFormat());
        Assert.Equal(PcfTableFormat.Of(
            msByteFirst: true,
            msBitFirst: true,
            glyphPad: 2,
            scanUnit: 4), new PcfFontConfig(
            msByteFirst: true,
            msBitFirst: true,
            glyphPad: 2,
            scanUnit: 4).ToTableFormat());
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
            glyphPad: 2,
            scanUnit: 4);
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
            glyphPad: 2,
            scanUnit: 4);
        var config2 = new PcfFontConfig(
            fontAscent: 1,
            fontDescent: 2,
            defaultChar: 3,
            drawRightToLeft: true,
            msByteFirst: true,
            msBitFirst: true,
            glyphPad: 2,
            scanUnit: 4);
        Assert.Equal(config1, config2);
    }
}
