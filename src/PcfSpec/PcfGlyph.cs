namespace PcfSpec;

public class PcfGlyph
{
    public string Name;
    public ushort Encoding;
    public int ScalableWidth;
    public short CharacterWidth;
    public short Width;
    public short Height;
    public short OffsetX;
    public short OffsetY;
    public List<List<byte>> Bitmap;

    public PcfGlyph(
        string name,
        ushort encoding,
        int scalableWidth = 0,
        short characterWidth = 0,
        (short, short) dimensions = default,
        (short, short) offset = default,
        List<List<byte>>? bitmap = null)
    {
        Name = name;
        Encoding = encoding;
        ScalableWidth = scalableWidth;
        CharacterWidth = characterWidth;
        (Width, Height) = dimensions;
        (OffsetX, OffsetY) = offset;
        Bitmap = bitmap ?? [];
    }

    public (short, short) Dimensions
    {
        get => (Width, Height);
        set => (Width, Height) = value;
    }

    public (short, short) Offset
    {
        get => (OffsetX, OffsetY);
        set => (OffsetX, OffsetY) = value;
    }

    public PcfMetric CreateMetric(bool isInk)
    {
        var metric = new PcfMetric(
            leftSideBearing: OffsetX,
            rightSideBearing: (short)(OffsetX + Width),
            characterWidth: CharacterWidth,
            ascent: (short)(OffsetY + Height),
            descent: (short)-OffsetY);

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
        foreach (var bitmapRow in Enumerable.Reverse(Bitmap))
        {
            if (bitmapRow.Any(color => color != 0))
            {
                break;
            }
            metric.Descent -= 1;
        }

        // Left
        foreach (var i in Enumerable.Range(0, Width))
        {
            if (Bitmap.Any(bitmapRow => bitmapRow[i] != 0))
            {
                break;
            }
            metric.LeftSideBearing += 1;
        }

        // Right
        foreach (var i in Enumerable.Range(0, Width))
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
