using PcfSpec.Tables;

namespace PcfSpec.Tests;

public class NoCompatTests
{
    [Fact]
    public void TestNoCompat()
    {
        var loadPath = Path.Combine("assets", "unifont", "unifont-17.0.03.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "unifont-17.0.03.pcf");

        var font1 = PcfFont.Load(loadPath);
        font1.Accelerators!.CompatInfo = null;
        font1.BdfAccelerators!.CompatInfo = null;
        font1.Bitmaps!.CompatInfo = null;
        font1.Save(savePath);

        var font2 = PcfFont.Load(savePath);
        Assert.Null(font2.Accelerators!.CompatInfo);
        Assert.Null(font2.BdfAccelerators!.CompatInfo);
        font2.Bitmaps!.CompatInfo = null;

        Assert.True(PcfBdfEncodings.Equals(font1.BdfEncodings, font2.BdfEncodings));
        Assert.True(PcfGlyphNames.Equals(font1.GlyphNames, font2.GlyphNames));
        Assert.True(PcfScalableWidths.Equals(font1.ScalableWidths, font2.ScalableWidths));
        Assert.True(PcfMetrics.Equals(font1.Metrics, font2.Metrics));
        Assert.True(PcfMetrics.Equals(font1.InkMetrics, font2.InkMetrics));
        Assert.True(PcfBitmaps.Equals(font1.Bitmaps, font2.Bitmaps));
        Assert.True(PcfAccelerators.Equals(font1.Accelerators, font2.Accelerators));
        Assert.True(PcfAccelerators.Equals(font1.BdfAccelerators, font2.BdfAccelerators));
        Assert.True(PcfProperties.Equals(font1.Properties, font2.Properties));
    }
}
