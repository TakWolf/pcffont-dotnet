namespace PcfSpec;

public class PcfGlyph
{
    public string Name;
    public ushort Encoding;
    public int ScalableWidth;
    public short CharacterWidth;
    public int Width;
    public int Height;
    public int OffsetX;
    public int OffsetY;
    public List<List<byte>> Bitmap;
    public ushort Attributes;

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
            if (bitmapRow.Any(color => color != 0))
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
            if (bitmapRow.Any(color => color != 0))
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
}
