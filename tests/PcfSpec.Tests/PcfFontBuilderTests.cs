using BdfSpec;

namespace PcfSpec.Tests;

public class PcfFontBuilderTests
{
    [Theory]
    [InlineData("demo", "demo")]
    [InlineData("demo", "demo-2")]
    [InlineData("unifont", "unifont-17.0.04")]
    public void TestBuilder(string fontDir, string fontFileName)
    {
        var font1 = PcfFont.Load(Path.Combine("assets", fontDir, $"{fontFileName}.pcf"));
        var font2 = PcfFontBuilder.Modify(font1).Build();

        PcfFont font3;
        {
            var bdfFont = BdfFont.Load(Path.Combine("assets", fontDir, $"{fontFileName}.bdf"));

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
                builder.Properties[key] = value switch
                {
                    string stringValue => stringValue,
                    int intValue => intValue,
                    _ => throw new InvalidOperationException()
                };
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

            font3 = builder.Build();
            font3.Properties = font1.Properties;
        }

        var data1 = font1.DumpToBytes();
        var data2 = font2.DumpToBytes();
        var data3 = font3.DumpToBytes();

        Assert.Equal(data1, data2);
        Assert.Equal(data1, data3);
    }

    [Fact]
    public void TestCopy()
    {
        var builder1 = PcfFontBuilder.Modify(PcfFont.Load(Path.Combine("assets", "demo", "demo.pcf")));
        var builder2 = builder1.Copy();

        Assert.Equal(builder1, builder2);
        Assert.NotSame(builder1, builder2);
        Assert.Same(builder1.Config, builder2.Config);
        Assert.Same(builder1.Properties, builder2.Properties);
        Assert.Same(builder1.Glyphs, builder2.Glyphs);
    }

    [Fact]
    public void TestDeepCopy()
    {
        var builder1 = PcfFontBuilder.Modify(PcfFont.Load(Path.Combine("assets", "demo", "demo.pcf")));
        var builder2 = builder1.DeepCopy();

        Assert.Equal(builder1, builder2);
        Assert.NotSame(builder1, builder2);
        Assert.NotSame(builder1.Config, builder2.Config);
        Assert.NotSame(builder1.Properties, builder2.Properties);
        Assert.NotSame(builder1.Glyphs, builder2.Glyphs);

        foreach (var (glyph1, glyph2) in builder1.Glyphs.Zip(builder2.Glyphs))
        {
            Assert.NotSame(glyph1, glyph2);
        }
    }

    [Fact]
    public void TestEquals()
    {
        var filePath = Path.Combine("assets", "demo", "demo.pcf");
        var builder1 = PcfFontBuilder.Modify(PcfFont.Load(filePath));
        var builder2 = PcfFontBuilder.Modify(PcfFont.Load(filePath));
        Assert.Equal(builder1, builder2);
    }
}
