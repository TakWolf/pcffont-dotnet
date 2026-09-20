using PcfSpec.Utils;

namespace PcfSpec.Tables;

public class PcfMetrics : List<PcfMetric>, IPcfTable, ICopyable<PcfMetrics>, IEquatable<PcfMetrics>
{
    public static PcfMetrics Parse(Stream stream, PcfHeader header, PcfFont font)
    {
        var tableFormat = header.ReadAndCheckTableFormat(stream);

        uint glyphCount;
        if (tableFormat.CompressedMetrics)
        {
            glyphCount = stream.ReadUInt16(tableFormat.MsByteFirst);
        }
        else
        {
            glyphCount = stream.ReadUInt32(tableFormat.MsByteFirst);
        }

        var metrics = new PcfMetrics((int)glyphCount, tableFormat);
        for (var i = 0; i < glyphCount; i++)
        {
            var metric = PcfMetric.Parse(stream, tableFormat.MsByteFirst, tableFormat.CompressedMetrics);
            metrics.Add(metric);
        }
        return metrics;
    }

    public PcfTableFormat TableFormat { get; set; }

    public PcfMetrics(PcfTableFormat tableFormat = default) : this(0, tableFormat) { }

    public PcfMetrics(
        int capacity,
        PcfTableFormat tableFormat = default) : base(capacity)
    {
        TableFormat = tableFormat;
    }

    public PcfMetrics(
        IEnumerable<PcfMetric> metrics,
        PcfTableFormat tableFormat = default) : base(metrics)
    {
        TableFormat = tableFormat;
    }

    public uint Dump(Stream stream, uint tableOffset, PcfFont font)
    {
        var glyphCount = (uint)Count;

        stream.Seek(tableOffset, SeekOrigin.Begin);
        stream.WriteUInt32(TableFormat);
        if (TableFormat.CompressedMetrics)
        {
            stream.WriteUInt16((ushort)glyphCount, TableFormat.MsByteFirst);
        }
        else
        {
            stream.WriteUInt32(glyphCount, TableFormat.MsByteFirst);
        }
        foreach (var metric in this)
        {
            metric.Dump(stream, TableFormat.MsByteFirst, TableFormat.CompressedMetrics);
        }
        stream.AlignTo4Bytes();

        var tableSize = stream.Position - tableOffset;
        return (uint)tableSize;
    }

    public PcfMetrics Copy() => new(this, TableFormat);

    public PcfMetrics DeepCopy()
    {
        var metrics = new PcfMetrics(Count, TableFormat);
        CopyUtil.DeepCopyToList(this, metrics);
        return metrics;
    }

    public bool Equals(PcfMetrics? other)
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
        return Equals((PcfMetrics)other);
    }

    public override int GetHashCode() => 0;
}
