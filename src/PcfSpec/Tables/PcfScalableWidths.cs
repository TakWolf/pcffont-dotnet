using System.Runtime.InteropServices;
using PcfSpec.Utils;

namespace PcfSpec.Tables;

public class PcfScalableWidths : List<int>, IPcfTable, ICopyable<PcfScalableWidths>, IEquatable<PcfScalableWidths>
{
    public static PcfScalableWidths Parse(Stream stream, PcfHeader header, PcfFont font)
    {
        var tableFormat = header.ReadAndCheckTableFormat(stream);

        var glyphsCount = stream.ReadUInt32(tableFormat.MsByteFirst);
        var scalableWidths = stream.ReadInt32Array((int)glyphsCount, tableFormat.MsByteFirst);

        return new PcfScalableWidths(scalableWidths, tableFormat);
    }

    public PcfTableFormat TableFormat { get; set; }

    public PcfScalableWidths(PcfTableFormat tableFormat = default) : this(0, tableFormat) { }

    public PcfScalableWidths(
        int capacity,
        PcfTableFormat tableFormat = default) : base(capacity)
    {
        TableFormat = tableFormat;
    }

    public PcfScalableWidths(
        IEnumerable<int> scalableWidths,
        PcfTableFormat tableFormat = default) : base(scalableWidths)
    {
        TableFormat = tableFormat;
    }

    public uint Dump(Stream stream, uint tableOffset, PcfFont font)
    {
        var glyphsCount = (uint)Count;

        stream.Seek(tableOffset, SeekOrigin.Begin);
        stream.WriteUInt32(TableFormat);
        stream.WriteUInt32(glyphsCount, TableFormat.MsByteFirst);
        stream.WriteInt32Array(CollectionsMarshal.AsSpan(this), TableFormat.MsByteFirst);
        stream.AlignTo4Bytes();

        var tableSize = stream.Position - tableOffset;
        return (uint)tableSize;
    }

    public PcfScalableWidths Copy() => new(this, TableFormat);

    public PcfScalableWidths DeepCopy() => Copy();

    public bool Equals(PcfScalableWidths? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return TableFormat == other.TableFormat &&
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
        return Equals((PcfScalableWidths)other);
    }

    public override int GetHashCode() => 0;
}
