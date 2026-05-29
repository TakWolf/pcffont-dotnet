using System.Text.RegularExpressions;
using BdfSpec;

namespace PcfSpec.Tests;

public partial class ValidityTests
{
    [GeneratedRegex(@"^demo-(lsbyte|msbyte)-(lsbit|msbit)-p([1248])-u([124])\.pcf$")]
    private static partial Regex RegexDemoFontName();

    [Fact]
    public void TestDemo()
    {
        var bdfFont = BdfFont.Load(Path.Combine("assets", "demo", "demo.bdf"));
        var pcfFont0 = PcfFont.Load(Path.Combine("assets", "demo", "demo.pcf"));

        var pcfFilePaths = Directory.GetFiles(Path.Combine("assets", "demo"))
            .Where(path => RegexDemoFontName().IsMatch(Path.GetFileName(path)))
            .Order()
            .ToList();
        Assert.Equal(36, pcfFilePaths.Count);

        foreach (var path in pcfFilePaths)
        {
            var pcfFontX = PcfFont.Load(path);

            foreach (var glyphIndex in Enumerable.Range(0, bdfFont.Glyphs.Count))
            {
                var glyph = bdfFont.Glyphs[glyphIndex];

                var glyphName0 = pcfFont0.GlyphNames![glyphIndex];
                var glyphNameX = pcfFontX.GlyphNames![glyphIndex];
                Assert.Equal(glyphNameX, glyph.Name);
                Assert.Equal(glyphNameX, glyphName0);

                var metric0 = pcfFont0.Metrics![glyphIndex];
                var metricX = pcfFontX.Metrics![glyphIndex];
                Assert.Equal(metricX.CharacterWidth, glyph.DeviceWidthX);
                Assert.Equal(metricX.Dimensions, glyph.Dimensions);
                Assert.Equal(metricX.Offset, glyph.Offset);
                Assert.True(PcfMetric.Equals(metricX, metric0));

                var bitmap0 = pcfFont0.Bitmaps![glyphIndex];
                var bitmapX = pcfFontX.Bitmaps![glyphIndex];
                Assert.Equal(bitmapX, glyph.Bitmap);
                Assert.Equal(bitmapX, bitmap0);
            }
        }
    }

    [Fact]
    public void TestUnifont()
    {
        var bdfFont = BdfFont.Load(Path.Combine("assets", "unifont", "unifont-17.0.04.bdf"));
        var pcfFont = PcfFont.Load(Path.Combine("assets", "unifont", "unifont-17.0.04.pcf"));

        foreach (var glyphIndex in Enumerable.Range(0, bdfFont.Glyphs.Count))
        {
            var glyph = bdfFont.Glyphs[glyphIndex];

            var glyphName = pcfFont.GlyphNames![glyphIndex];
            Assert.Equal(glyph.Name, glyphName);

            var metric = pcfFont.Metrics![glyphIndex];
            Assert.Equal(glyph.DeviceWidthX, metric.CharacterWidth);
            Assert.Equal(glyph.Dimensions, metric.Dimensions);
            Assert.Equal(glyph.Offset, metric.Offset);

            var bitmap = pcfFont.Bitmaps![glyphIndex];
            Assert.Equal(glyph.Bitmap, bitmap);
        }
    }
}
