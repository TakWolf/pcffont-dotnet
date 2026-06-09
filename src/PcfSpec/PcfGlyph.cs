using PcfSpec.Utils;

namespace PcfSpec;

public class PcfGlyph : ICopyable<PcfGlyph>, IEquatable<PcfGlyph>
{
    public string Name { get; set; }
    public ushort Encoding { get; set; }
    public int ScalableWidth { get; set; }
    public short CharacterWidth { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
    public List<List<byte>> Bitmap { get; set; }
    public ushort Attributes { get; set; }

    public PcfGlyph(
        string name,
        ushort encoding,
        int scalableWidth = 0,
        short characterWidth = 0,
        (int, int) dimensions = default,
        (int, int) offset = default,
        List<List<byte>>? bitmap = null,
        ushort attributes = 0)
    {
        Name = name;
        Encoding = encoding;
        ScalableWidth = scalableWidth;
        CharacterWidth = characterWidth;
        (Width, Height) = dimensions;
        (OffsetX, OffsetY) = offset;
        Bitmap = bitmap ?? [];
        Attributes = attributes;
    }

    public (int, int) Dimensions
    {
        get => (Width, Height);
        set => (Width, Height) = value;
    }

    public (int, int) Offset
    {
        get => (OffsetX, OffsetY);
        set => (OffsetX, OffsetY) = value;
    }

    public PcfMetric CreateMetric(bool isInk)
    {
        var metric = new PcfMetric(
            leftSideBearing: (short)OffsetX,
            rightSideBearing: (short)(OffsetX + Width),
            characterWidth: CharacterWidth,
            ascent: (short)(OffsetY + Height),
            descent: (short)-OffsetY,
            attributes: Attributes);

        if (!isInk)
        {
            return metric;
        }

        // Top
        foreach (var bitmapRow in Bitmap)
        {
            if (bitmapRow.Any(pixel => pixel != 0))
            {
                break;
            }
            metric.Ascent -= 1;
        }

        // Empty
        if (metric.Ascent + metric.Descent == 0)
        {
            metric.Ascent = 0;
            metric.Descent = 0;
            metric.RightSideBearing = metric.LeftSideBearing;
            return metric;
        }

        // Bottom
        for (var i = Bitmap.Count - 1; i >= 0; i--)
        {
            var bitmapRow = Bitmap[i];
            if (bitmapRow.Any(pixel => pixel != 0))
            {
                break;
            }
            metric.Descent -= 1;
        }

        // Left
        for (var i = 0; i < Width; i++)
        {
            if (Bitmap.Any(bitmapRow => bitmapRow[i] != 0))
            {
                break;
            }
            metric.LeftSideBearing += 1;
        }

        // Right
        for (var i = 0; i < Width; i++)
        {
            if (Bitmap.Any(bitmapRow => bitmapRow[Width - 1 - i] != 0))
            {
                break;
            }
            metric.RightSideBearing -= 1;
        }

        return metric;
    }

    public PcfGlyph Copy() => new(
        Name,
        Encoding,
        ScalableWidth,
        CharacterWidth,
        Dimensions,
        Offset,
        Bitmap,
        Attributes);

    public PcfGlyph DeepCopy() => new(
        Name,
        Encoding,
        ScalableWidth,
        CharacterWidth,
        Dimensions,
        Offset,
        CopyUtil.DeepCopyBitmap(Bitmap),
        Attributes);

    public bool Equals(PcfGlyph? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return Name == other.Name &&
               Encoding == other.Encoding &&
               ScalableWidth == other.ScalableWidth &&
               CharacterWidth == other.CharacterWidth &&
               Width == other.Width &&
               Height == other.Height &&
               OffsetX == other.OffsetX &&
               OffsetY == other.OffsetY &&
               Attributes == other.Attributes &&
               EqualUtil.BitmapEquals(Bitmap, other.Bitmap);
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
        return Equals((PcfGlyph)other);
    }

    public override int GetHashCode() => 0;
}
