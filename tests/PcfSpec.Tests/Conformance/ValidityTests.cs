using System.Runtime.InteropServices;
using BdfSpec;
using FreeTypeSharp;

namespace PcfSpec.Tests.Conformance;

public class DemoFonts
{
    public BdfFont DemoBdf { get; }
    public PcfFont DemoPcf { get; }

    public DemoFonts()
    {
        DemoBdf = BdfFont.Load(Path.Combine("assets", "demo", "demo.bdf"));
        DemoPcf = PcfFont.Load(Path.Combine("assets", "demo", "demo.pcf"));
    }
}

public class ValidityTests : IClassFixture<DemoFonts>
{
    private readonly DemoFonts _demoFonts;

    public ValidityTests(DemoFonts demoFonts)
    {
        _demoFonts = demoFonts;
    }

    [Theory]
    [InlineData("demo-lsbyte-lsbit-p1-u1.pcf")]
    [InlineData("demo-lsbyte-lsbit-p1-u2.pcf")]
    [InlineData("demo-lsbyte-lsbit-p1-u4.pcf")]
    [InlineData("demo-lsbyte-lsbit-p2-u1.pcf")]
    [InlineData("demo-lsbyte-lsbit-p2-u2.pcf")]
    [InlineData("demo-lsbyte-lsbit-p2-u4.pcf")]
    [InlineData("demo-lsbyte-lsbit-p4-u1.pcf")]
    [InlineData("demo-lsbyte-lsbit-p4-u2.pcf")]
    [InlineData("demo-lsbyte-lsbit-p4-u4.pcf")]
    [InlineData("demo-lsbyte-msbit-p1-u1.pcf")]
    [InlineData("demo-lsbyte-msbit-p1-u2.pcf")]
    [InlineData("demo-lsbyte-msbit-p1-u4.pcf")]
    [InlineData("demo-lsbyte-msbit-p2-u1.pcf")]
    [InlineData("demo-lsbyte-msbit-p2-u2.pcf")]
    [InlineData("demo-lsbyte-msbit-p2-u4.pcf")]
    [InlineData("demo-lsbyte-msbit-p4-u1.pcf")]
    [InlineData("demo-lsbyte-msbit-p4-u2.pcf")]
    [InlineData("demo-lsbyte-msbit-p4-u4.pcf")]
    [InlineData("demo-msbyte-lsbit-p1-u1.pcf")]
    [InlineData("demo-msbyte-lsbit-p1-u2.pcf")]
    [InlineData("demo-msbyte-lsbit-p1-u4.pcf")]
    [InlineData("demo-msbyte-lsbit-p2-u1.pcf")]
    [InlineData("demo-msbyte-lsbit-p2-u2.pcf")]
    [InlineData("demo-msbyte-lsbit-p2-u4.pcf")]
    [InlineData("demo-msbyte-lsbit-p4-u1.pcf")]
    [InlineData("demo-msbyte-lsbit-p4-u2.pcf")]
    [InlineData("demo-msbyte-lsbit-p4-u4.pcf")]
    [InlineData("demo-msbyte-msbit-p1-u1.pcf")]
    [InlineData("demo-msbyte-msbit-p1-u2.pcf")]
    [InlineData("demo-msbyte-msbit-p1-u4.pcf")]
    [InlineData("demo-msbyte-msbit-p2-u1.pcf")]
    [InlineData("demo-msbyte-msbit-p2-u2.pcf")]
    [InlineData("demo-msbyte-msbit-p2-u4.pcf")]
    [InlineData("demo-msbyte-msbit-p4-u1.pcf")]
    [InlineData("demo-msbyte-msbit-p4-u2.pcf")]
    [InlineData("demo-msbyte-msbit-p4-u4.pcf")]
    public void TestDemo(string fontFileName)
    {
        var bdfFont = _demoFonts.DemoBdf;
        var pcfFont0 = _demoFonts.DemoPcf;
        var pcfFontX = PcfFont.Load(Path.Combine("assets", "demo", fontFileName));

        for (var glyphIndex = 0; glyphIndex < bdfFont.Glyphs.Count; glyphIndex++)
        {
            var glyph = bdfFont.Glyphs[glyphIndex];

            var glyphName0 = pcfFont0.GlyphNames![glyphIndex];
            var glyphNameX = pcfFontX.GlyphNames![glyphIndex];
            Assert.Equal(glyph.Name, glyphNameX);
            Assert.Equal(glyphName0, glyphNameX);

            var metric0 = pcfFont0.Metrics![glyphIndex];
            var metricX = pcfFontX.Metrics![glyphIndex];
            Assert.Equal(glyph.DeviceWidthX, metricX.CharacterWidth);
            Assert.Equal(glyph.Dimensions, metricX.Dimensions);
            Assert.Equal(glyph.Offset, metricX.Offset);
            Assert.Equal(metric0, metricX);

            var bitmap0 = pcfFont0.Bitmaps![glyphIndex];
            var bitmapX = pcfFontX.Bitmaps![glyphIndex];
            Assert.Equal(glyph.Bitmap, bitmapX);
            Assert.Equal(bitmap0, bitmapX);
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

    [Theory, CombinatorialData]
    public unsafe void TestWithFreetype(
        [CombinatorialValues(false, true)] bool msByteFirst,
        [CombinatorialValues(false, true)] bool msBitFirst,
        [CombinatorialValues(1, 2, 4, 8)] uint glyphPad,
        [CombinatorialValues(1, 2, 4)] uint scanUnit)
    {
        var builder = new PcfFontBuilder();
        builder.Config.FontAscent = _demoFonts.DemoBdf.Properties.FontAscent!.Value;
        builder.Config.FontDescent = _demoFonts.DemoBdf.Properties.FontDescent!.Value;
        builder.Config.MsByteFirst = msByteFirst;
        builder.Config.MsBitFirst = msBitFirst;
        builder.Config.GlyphPad = glyphPad;
        builder.Config.ScanUnit = scanUnit;

        foreach (var bdfGlyph in CollectionsMarshal.AsSpan(_demoFonts.DemoBdf.Glyphs)[..10])
        {
            builder.Glyphs.Add(new PcfGlyph(
                name: bdfGlyph.Name,
                encoding: (ushort)bdfGlyph.Encoding,
                scalableWidth: bdfGlyph.ScalableWidthX,
                characterWidth: (short)bdfGlyph.DeviceWidthX,
                dimensions: bdfGlyph.Dimensions,
                offset: bdfGlyph.Offset,
                attributes: bdfGlyph.Attributes,
                bitmap: bdfGlyph.Bitmap));
        }

        foreach (var (key, value) in _demoFonts.DemoBdf.Properties)
        {
            builder.Properties[key] = value switch
            {
                { IsInt: true } => value.AsInt(),
                { IsString: true } => value.AsString(),
                _ => throw new InvalidOperationException()
            };
        }
        builder.Properties.GenerateXlfd();

        var pcfFont = builder.Build();
        var fontBytes = pcfFont.DumpToBytes();
        fixed (byte* fontBytesPtr = fontBytes)
        {
            using var freeTypeLibrary = new FreeTypeLibrary();
            var ftFont = new FreeTypeFaceFacade(freeTypeLibrary, (IntPtr)fontBytesPtr, fontBytes.Length);

            var pcfGlyphIndexToEncoding = new Dictionary<int, ushort>(pcfFont.BdfEncodings!.Count);
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
