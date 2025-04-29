using PcfSpec.Util;

namespace PcfSpec.Table;

public class PcfBitmaps : List<List<List<byte>>>, IPcfTable
{
    private static readonly uint[] GlyphPadOptions = [1, 2, 4, 8];
    private static readonly uint[] ScanUnitOptions = [1, 2, 4, 8];

    private static void SwapFragments(List<List<byte>> fragments, uint scanUnit)
    {
        switch (scanUnit)
        {
            case 2:
                for (var i = 0; i < fragments.Count; i += 2)
                {
                    (fragments[i], fragments[i + 1]) = (fragments[i + 1], fragments[i]);
                }
                break;
            case 4:
                for (var i = 0; i < fragments.Count; i += 4)
                {
                    (fragments[i], fragments[i + 1], fragments[i + 2], fragments[i + 3]) = (fragments[i + 3], fragments[i + 2], fragments[i + 1], fragments[i]);
                }
                break;
        }
    }

    public static PcfBitmaps Parse(Stream stream, PcfHeader header, PcfFont font)
    {
        var tableFormat = header.ReadAndCheckTableFormat(stream);

        var glyphPad = GlyphPadOptions[tableFormat.GlyphPadIndex];
        var scanUnit = ScanUnitOptions[tableFormat.ScanUnitIndex];

        var glyphsCount = stream.ReadUInt32(tableFormat.MsByteFirst);
        var bitmapOffsets = stream.ReadUInt32List((int)glyphsCount, tableFormat.MsByteFirst);
        var bitmapsSizes = stream.ReadUInt32List(4, tableFormat.MsByteFirst);
        var bitmapsStart = stream.Position;

        var bitmaps = new PcfBitmaps(tableFormat);
        foreach (var (bitmapOffset, metric) in bitmapOffsets.Zip(font.Metrics!))
        {
            stream.Seek(bitmapsStart + bitmapOffset, SeekOrigin.Begin);
            var glyphRowPad = (int)(Math.Ceiling(metric.Width / (double)(glyphPad * 8)) * glyphPad);

            var fragments = stream.ReadBinaryList(glyphRowPad * metric.Height, tableFormat.MsBitFirst);
            if (tableFormat.MsByteFirst != tableFormat.MsBitFirst)
            {
                SwapFragments(fragments, scanUnit);
            }

            var bitmap = new List<List<byte>>();
            foreach (var y in Enumerable.Range(0, metric.Height))
            {
                var bitmapRow = new List<byte>();
                foreach (var i in Enumerable.Range(0, glyphRowPad))
                {
                    bitmapRow.AddRange(fragments[glyphRowPad * y + i]);
                }
                bitmapRow = bitmapRow[..metric.Width];
                bitmap.Add(bitmapRow);
            }
            bitmaps.Add(bitmap);
        }

        // Compat
        bitmaps.CompatInfo = bitmapsSizes;

        return bitmaps;
    }

    public PcfTableFormat TableFormat { get; set; }
    public List<uint>? CompatInfo;

    public PcfBitmaps(
        PcfTableFormat? tableFormat = null,
        IEnumerable<List<List<byte>>>? bitmaps = null)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
        if (bitmaps is not null)
        {
            AddRange(bitmaps);
        }
    }

    public uint Dump(Stream stream, uint tableOffset, PcfFont font)
    {
        var glyphPad = GlyphPadOptions[TableFormat.GlyphPadIndex];
        var scanUnit = ScanUnitOptions[TableFormat.ScanUnitIndex];

        var glyphsCount = (uint)Count;

        var bitmapsStart = tableOffset + 4 + 4 + 4 * glyphsCount + 4 * 4;
        var bitmapsSize = 0u;
        var bitmapOffsets = new List<uint>();
        stream.Seek(bitmapsStart, SeekOrigin.Begin);
        foreach (var (bitmap, metric) in this.Zip(font.Metrics!))
        {
            bitmapOffsets.Add(bitmapsSize);
            var bitmapRowWidth = (int)(Math.Ceiling(metric.Width / (double)(glyphPad * 8)) * glyphPad * 8);

            var fragments = new List<List<byte>>();
            foreach (var bitmapRow in bitmap)
            {
                var bitmapRowAligned = new List<byte>(bitmapRow.Take(bitmapRowWidth));
                if (bitmapRowAligned.Count < bitmapRowWidth)
                {
                    bitmapRowAligned.AddRange(Enumerable.Repeat((byte)0, bitmapRowWidth - bitmapRowAligned.Count));
                }
                for (var i = 0; i < bitmapRowWidth; i += 8)
                {
                    fragments.Add(bitmapRowAligned[i..(i + 8)]);
                }
            }

            if (TableFormat.MsByteFirst != TableFormat.MsBitFirst)
            {
                SwapFragments(fragments, scanUnit);
            }

            bitmapsSize += (uint)stream.WriteBinaryList(fragments, TableFormat.MsBitFirst);
        }

        // Compat
        var bitmapsSizes = new List<uint>();
        if (CompatInfo is not null)
        {
            bitmapsSizes.AddRange(CompatInfo);
            bitmapsSizes[TableFormat.GlyphPadIndex] = bitmapsSize;
        }
        else
        {
            bitmapsSizes.AddRange(GlyphPadOptions.Select(glyphPadOption => bitmapsSize / glyphPad * glyphPadOption));
        }

        stream.Seek(tableOffset, SeekOrigin.Begin);
        stream.WriteUInt32(TableFormat.Value);
        stream.WriteUInt32(glyphsCount, TableFormat.MsByteFirst);
        stream.WriteUInt32List(bitmapOffsets, TableFormat.MsByteFirst);
        stream.WriteUInt32List(bitmapsSizes, TableFormat.MsByteFirst);
        stream.Seek(bitmapsSize, SeekOrigin.Current);
        stream.AlignTo4ByteWithNulls();

        var tableSize = stream.Position - tableOffset;
        return (uint)tableSize;
    }

    private static bool BitmapListEquals(List<List<List<byte>>> objA, List<List<List<byte>>> objB)
    {
        foreach (var (bitmapA, bitmapB) in objA.Zip(objB))
        {
            foreach (var (bitmapRowA, bitmapRowB) in bitmapA.Zip(bitmapB))
            {
                if (!bitmapRowA.SequenceEqual(bitmapRowB))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static bool CompatInfoEquals(List<uint>? objA, List<uint>? objB)
    {
        if (objA == objB)
        {
            return true;
        }
        if (objA is null || objB is null)
        {
            return false;
        }
        return objA.SequenceEqual(objB);
    }

    public static bool Equals(PcfBitmaps? objA, PcfBitmaps? objB)
    {
        if (objA == objB)
        {
            return true;
        }
        if (objA is null || objB is null)
        {
            return false;
        }
        return objA.TableFormat.Value == objB.TableFormat.Value &&
               BitmapListEquals(objA, objB) &&
               CompatInfoEquals(objA.CompatInfo, objB.CompatInfo);
    }
}
