using PcfSpec.Tables;

namespace PcfSpec;

public class PcfFontBuilder
{
    public PcfFontConfig Config;
    public PcfProperties Properties;
    public List<PcfGlyph> Glyphs;

    public PcfFontBuilder(
        PcfFontConfig? config = null,
        PcfProperties? properties = null,
        List<PcfGlyph>? glyphs = null)
    {
        Config = config ?? new PcfFontConfig();
        Properties = properties ?? new PcfProperties();
        Glyphs = glyphs ?? [];
    }

    public PcfFont Build()
    {
        var bdfEncodings = new PcfBdfEncodings(
            Config.ToTableFormat(),
            defaultChar: Config.DefaultChar);
        var glyphNames = new PcfGlyphNames(Config.ToTableFormat());
        var scalableWidths = new PcfScalableWidths(Config.ToTableFormat());
        var metrics = new PcfMetrics(Config.ToTableFormat());
        var bitmaps = new PcfBitmaps(Config.ToTableFormat());
        var accelerators = new PcfAccelerators(
            Config.ToTableFormat(),
            drawRightToLeft: Config.DrawRightToLeft,
            fontAscent: Config.FontAscent,
            fontDescent: Config.FontDescent);
        var properties = new PcfProperties(
            Config.ToTableFormat(),
            Properties);

        for (var glyphIndex = 0; glyphIndex < Glyphs.Count; glyphIndex++)
        {
            var glyph = Glyphs[glyphIndex];
            bdfEncodings[glyph.Encoding] = (ushort)glyphIndex;
            glyphNames.Add(glyph.Name);
            scalableWidths.Add(glyph.ScalableWidth);
            metrics.Add(glyph.CreateMetric(false));
            bitmaps.Add(glyph.Bitmap);
        }

        accelerators.MinBounds = metrics.CalculateMinBounds();
        accelerators.MaxBounds = metrics.CalculateMaxBounds();
        accelerators.MaxOverlap = metrics.CalculateMaxOverlap();
        accelerators.CalculateBounds();

        PcfMetrics? inkMetrics;
        if (accelerators.ConstantMetrics)
        {
            inkMetrics = new PcfMetrics(Config.ToTableFormat(), Glyphs.Select(glyph => glyph.CreateMetric(true)));

            accelerators.InkMinBounds = inkMetrics.CalculateMinBounds();
            accelerators.InkMaxBounds = inkMetrics.CalculateMaxBounds();
            accelerators.TableFormat.InkBounds = true;
            accelerators.InkMetrics = true;
        }
        else
        {
            inkMetrics = null;

            accelerators.TableFormat.InkBounds = false;
            accelerators.InkMetrics = false;
        }

        metrics.TableFormat.CompressedMetrics = accelerators.MinBounds.Compressible && accelerators.MaxBounds.Compressible;
        if (inkMetrics is not null)
        {
            inkMetrics.TableFormat.CompressedMetrics = accelerators.InkMinBounds!.Compressible && accelerators.InkMaxBounds!.Compressible;
        }

        var font = new PcfFont
        {
            BdfEncodings = bdfEncodings,
            GlyphNames = glyphNames,
            ScalableWidths = scalableWidths,
            Metrics = metrics,
            InkMetrics = inkMetrics,
            Bitmaps = bitmaps,
            Accelerators = accelerators,
            BdfAccelerators = accelerators,
            Properties = properties
        };
        return font;
    }

    public void Save(string path) => Build().Save(path);
}
