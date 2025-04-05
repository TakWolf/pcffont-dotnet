using PcfSpec.Table;

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

        foreach (ushort glyphIndex in Enumerable.Range(0, Glyphs.Count))
        {
            var glyph = Glyphs[glyphIndex];
            bdfEncodings[glyph.Encoding] = glyphIndex;
            glyphNames.Add(glyph.Name);
            scalableWidths.Add(glyph.ScalableWidth);
            metrics.Add(glyph.CreateMetric(false));
            bitmaps.Add(glyph.Bitmap);
        }

        accelerators.MinBounds = metrics.CalculateMinBounds();
        accelerators.MaxBounds = metrics.CalculateMaxBounds();
        accelerators.MaxOverlap = metrics.CalculateMaxOverlap();
        accelerators.NoOverlap = accelerators.MaxOverlap <= accelerators.MinBounds.LeftSideBearing;
        accelerators.ConstantWidth = accelerators.MinBounds.CharacterWidth == accelerators.MaxBounds.CharacterWidth;
        accelerators.InkInside =
            accelerators.MaxOverlap <= 0 &&
            accelerators.MinBounds.LeftSideBearing >= 0 &&
            accelerators.MinBounds.Ascent >= -accelerators.FontDescent &&
            accelerators.MaxBounds.Ascent <= accelerators.FontAscent &&
            -accelerators.MinBounds.Descent <= accelerators.FontAscent &&
            accelerators.MaxBounds.Descent <= accelerators.FontDescent;

        if (PcfMetric.Equals(accelerators.MinBounds, accelerators.MaxBounds))
        {
            accelerators.ConstantMetrics = true;
            accelerators.TerminalFont =
                accelerators.MinBounds.LeftSideBearing == 0 &&
                accelerators.MinBounds.RightSideBearing == accelerators.MinBounds.CharacterWidth &&
                accelerators.MinBounds.Ascent == accelerators.FontAscent &&
                accelerators.MinBounds.Descent == accelerators.FontDescent;
        }
        else
        {
            accelerators.ConstantMetrics = false;
            accelerators.TerminalFont = false;
        }

        PcfMetrics? inkMetrics;
        if (accelerators.ConstantMetrics)
        {
            inkMetrics = new PcfMetrics(Config.ToTableFormat(), Glyphs.Select(glyph => glyph.CreateMetric(true)));

            accelerators.InkMinBounds = inkMetrics.CalculateMinBounds();
            accelerators.InkMaxBounds = inkMetrics.CalculateMaxBounds();
            accelerators.TableFormat.InkBoundsOrCompressedMetrics = true;
            accelerators.InkMetrics = true;
        }
        else
        {
            inkMetrics = null;

            accelerators.TableFormat.InkBoundsOrCompressedMetrics = false;
            accelerators.InkMetrics = false;
        }

        metrics.TableFormat.InkBoundsOrCompressedMetrics = metrics.CalculateCompressible();
        if (inkMetrics is not null)
        {
            inkMetrics.TableFormat.InkBoundsOrCompressedMetrics = inkMetrics.CalculateCompressible();
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
