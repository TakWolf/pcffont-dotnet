using System.Text.RegularExpressions;
using BdfSpec;
using FreeTypeSharp;

namespace PcfSpec.Tests.Conformance;

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

            for (var glyphIndex = 0; glyphIndex < bdfFont.Glyphs.Count; glyphIndex++)
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

        for (var glyphIndex = 0; glyphIndex < bdfFont.Glyphs.Count; glyphIndex++)
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

    [Fact]
    public unsafe void TestWithFreetype()
    {
        var bdfFont = BdfFont.Load(Path.Combine("assets", "demo", "demo.bdf"));
        bdfFont.Glyphs.RemoveRange(10, bdfFont.Glyphs.Count - 10);

        var builder = new PcfFontBuilder();
        builder.Config.FontAscent = bdfFont.Properties.FontAscent!.Value;
        builder.Config.FontDescent = bdfFont.Properties.FontDescent!.Value;

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

        foreach (var (key, value) in bdfFont.Properties)
        {
            builder.Properties[key] = value;
        }
        builder.Properties.GenerateXlfd();

        using var freeTypeLibrary = new FreeTypeLibrary();
        foreach (var msByteFirst in new[] { false, true })
        {
            foreach (var msBitFirst in new[] { false, true })
            {
                foreach (var glyphPad in PcfTableFormat.GlyphPadOptions)
                {
                    foreach (var scanUnit in PcfTableFormat.ScanUnitOptions)
                    {
                        builder.Config.MsByteFirst = msByteFirst;
                        builder.Config.MsBitFirst = msBitFirst;
                        builder.Config.GlyphPad = glyphPad;
                        builder.Config.ScanUnit = scanUnit;

                        var pcfFont = builder.Build();
                        var fontBytes = pcfFont.DumpToBytes();
                        fixed (byte* fontBytesPtr = fontBytes)
                        {
                            var ftFont = new FreeTypeFaceFacade(freeTypeLibrary, (IntPtr)fontBytesPtr, fontBytes.Length);

                            var pcfGlyphIndexToEncoding = new Dictionary<int, ushort>();
                            foreach (var (encoding, glyphIndex) in pcfFont.BdfEncodings!)
                            {
                                pcfGlyphIndexToEncoding[glyphIndex] = encoding;
                            }

                            for (var pcfGlyphIndex = 0; pcfGlyphIndex < pcfFont.GlyphNames!.Count; pcfGlyphIndex++)
                            {
                                var encoding = pcfGlyphIndexToEncoding[pcfGlyphIndex];
                                var ftGlyphIndex = ftFont.GetCharIndex(encoding);
                                Assert.Equal(pcfGlyphIndex + 1u, ftGlyphIndex);

                                var loadGlyphError = FT.FT_Load_Glyph(ftFont.FaceRec, ftGlyphIndex, 0);
                                if (loadGlyphError != FT_Error.FT_Err_Ok)
                                {
                                    throw new FreeTypeException(loadGlyphError);
                                }
                                var ftBitmap = ftFont.GlyphBitmap;

                                var pcfBitmap = pcfFont.Bitmaps![pcfGlyphIndex];
                                var pcfMetric = pcfFont.Metrics![pcfGlyphIndex];

                                Assert.Equal((uint)pcfBitmap[0].Count, ftBitmap.width);
                                Assert.Equal((uint)pcfMetric.Width, ftBitmap.width);
                                Assert.Equal((uint)pcfBitmap.Count, ftBitmap.rows);
                                Assert.Equal((uint)pcfMetric.Height, ftBitmap.rows);

                                for (var y = 0; y < ftBitmap.rows; y++)
                                {
                                    var ftBitmapRow = new List<byte>((int)ftBitmap.width);
                                    for (var x = 0; x < ftBitmap.width; x++)
                                    {
                                        ftBitmapRow.Add((byte)((ftBitmap.buffer[y * ftBitmap.pitch + x / 8] >> (7 - x % 8)) & 1));
                                    }
                                    var pcfBitmapRow = pcfBitmap[y];
                                    Assert.Equal(pcfBitmapRow, ftBitmapRow);
                                }

                                var pcfBitmapRowSize = (pcfMetric.Width + glyphPad * 8 - 1) / (glyphPad * 8) * glyphPad;
                                Assert.Equal(pcfBitmapRowSize, ftBitmap.pitch);
                                var pcfBitmapSize = pcfBitmapRowSize * pcfMetric.Height;
                                Assert.Equal(pcfBitmapSize, ftBitmap.pitch * ftBitmap.rows);
                            }
                        }
                    }
                }
            }
        }
    }
}
