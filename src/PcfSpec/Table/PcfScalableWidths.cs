using PcfSpec.Util;

namespace PcfSpec.Table;

public class PcfScalableWidths : List<int>, IPcfTable
{
    public static PcfScalableWidths Parse(Stream stream, PcfHeader header, PcfFont font)
    {
        var tableFormat = header.ReadAndCheckTableFormat(stream);

        var glyphsCount = stream.ReadUInt32(tableFormat.MsByteFirst);

        var scalableWidths = Enumerable.Range(0, (int)glyphsCount).Select(_ => stream.ReadInt32(tableFormat.MsByteFirst)).ToList();

        return new PcfScalableWidths(tableFormat, scalableWidths);
    }

    public PcfTableFormat TableFormat { get; set; }

    public PcfScalableWidths(
        PcfTableFormat? tableFormat = null,
        IEnumerable<int>? scalableWidths = null)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
        if (scalableWidths is not null)
        {
            AddRange(scalableWidths);
        }
    }

    public uint Dump(Stream stream, uint tableOffset, PcfFont font)
    {
        var glyphsCount = (uint)Count;

        stream.Seek(tableOffset, SeekOrigin.Begin);
        stream.WriteUInt32(TableFormat.Value);
        stream.WriteUInt32(glyphsCount, TableFormat.MsByteFirst);
        foreach (var scalableWidth in this)
        {
            stream.WriteInt32(scalableWidth, TableFormat.MsByteFirst);
        }
        stream.AlignTo4ByteWithNulls();

        var tableSize = stream.Position - tableOffset;
        return (uint)tableSize;
    }

    public static bool Equals(PcfScalableWidths? objA, PcfScalableWidths? objB)
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
               objA.SequenceEqual(objB);
    }
}
