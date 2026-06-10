using System.Runtime.InteropServices;
using PcfSpec.Utils;

namespace PcfSpec.Tables;

public class PcfBitmaps : List<List<List<byte>>>, IPcfTable, ICopyable<PcfBitmaps>, IEquatable<PcfBitmaps>
{
    private static void SwapBytes(byte[] data, uint scanUnit)
    {
        if (scanUnit <= 1)
        {
            return;
        }

        for (var i = 0; i < data.Length / scanUnit * scanUnit; i += (int)scanUnit)
        {
            for (var j = 0; j < scanUnit / 2; j++)
            {
                var left = i + j;
                var right = i + scanUnit - 1 - j;
                (data[left], data[right]) = (data[right], data[left]);
            }
        }
    }

    public static PcfBitmaps Parse(Stream stream, PcfHeader header, PcfFont font)
    {
        var tableFormat = header.ReadAndCheckTableFormat(stream);

        var glyphsCount = stream.ReadUInt32(tableFormat.MsByteFirst);
        var bitmapOffsets = stream.ReadUInt32Array((int)glyphsCount, tableFormat.MsByteFirst);
        stream.Seek(16, SeekOrigin.Current);  // bitmapsSizeConfigs
        var bitmapsStart = stream.Position;

        var bitmaps = new PcfBitmaps((int)glyphsCount, tableFormat);
        for (var glyphIndex = 0; glyphIndex < glyphsCount; glyphIndex++)
        {
            var bitmapOffset = bitmapOffsets[glyphIndex];
            var metric = font.Metrics![glyphIndex];

            var bitmapRowSize = (int)((metric.Width + tableFormat.GlyphPad * 8 - 1) / (tableFormat.GlyphPad * 8) * tableFormat.GlyphPad);

            stream.Seek(bitmapsStart + bitmapOffset, SeekOrigin.Begin);
            var bitmapData = stream.ReadBytes(bitmapRowSize * metric.Height);

            if (tableFormat.MsByteFirst != tableFormat.MsBitFirst)
            {
                SwapBytes(bitmapData, tableFormat.ScanUnit);
            }

            var bitmap = new List<List<byte>>(metric.Height);
            for (var y = 0; y < metric.Height; y++)
            {
                var bitmapRow = new List<byte>(bitmapRowSize * 8);
                for (var i = 0; i < bitmapRowSize; i++)
                {
                    var b = bitmapData[y * bitmapRowSize + i];
                    if (tableFormat.MsBitFirst)
                    {
                        for (var shift = 7; shift >= 0; shift--)
                        {
                            bitmapRow.Add((byte)((b >> shift) & 1));
                        }
                    }
                    else
                    {
                        for (var shift = 0; shift < 8; shift++)
                        {
                            bitmapRow.Add((byte)((b >> shift) & 1));
                        }
                    }
                }
                if (bitmapRow.Count > metric.Width)
                {
                    bitmapRow.RemoveRange(metric.Width, bitmapRow.Count - metric.Width);
                }
                bitmap.Add(bitmapRow);
            }
            bitmaps.Add(bitmap);
        }
        return bitmaps;
    }

    public PcfTableFormat TableFormat { get; set; }

    public PcfBitmaps(PcfTableFormat? tableFormat = null) : this(0, tableFormat) { }

    public PcfBitmaps(
        int capacity,
        PcfTableFormat? tableFormat = null) : base(capacity)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
    }

    public PcfBitmaps(
        IEnumerable<List<List<byte>>> bitmaps,
        PcfTableFormat? tableFormat = null) : base(bitmaps)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
    }

    public uint Dump(Stream stream, uint tableOffset, PcfFont font)
    {
        var glyphsCount = (uint)Count;

        var bitmapsStart = tableOffset + 4 + 4 + 4 * glyphsCount + 4 * 4;
        var bitmapsSize = 0u;
        var bitmapOffsets = new List<uint>((int)glyphsCount);
        var bitmapsSizeConfigs = new uint[4];
        stream.Seek(bitmapsStart, SeekOrigin.Begin);
        for (var glyphIndex = 0; glyphIndex < glyphsCount; glyphIndex++)
        {
            var bitmap = this[glyphIndex];
            var metric = font.Metrics![glyphIndex];

            for (var glyphPadIndex = 0; glyphPadIndex < PcfTableFormat.GlyphPadOptions.Length; glyphPadIndex++)
            {
                var glyphPad = PcfTableFormat.GlyphPadOptions[glyphPadIndex];
                bitmapsSizeConfigs[glyphPadIndex] += (uint)((metric.Width + glyphPad * 8 - 1) / (glyphPad * 8) * glyphPad * metric.Height);
            }

            var bitmapRowSize = (int)((metric.Width + TableFormat.GlyphPad * 8 - 1) / (TableFormat.GlyphPad * 8) * TableFormat.GlyphPad);

            var bitmapData = new byte[bitmapRowSize * metric.Height];
            for (var y = 0; y < metric.Height; y++)
            {
                if (y >= bitmap.Count)
                {
                    Array.Fill(bitmapData, (byte)0, y * bitmapRowSize, bitmapRowSize);
                    continue;
                }

                var bitmapRow = bitmap[y];
                var widthLimit = Math.Min(bitmapRow.Count, metric.Width);
                for (var i = 0; i < bitmapRowSize; i++)
                {
                    byte b = 0;
                    if (TableFormat.MsBitFirst)
                    {
                        for (var shift = 0; shift < 8; shift++)
                        {
                            var pixelIndex = i * 8 + shift;
                            var pixel = pixelIndex < widthLimit && bitmapRow[pixelIndex] != 0 ? 1 : 0;
                            b = (byte)((b << 1) | pixel);
                        }
                    }
                    else
                    {
                        for (var shift = 7; shift >= 0; shift--)
                        {
                            var pixelIndex = i * 8 + shift;
                            var pixel = pixelIndex < widthLimit && bitmapRow[pixelIndex] != 0 ? 1 : 0;
                            b = (byte)((b << 1) | pixel);
                        }
                    }
                    bitmapData[y * bitmapRowSize + i] = b;
                }
            }

            if (TableFormat.MsByteFirst != TableFormat.MsBitFirst)
            {
                SwapBytes(bitmapData, TableFormat.ScanUnit);
            }

            bitmapOffsets.Add(bitmapsSize);
            bitmapsSize += (uint)stream.WriteBytes(bitmapData);
        }

        stream.Seek(tableOffset, SeekOrigin.Begin);
        stream.WriteUInt32(TableFormat.Value);
        stream.WriteUInt32(glyphsCount, TableFormat.MsByteFirst);
        stream.WriteUInt32Array(CollectionsMarshal.AsSpan(bitmapOffsets), TableFormat.MsByteFirst);
        stream.WriteUInt32Array(bitmapsSizeConfigs, TableFormat.MsByteFirst);
        stream.Seek(bitmapsSize, SeekOrigin.Current);
        stream.AlignTo4Bytes();

        var tableSize = stream.Position - tableOffset;
        return (uint)tableSize;
    }

    public PcfBitmaps Copy() => new(this, TableFormat);

    public PcfBitmaps DeepCopy() => new(CopyUtil.DeepCopyList(this), TableFormat.DeepCopy());

    public bool Equals(PcfBitmaps? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return TableFormat.Equals(other.TableFormat) &&
               EqualUtil.ListEquals(this, other);
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
        return Equals((PcfBitmaps)other);
    }

    public override int GetHashCode() => 0;
}
