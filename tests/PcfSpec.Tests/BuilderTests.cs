using BdfSpec;
using PcfSpec.Tables;

namespace PcfSpec.Tests;

public class BuilderTests
{
    private static PcfFont LoadPcfByBdf(string path)
    {
        var bdfFont = BdfFont.Load(path);

        var builder = new PcfFontBuilder();
        builder.Config.FontAscent = bdfFont.Properties.FontAscent!.Value;
        builder.Config.FontDescent = bdfFont.Properties.FontDescent!.Value;
        if (bdfFont.Properties.DefaultChar is not null)
        {
            builder.Config.DefaultChar = (ushort)bdfFont.Properties.DefaultChar;
        }
        builder.Config.MsByteFirst = true;
        builder.Config.MsBitFirst = true;
        builder.Config.GlyphPadIndex = 2;

        foreach (var bdfGlyph in bdfFont.Glyphs)
        {
            builder.Glyphs.Add(new PcfGlyph(
                name: bdfGlyph.Name,
                encoding: (ushort)bdfGlyph.Encoding,
                scalableWidth: bdfGlyph.ScalableWidthX,
                characterWidth: (short)bdfGlyph.DeviceWidthX,
                dimensions: ((short, short))bdfGlyph.Dimensions,
                offset: ((short, short))bdfGlyph.Offset,
                bitmap: bdfGlyph.Bitmap));
        }

        foreach (var (key, value) in bdfFont.Properties)
        {
            builder.Properties[key] = value;
        }
        builder.Properties.GenerateXlfd();

        return builder.Build();
    }

    [Fact]
    public void TestUnifont()
    {
        var pcfFont = PcfFont.Load(Path.Combine("assets", "unifont", "unifont-17.0.04.pcf"));
        pcfFont.Accelerators!.CompatInfo = null;
        pcfFont.BdfAccelerators!.CompatInfo = null;
        pcfFont.Bitmaps!.CompatInfo = null;
        var bdfFont = LoadPcfByBdf(Path.Combine("assets", "unifont", "unifont-17.0.04.bdf"));

        Assert.True(PcfBdfEncodings.Equals(pcfFont.BdfEncodings, bdfFont.BdfEncodings));
        Assert.True(PcfGlyphNames.Equals(pcfFont.GlyphNames, bdfFont.GlyphNames));
        Assert.True(PcfScalableWidths.Equals(pcfFont.ScalableWidths, bdfFont.ScalableWidths));
        Assert.True(PcfMetrics.Equals(pcfFont.Metrics, bdfFont.Metrics));
        Assert.True(PcfMetrics.Equals(pcfFont.InkMetrics, bdfFont.InkMetrics));
        Assert.True(PcfBitmaps.Equals(pcfFont.Bitmaps, bdfFont.Bitmaps));
        Assert.True(PcfAccelerators.Equals(pcfFont.Accelerators, bdfFont.Accelerators));
        Assert.True(PcfAccelerators.Equals(pcfFont.BdfAccelerators, bdfFont.BdfAccelerators));
        Assert.Equal(pcfFont.Properties!.Font!.ToUpper(), bdfFont.Properties!.Font!.ToUpper().Replace("-SANS SERIF", "-SANS"));
    }

    [Fact]
    public void TestDemo()
    {
        var pcfFont = PcfFont.Load(Path.Combine("assets", "demo", "demo.pcf"));
        pcfFont.Accelerators!.CompatInfo = null;
        pcfFont.BdfAccelerators!.CompatInfo = null;
        pcfFont.Bitmaps!.CompatInfo = null;
        var bdfFont = LoadPcfByBdf(Path.Combine("assets", "demo", "demo.bdf"));

        Assert.True(PcfBdfEncodings.Equals(pcfFont.BdfEncodings, bdfFont.BdfEncodings));
        Assert.True(PcfGlyphNames.Equals(pcfFont.GlyphNames, bdfFont.GlyphNames));
        Assert.True(PcfScalableWidths.Equals(pcfFont.ScalableWidths, bdfFont.ScalableWidths));
        Assert.True(PcfMetrics.Equals(pcfFont.Metrics, bdfFont.Metrics));
        Assert.True(PcfMetrics.Equals(pcfFont.InkMetrics, bdfFont.InkMetrics));
        Assert.True(PcfBitmaps.Equals(pcfFont.Bitmaps, bdfFont.Bitmaps));
        Assert.True(PcfAccelerators.Equals(pcfFont.Accelerators, bdfFont.Accelerators));
        Assert.True(PcfAccelerators.Equals(pcfFont.BdfAccelerators, bdfFont.BdfAccelerators));
        Assert.Equal(pcfFont.Properties!.Font, bdfFont.Properties!.Font);
    }

    [Fact]
    public void TestDemo2()
    {
        var pcfFont = PcfFont.Load(Path.Combine("assets", "demo", "demo-2.pcf"));
        pcfFont.Accelerators!.CompatInfo = null;
        pcfFont.BdfAccelerators!.CompatInfo = null;
        pcfFont.Bitmaps!.CompatInfo = null;
        var bdfFont = LoadPcfByBdf(Path.Combine("assets", "demo", "demo-2.bdf"));

        Assert.True(PcfBdfEncodings.Equals(pcfFont.BdfEncodings, bdfFont.BdfEncodings));
        Assert.True(PcfGlyphNames.Equals(pcfFont.GlyphNames, bdfFont.GlyphNames));
        Assert.True(PcfScalableWidths.Equals(pcfFont.ScalableWidths, bdfFont.ScalableWidths));
        Assert.True(PcfMetrics.Equals(pcfFont.Metrics, bdfFont.Metrics));
        Assert.True(PcfMetrics.Equals(pcfFont.InkMetrics, bdfFont.InkMetrics));
        Assert.True(PcfBitmaps.Equals(pcfFont.Bitmaps, bdfFont.Bitmaps));
        Assert.True(PcfAccelerators.Equals(pcfFont.Accelerators, bdfFont.Accelerators));
        Assert.True(PcfAccelerators.Equals(pcfFont.BdfAccelerators, bdfFont.BdfAccelerators));
        Assert.Equal(pcfFont.Properties!.Font, bdfFont.Properties!.Font);
    }
}
