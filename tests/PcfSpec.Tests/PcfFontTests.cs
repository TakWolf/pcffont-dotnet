namespace PcfSpec.Tests;

public class PcfFontTests
{
    [Fact]
    public void TestCopy()
    {
        var font1 = PcfFont.Load(Path.Combine("assets", "demo", "demo.pcf"));
        var font2 = font1.Copy();

        Assert.Equal(font1, font2);
        Assert.NotSame(font1, font2);
        Assert.Same(font1.Properties, font2.Properties);
        Assert.Same(font1.Accelerators, font2.Accelerators);
        Assert.Same(font1.Metrics, font2.Metrics);
        Assert.Same(font1.Bitmaps, font2.Bitmaps);
        Assert.Same(font1.InkMetrics, font2.InkMetrics);
        Assert.Same(font1.BdfEncodings, font2.BdfEncodings);
        Assert.Same(font1.ScalableWidths, font2.ScalableWidths);
        Assert.Same(font1.GlyphNames, font2.GlyphNames);
        Assert.Same(font1.BdfAccelerators, font2.BdfAccelerators);
    }

    [Fact]
    public void TestDeepCopy()
    {
        var font1 = PcfFont.Load(Path.Combine("assets", "demo", "demo.pcf"));
        var font2 = font1.DeepCopy();

        Assert.Equal(font1, font2);
        Assert.NotSame(font1, font2);
        Assert.NotSame(font1.Properties, font2.Properties);
        Assert.NotSame(font1.Accelerators, font2.Accelerators);
        Assert.NotSame(font1.Metrics, font2.Metrics);
        Assert.NotSame(font1.Bitmaps, font2.Bitmaps);
        Assert.NotSame(font1.InkMetrics, font2.InkMetrics);
        Assert.NotSame(font1.BdfEncodings, font2.BdfEncodings);
        Assert.NotSame(font1.ScalableWidths, font2.ScalableWidths);
        Assert.NotSame(font1.GlyphNames, font2.GlyphNames);
        Assert.NotSame(font1.BdfAccelerators, font2.BdfAccelerators);
    }

    [Fact]
    public void TestEquals()
    {
        var filePath = Path.Combine("assets", "demo", "demo.pcf");
        var font1 = PcfFont.Load(filePath);
        var font2 = PcfFont.Load(filePath);
        Assert.Equal(font1, font2);
    }
}
