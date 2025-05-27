using BdfSpec;

namespace PcfSpec.Tests;

public class ValidityTests
{
    [Fact]
    public void TestDemo()
    {
        var font0 = BdfFont.Load(Path.Combine("assets", "demo", "demo.bdf"));
        var font1 = PcfFont.Load(Path.Combine("assets", "demo", "demo-lsbyte-lsbit-p4-u2.pcf"));
        var font2 = PcfFont.Load(Path.Combine("assets", "demo", "demo-lsbyte-msbit-p4-u2.pcf"));
        var font3 = PcfFont.Load(Path.Combine("assets", "demo", "demo-msbyte-lsbit-p4-u2.pcf"));
        var font4 = PcfFont.Load(Path.Combine("assets", "demo", "demo-msbyte-msbit-p4-u2.pcf"));
        var font5 = PcfFont.Load(Path.Combine("assets", "demo", "demo-lsbyte-lsbit-p2-u4.pcf"));
        var font6 = PcfFont.Load(Path.Combine("assets", "demo", "demo-lsbyte-msbit-p2-u4.pcf"));
        var font7 = PcfFont.Load(Path.Combine("assets", "demo", "demo-msbyte-lsbit-p2-u4.pcf"));
        var font8 = PcfFont.Load(Path.Combine("assets", "demo", "demo-msbyte-msbit-p2-u4.pcf"));

        foreach (var glyphIndex in Enumerable.Range(0, font0.Glyphs.Count))
        {
            var glyph = font0.Glyphs[glyphIndex];

            string[] glyphNames = [
                font1.GlyphNames![glyphIndex],
                font2.GlyphNames![glyphIndex],
                font3.GlyphNames![glyphIndex],
                font4.GlyphNames![glyphIndex],
                font5.GlyphNames![glyphIndex],
                font6.GlyphNames![glyphIndex],
                font7.GlyphNames![glyphIndex],
                font8.GlyphNames![glyphIndex]
            ];
            foreach (var glyphName in glyphNames)
            {
                Assert.Equal(glyph.Name, glyphName);
            }

            PcfMetric[] metrics = [
                font1.Metrics![glyphIndex],
                font2.Metrics![glyphIndex],
                font3.Metrics![glyphIndex],
                font4.Metrics![glyphIndex],
                font5.Metrics![glyphIndex],
                font6.Metrics![glyphIndex],
                font7.Metrics![glyphIndex],
                font8.Metrics![glyphIndex]
            ];
            foreach (var metric in metrics)
            {
                Assert.True(PcfMetric.Equals(metrics[0], metric));
                Assert.Equal(glyph.DeviceWidthX, metric.CharacterWidth);
                Assert.Equal(glyph.Dimensions, metric.Dimensions);
                Assert.Equal(glyph.Offset, metric.Offset);
            }

            List<List<List<byte>>> bitmaps = [
                font1.Bitmaps![glyphIndex],
                font2.Bitmaps![glyphIndex],
                font3.Bitmaps![glyphIndex],
                font4.Bitmaps![glyphIndex],
                font5.Bitmaps![glyphIndex],
                font6.Bitmaps![glyphIndex],
                font7.Bitmaps![glyphIndex],
                font8.Bitmaps![glyphIndex]
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
        var font1 = BdfFont.Load(Path.Combine("assets", "unifont", "unifont-16.0.03.bdf"));
        var font2 = PcfFont.Load(Path.Combine("assets", "unifont", "unifont-16.0.03.pcf"));

        foreach (var glyphIndex in Enumerable.Range(0, font1.Glyphs.Count))
        {
            var glyph = font1.Glyphs[glyphIndex];

            var glyphName = font2.GlyphNames![glyphIndex];
            Assert.Equal(glyph.Name, glyphName);

            var metric = font2.Metrics![glyphIndex];
            Assert.Equal(glyph.DeviceWidthX, metric.CharacterWidth);
            Assert.Equal(glyph.Dimensions, metric.Dimensions);
            Assert.Equal(glyph.Offset, metric.Offset);

            var bitmap = font2.Bitmaps![glyphIndex];
            Assert.Equal(glyph.Bitmap, bitmap);
        }
    }
}
