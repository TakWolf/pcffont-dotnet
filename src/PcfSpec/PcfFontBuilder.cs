using PcfSpec.Tables;
using PcfSpec.Utils;

namespace PcfSpec;

public class PcfFontBuilder : ICopyable<PcfFontBuilder>, IEquatable<PcfFontBuilder>
{
    public static PcfFontBuilder Modify(PcfFont font)
    {
        var accelerators = font.Accelerators!;
        var bdfEncodings = font.BdfEncodings!;
        var glyphNames = font.GlyphNames!;
        var scalableWidths = font.ScalableWidths!;
        var metrics = font.Metrics!;
        var bitmaps = font.Bitmaps!;
        var properties = font.Properties!;

        var builder = new PcfFontBuilder();
        builder.Config.FontAscent = accelerators.FontAscent;
        builder.Config.FontDescent = accelerators.FontDescent;
        builder.Config.DefaultChar = bdfEncodings.DefaultChar;
        builder.Config.DrawRightToLeft = accelerators.DrawRightToLeft;
        builder.Config.MsByteFirst = bitmaps.TableFormat.MsByteFirst;
        builder.Config.MsBitFirst = bitmaps.TableFormat.MsBitFirst;
        builder.Config.GlyphPad = bitmaps.TableFormat.GlyphPad;
        builder.Config.ScanUnit = bitmaps.TableFormat.ScanUnit;

        builder.Properties = properties;

        var glyphIndexToEncoding = new Dictionary<ushort, ushort>(bdfEncodings.Count);
        foreach (var (encoding, glyphIndex) in bdfEncodings)
        {
            glyphIndexToEncoding[glyphIndex] = encoding;
        }

        for (var glyphIndex = 0; glyphIndex < glyphNames.Count; glyphIndex++)
        {
            var encoding = glyphIndexToEncoding.GetValueOrDefault((ushort)glyphIndex, PcfBdfEncodings.NoEncoding);
            var glyphName = glyphNames[glyphIndex];
            var scalableWidth = scalableWidths[glyphIndex];
            var metric = metrics[glyphIndex];
            var bitmap = bitmaps[glyphIndex];

            builder.Glyphs.Add(new PcfGlyph(
                name: glyphName,
                encoding: encoding,
                scalableWidth: scalableWidth,
                characterWidth: metric.CharacterWidth,
                dimensions: metric.Dimensions,
                offset: metric.Offset,
                attributes: metric.Attributes,
                bitmap: bitmap));
        }

        return builder;
    }

    public PcfFontConfig Config { get; set; }
    public PcfProperties Properties { get; set; }
    public List<PcfGlyph> Glyphs { get; set; }

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
        var tableFormat = Config.ToTableFormat();

        var bdfEncodings = new PcfBdfEncodings(
            tableFormat: tableFormat,
            defaultChar: Config.DefaultChar);
        var glyphNames = new PcfGlyphNames(
            tableFormat: tableFormat);
        var scalableWidths = new PcfScalableWidths(
            tableFormat: tableFormat);
        var metrics = new PcfMetrics(
            tableFormat: tableFormat);
        var bitmaps = new PcfBitmaps(
            tableFormat: tableFormat);
        var accelerators = new PcfAccelerators(
            tableFormat: tableFormat,
            drawRightToLeft: Config.DrawRightToLeft,
            fontAscent: Config.FontAscent,
            fontDescent: Config.FontDescent);
        var properties = new PcfProperties(
            Properties,
            tableFormat: tableFormat);

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
            bdfAccelerators = accelerators.DeepCopy();
        }
        else
        {
            var bdfMetrics = new List<PcfMetric>(glyphIndices.Count);
            foreach (var glyphIndex in glyphIndices)
            {
                bdfMetrics.Add(metrics[glyphIndex]);
            }

            bdfAccelerators = new PcfAccelerators(
                tableFormat: tableFormat,
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
            inkMetrics = new PcfMetrics(
                tableFormat: tableFormat);
            foreach (var glyph in Glyphs)
            {
                inkMetrics.Add(glyph.CreateMetric(true));
            }

            accelerators.InkMinBounds = CalculateUtil.CalculateMinBounds(inkMetrics);
            accelerators.InkMaxBounds = CalculateUtil.CalculateMaxBounds(inkMetrics);
            accelerators.TableFormat = accelerators.TableFormat.WithInkBounds(true);
            accelerators.InkMetrics = true;

            if (glyphIndices.Count == Glyphs.Count)
            {
                bdfAccelerators.InkMinBounds = accelerators.InkMinBounds.DeepCopy();
                bdfAccelerators.InkMaxBounds = accelerators.InkMaxBounds.DeepCopy();
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
            bdfAccelerators.TableFormat = bdfAccelerators.TableFormat.WithInkBounds(true);
            bdfAccelerators.InkMetrics = true;
        }
        else
        {
            inkMetrics = null;

            accelerators.TableFormat = accelerators.TableFormat.WithInkBounds(false);
            accelerators.InkMetrics = false;

            bdfAccelerators.TableFormat = bdfAccelerators.TableFormat.WithInkBounds(false);
            bdfAccelerators.InkMetrics = false;
        }

        metrics.TableFormat = metrics.TableFormat.WithCompressedMetrics(accelerators.MinBounds.Compressible && accelerators.MaxBounds.Compressible);
        if (inkMetrics is not null)
        {
            inkMetrics.TableFormat = inkMetrics.TableFormat.WithCompressedMetrics(accelerators.InkMinBounds!.Compressible && accelerators.InkMaxBounds!.Compressible);
        }

        return new PcfFont(
            properties,
            accelerators,
            metrics,
            bitmaps,
            inkMetrics,
            bdfEncodings,
            scalableWidths,
            glyphNames,
            bdfAccelerators);
    }

    public void Save(string path) => Build().Save(path);

    public PcfFontBuilder Copy() => new(
        Config,
        Properties,
        Glyphs);

    public PcfFontBuilder DeepCopy() => new(
        Config.DeepCopy(),
        Properties.DeepCopy(),
        CopyUtil.DeepCopyList(Glyphs));

    public bool Equals(PcfFontBuilder? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return Config.Equals(other.Config) &&
               Properties.Equals(other.Properties) &&
               EqualUtil.ListEquals(Glyphs, other.Glyphs);
    }

    public override bool Equals(object? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        if (other.GetType() != GetType())
        {
            return false;
        }
        return Equals((PcfFontBuilder)other);
    }

    public override int GetHashCode() => 0;
}
