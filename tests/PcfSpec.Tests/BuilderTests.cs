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
        var font1 = PcfFont.Load(Path.Combine("assets", "unifont", "unifont-17.0.03.pcf"));
        font1.Accelerators!.CompatInfo = null;
        font1.BdfAccelerators!.CompatInfo = null;
        font1.Bitmaps!.CompatInfo = null;
        var font2 = LoadPcfByBdf(Path.Combine("assets", "unifont", "unifont-17.0.03.bdf"));

        Assert.True(PcfBdfEncodings.Equals(font1.BdfEncodings, font2.BdfEncodings));
        Assert.True(PcfGlyphNames.Equals(font1.GlyphNames, font2.GlyphNames));
        Assert.True(PcfScalableWidths.Equals(font1.ScalableWidths, font2.ScalableWidths));
        Assert.True(PcfMetrics.Equals(font1.Metrics, font2.Metrics));
        Assert.True(PcfMetrics.Equals(font1.InkMetrics, font2.InkMetrics));
        Assert.True(PcfBitmaps.Equals(font1.Bitmaps, font2.Bitmaps));
        Assert.True(PcfAccelerators.Equals(font1.Accelerators, font2.Accelerators));
        Assert.True(PcfAccelerators.Equals(font1.BdfAccelerators, font2.BdfAccelerators));
        Assert.Equal(font1.Properties!.Font!.ToUpper(), font2.Properties!.Font!.ToUpper().Replace("-SANS SERIF", "-SANS"));
    }

    [Fact]
    public void TestDemo()
    {
        var font1 = PcfFont.Load(Path.Combine("assets", "demo", "demo.pcf"));
        font1.Accelerators!.CompatInfo = null;
        font1.BdfAccelerators!.CompatInfo = null;
        font1.Bitmaps!.CompatInfo = null;
        var font2 = LoadPcfByBdf(Path.Combine("assets", "demo", "demo.bdf"));

        Assert.True(PcfBdfEncodings.Equals(font1.BdfEncodings, font2.BdfEncodings));
        Assert.True(PcfGlyphNames.Equals(font1.GlyphNames, font2.GlyphNames));
        Assert.True(PcfScalableWidths.Equals(font1.ScalableWidths, font2.ScalableWidths));
        Assert.True(PcfMetrics.Equals(font1.Metrics, font2.Metrics));
        Assert.True(PcfMetrics.Equals(font1.InkMetrics, font2.InkMetrics));
        Assert.True(PcfBitmaps.Equals(font1.Bitmaps, font2.Bitmaps));
        Assert.True(PcfAccelerators.Equals(font1.Accelerators, font2.Accelerators));
        Assert.True(PcfAccelerators.Equals(font1.BdfAccelerators, font2.BdfAccelerators));
        Assert.Equal(font1.Properties!.Font, font2.Properties!.Font);
    }

    [Fact]
    public void TestDemo2()
    {
        var font1 = PcfFont.Load(Path.Combine("assets", "demo", "demo-2.pcf"));
        font1.Accelerators!.CompatInfo = null;
        font1.BdfAccelerators!.CompatInfo = null;
        font1.Bitmaps!.CompatInfo = null;
        var font2 = LoadPcfByBdf(Path.Combine("assets", "demo", "demo-2.bdf"));

        Assert.True(PcfBdfEncodings.Equals(font1.BdfEncodings, font2.BdfEncodings));
        Assert.True(PcfGlyphNames.Equals(font1.GlyphNames, font2.GlyphNames));
        Assert.True(PcfScalableWidths.Equals(font1.ScalableWidths, font2.ScalableWidths));
        Assert.True(PcfMetrics.Equals(font1.Metrics, font2.Metrics));
        Assert.True(PcfMetrics.Equals(font1.InkMetrics, font2.InkMetrics));
        Assert.True(PcfBitmaps.Equals(font1.Bitmaps, font2.Bitmaps));
        Assert.True(PcfAccelerators.Equals(font1.Accelerators, font2.Accelerators));
        Assert.True(PcfAccelerators.Equals(font1.BdfAccelerators, font2.BdfAccelerators));
        Assert.Equal(font1.Properties!.Font, font2.Properties!.Font);
    }
}
