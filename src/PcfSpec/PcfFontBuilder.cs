using PcfSpec.Tables;
using PcfSpec.Utils;

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

        accelerators.MaxOverlap = CalculateUtil.CalculateMaxOverlap(metrics);
        accelerators.MinBounds = CalculateUtil.CalculateMinBounds(metrics);
        accelerators.MaxBounds = CalculateUtil.CalculateMaxBounds(metrics);
        accelerators.CalculateBounds();

        var glyphIndices = new HashSet<ushort>(bdfEncodings.Values);

        PcfAccelerators bdfAccelerators;
        if (glyphIndices.Count == Glyphs.Count)
        {
            bdfAccelerators = accelerators.Copy();
        }
        else
        {
            var bdfMetrics = new List<PcfMetric>(glyphIndices.Count);
            foreach (var glyphIndex in glyphIndices)
            {
                bdfMetrics.Add(metrics[glyphIndex]);
            }

            bdfAccelerators = new PcfAccelerators(
                Config.ToTableFormat(),
                drawRightToLeft: Config.DrawRightToLeft,
                fontAscent: Config.FontAscent,
                fontDescent: Config.FontDescent);
            bdfAccelerators.MaxOverlap = CalculateUtil.CalculateMaxOverlap(bdfMetrics);
            bdfAccelerators.MinBounds = CalculateUtil.CalculateMinBounds(bdfMetrics);
            bdfAccelerators.MaxBounds = CalculateUtil.CalculateMaxBounds(bdfMetrics);
            bdfAccelerators.CalculateBounds();
        }

        PcfMetrics? inkMetrics;
        if (bdfAccelerators.ConstantMetrics)
        {
            inkMetrics = new PcfMetrics(Config.ToTableFormat());
            foreach (var glyph in Glyphs)
            {
                inkMetrics.Add(glyph.CreateMetric(true));
            }

            accelerators.InkMinBounds = CalculateUtil.CalculateMinBounds(inkMetrics);
            accelerators.InkMaxBounds = CalculateUtil.CalculateMaxBounds(inkMetrics);
            accelerators.TableFormat.InkBounds = true;
            accelerators.InkMetrics = true;

            if (glyphIndices.Count == Glyphs.Count)
            {
                bdfAccelerators.InkMinBounds = accelerators.InkMinBounds.Copy();
                bdfAccelerators.InkMaxBounds = accelerators.InkMaxBounds.Copy();
            }
            else
            {
                var bdfInkMetrics = new List<PcfMetric>(glyphIndices.Count);
                foreach (var glyphIndex in glyphIndices)
                {
                    bdfInkMetrics.Add(inkMetrics[glyphIndex]);
                }

                bdfAccelerators.InkMinBounds = CalculateUtil.CalculateMinBounds(bdfInkMetrics);
                bdfAccelerators.InkMaxBounds = CalculateUtil.CalculateMaxBounds(bdfInkMetrics);
            }
            bdfAccelerators.TableFormat.InkBounds = true;
            bdfAccelerators.InkMetrics = true;
        }
        else
        {
            inkMetrics = null;

            accelerators.TableFormat.InkBounds = false;
            accelerators.InkMetrics = false;

            bdfAccelerators.TableFormat.InkBounds = false;
            bdfAccelerators.InkMetrics = false;
        }

        metrics.TableFormat.CompressedMetrics = accelerators.MinBounds.Compressible && accelerators.MaxBounds.Compressible;
        if (inkMetrics is not null)
        {
            inkMetrics.TableFormat.CompressedMetrics = accelerators.InkMinBounds!.Compressible && accelerators.InkMaxBounds!.Compressible;
        }

        return new PcfFont
        {
            Properties = properties,
            Accelerators = accelerators,
            Metrics = metrics,
            Bitmaps = bitmaps,
            InkMetrics = inkMetrics,
            BdfEncodings = bdfEncodings,
            ScalableWidths = scalableWidths,
            GlyphNames = glyphNames,
            BdfAccelerators = bdfAccelerators,
        };
    }

    public void Save(string path) => Build().Save(path);
}
