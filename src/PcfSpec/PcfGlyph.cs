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
    public ushort Attributes { get; set; }
    public List<List<byte>> Bitmap { get; set; }

    public PcfGlyph(
        string name,
        ushort encoding,
        int scalableWidth = 0,
        short characterWidth = 0,
        (int, int) dimensions = default,
        (int, int) offset = default,
        ushort attributes = 0,
        List<List<byte>>? bitmap = null)
    {
        Name = name;
        Encoding = encoding;
        ScalableWidth = scalableWidth;
        CharacterWidth = characterWidth;
        (Width, Height) = dimensions;
        (OffsetX, OffsetY) = offset;
        Attributes = attributes;
        Bitmap = bitmap ?? [];
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

        var firstRow = Height;
        var lastRow = -1;
        var firstCol = Width;
        var lastCol = -1;

        for (var y = 0; y < Height; y++)
        {
            if (y >= Bitmap.Count)
            {
                break;
            }
            var bitmapRow = Bitmap[y];
            var widthLimit = Math.Min(bitmapRow.Count, Width);
            for (var x = 0; x < widthLimit; x++)
            {
                if (bitmapRow[x] != 0)
                {
                    if (y < firstRow)
                    {
                        firstRow = y;
                    }
                    if (y > lastRow)
                    {
                        lastRow = y;
                    }
                    if (x < firstCol)
                    {
                        firstCol = x;
                    }
                    if (x > lastCol)
                    {
                        lastCol = x;
                    }
                }
            }
        }

        if (firstRow == Height)
        {
            metric.Ascent = 0;
            metric.Descent = 0;
            metric.RightSideBearing = metric.LeftSideBearing;
            return metric;
        }

        metric.Ascent -= (short)firstRow;
        metric.Descent -= (short)(lastRow != -1 ? Height - 1 - lastRow : Height);
        metric.LeftSideBearing += (short)firstCol;
        metric.RightSideBearing -= (short)(lastCol != -1 ? Width - 1 - lastCol : Width);
        return metric;
    }

    public PcfGlyph Copy() => new(
        Name,
        Encoding,
        ScalableWidth,
        CharacterWidth,
        Dimensions,
        Offset,
        Attributes,
        Bitmap);

    public PcfGlyph DeepCopy() => new(
        Name,
        Encoding,
        ScalableWidth,
        CharacterWidth,
        Dimensions,
        Offset,
        Attributes,
        CopyUtil.DeepCopyBitmap(Bitmap));

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
