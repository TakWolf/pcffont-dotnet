using BdfSpec;

namespace PcfSpec.Tests;

public class ValidityTests
{
    [Fact]
    public void TestDemo()
    {
        var bdfFont = BdfFont.Load(Path.Combine("assets", "demo", "demo.bdf"));
        var pcfFont1 = PcfFont.Load(Path.Combine("assets", "demo", "demo-lsbyte-lsbit-p4-u2.pcf"));
        var pcfFont2 = PcfFont.Load(Path.Combine("assets", "demo", "demo-lsbyte-msbit-p4-u2.pcf"));
        var pcfFont3 = PcfFont.Load(Path.Combine("assets", "demo", "demo-msbyte-lsbit-p4-u2.pcf"));
        var pcfFont4 = PcfFont.Load(Path.Combine("assets", "demo", "demo-msbyte-msbit-p4-u2.pcf"));
        var pcfFont5 = PcfFont.Load(Path.Combine("assets", "demo", "demo-lsbyte-lsbit-p2-u4.pcf"));
        var pcfFont6 = PcfFont.Load(Path.Combine("assets", "demo", "demo-lsbyte-msbit-p2-u4.pcf"));
        var pcfFont7 = PcfFont.Load(Path.Combine("assets", "demo", "demo-msbyte-lsbit-p2-u4.pcf"));
        var pcfFont8 = PcfFont.Load(Path.Combine("assets", "demo", "demo-msbyte-msbit-p2-u4.pcf"));

        foreach (var glyphIndex in Enumerable.Range(0, bdfFont.Glyphs.Count))
        {
            var glyph = bdfFont.Glyphs[glyphIndex];

            string[] glyphNames = [
                pcfFont1.GlyphNames![glyphIndex],
                pcfFont2.GlyphNames![glyphIndex],
                pcfFont3.GlyphNames![glyphIndex],
                pcfFont4.GlyphNames![glyphIndex],
                pcfFont5.GlyphNames![glyphIndex],
                pcfFont6.GlyphNames![glyphIndex],
                pcfFont7.GlyphNames![glyphIndex],
                pcfFont8.GlyphNames![glyphIndex]
            ];
            foreach (var glyphName in glyphNames)
            {
                Assert.Equal(glyph.Name, glyphName);
            }

            PcfMetric[] metrics = [
                pcfFont1.Metrics![glyphIndex],
                pcfFont2.Metrics![glyphIndex],
                pcfFont3.Metrics![glyphIndex],
                pcfFont4.Metrics![glyphIndex],
                pcfFont5.Metrics![glyphIndex],
                pcfFont6.Metrics![glyphIndex],
                pcfFont7.Metrics![glyphIndex],
                pcfFont8.Metrics![glyphIndex]
            ];
            foreach (var metric in metrics)
            {
                Assert.True(PcfMetric.Equals(metrics[0], metric));
                Assert.Equal(glyph.DeviceWidthX, metric.CharacterWidth);
                Assert.Equal(glyph.Dimensions, metric.Dimensions);
                Assert.Equal(glyph.Offset, metric.Offset);
            }

            List<List<List<byte>>> bitmaps = [
                pcfFont1.Bitmaps![glyphIndex],
                pcfFont2.Bitmaps![glyphIndex],
                pcfFont3.Bitmaps![glyphIndex],
                pcfFont4.Bitmaps![glyphIndex],
                pcfFont5.Bitmaps![glyphIndex],
                pcfFont6.Bitmaps![glyphIndex],
                pcfFont7.Bitmaps![glyphIndex],
                pcfFont8.Bitmaps![glyphIndex]
            ];
            foreach (var bitmap in bitmaps)
            {
                Assert.Equal(glyph.Bitmap, bitmap);
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
