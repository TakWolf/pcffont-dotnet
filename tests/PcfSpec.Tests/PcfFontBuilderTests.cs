using BdfSpec;

namespace PcfSpec.Tests;

public class PcfFontBuilderTests
{
    private static PcfFont CreatePcfByBdf(string path)
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
        builder.Config.GlyphPad = 4;

        foreach (var (key, value) in bdfFont.Properties)
        {
            builder.Properties[key] = value;
        }
        builder.Properties.GenerateXlfd();

        foreach (var bdfGlyph in bdfFont.Glyphs)
        {
            builder.Glyphs.Add(new PcfGlyph(
                name: bdfGlyph.Name,
                encoding: (ushort)bdfGlyph.Encoding,
                scalableWidth: bdfGlyph.ScalableWidthX,
                characterWidth: (short)bdfGlyph.DeviceWidthX,
                dimensions: bdfGlyph.Dimensions,
                offset: bdfGlyph.Offset,
                bitmap: bdfGlyph.Bitmap));
        }

        return builder.Build();
    }

    [Fact]
    public void TestUnifont()
    {
        var pcfPath = Path.Combine("assets", "unifont", "unifont-17.0.04.pcf");
        var bdfPath = Path.Combine("assets", "unifont", "unifont-17.0.04.bdf");

        var font1 = PcfFont.Load(pcfPath);
        var font2 = CreatePcfByBdf(bdfPath);
        font2.Properties = font1.Properties;
        var font3 = PcfFontBuilder.Modify(font1).Build();

        var data1 = font1.DumpToBytes();
        var data2 = font2.DumpToBytes();
        var data3 = font3.DumpToBytes();

        Assert.Equal(data1, data2);
        Assert.Equal(data1, data3);
    }

    [Fact]
    public void TestDemo()
    {
        var pcfPath = Path.Combine("assets", "demo", "demo.pcf");
        var bdfPath = Path.Combine("assets", "demo", "demo.bdf");

        var font1 = PcfFont.Load(pcfPath);
        var font2 = CreatePcfByBdf(bdfPath);
        font2.Properties = font1.Properties;
        var font3 = PcfFontBuilder.Modify(font1).Build();

        var data1 = font1.DumpToBytes();
        var data2 = font2.DumpToBytes();
        var data3 = font3.DumpToBytes();

        Assert.Equal(data1, data2);
        Assert.Equal(data1, data3);
    }

    [Fact]
    public void TestDemo2()
    {
        var pcfPath = Path.Combine("assets", "demo", "demo-2.pcf");
        var bdfPath = Path.Combine("assets", "demo", "demo-2.bdf");

        var font1 = PcfFont.Load(pcfPath);
        var font2 = CreatePcfByBdf(bdfPath);
        font2.Properties = font1.Properties;
        var font3 = PcfFontBuilder.Modify(font1).Build();

        var data1 = font1.DumpToBytes();
        var data2 = font2.DumpToBytes();
        var data3 = font3.DumpToBytes();

        Assert.Equal(data1, data2);
        Assert.Equal(data1, data3);
    }
}
