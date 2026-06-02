using PcfSpec.Utils;

namespace PcfSpec.Tables;

public class PcfMetrics : List<PcfMetric>, IPcfTable
{
    public static PcfMetrics Parse(Stream stream, PcfHeader header, PcfFont font)
    {
        var tableFormat = header.ReadAndCheckTableFormat(stream);

        uint glyphsCount;
        if (tableFormat.CompressedMetrics)
        {
            glyphsCount = stream.ReadUInt16(tableFormat.MsByteFirst);
        }
        else
        {
            glyphsCount = stream.ReadUInt32(tableFormat.MsByteFirst);
        }

        var metrics = new List<PcfMetric>((int)glyphsCount);
        for (var i = 0; i < glyphsCount; i++)
        {
            var metric = PcfMetric.Parse(stream, tableFormat.MsByteFirst, tableFormat.CompressedMetrics);
            metrics.Add(metric);
        }

        return new PcfMetrics(tableFormat, metrics);
    }

    public PcfTableFormat TableFormat { get; set; }

    public PcfMetrics(
        PcfTableFormat? tableFormat = null,
        IEnumerable<PcfMetric>? metrics = null)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
        if (metrics is not null)
        {
            AddRange(metrics);
        }
    }

    public uint Dump(Stream stream, uint tableOffset, PcfFont font)
    {
        var glyphsCount = (uint)Count;

        stream.Seek(tableOffset, SeekOrigin.Begin);
        stream.WriteUInt32(TableFormat.Value);
        if (TableFormat.CompressedMetrics)
        {
            stream.WriteUInt16((ushort)glyphsCount, TableFormat.MsByteFirst);
        }
        else
        {
            stream.WriteUInt32(glyphsCount, TableFormat.MsByteFirst);
        }
        foreach (var metric in this)
        {
            metric.Dump(stream, TableFormat.MsByteFirst, TableFormat.CompressedMetrics);
        }
        stream.AlignTo4Bytes();

        var tableSize = stream.Position - tableOffset;
        return (uint)tableSize;
    }

    private static bool MetricListEquals(List<PcfMetric> objA, List<PcfMetric> objB)
    {
        foreach (var (metricA, metricB) in objA.Zip(objB))
        {
            if (!PcfMetric.Equals(metricA, metricB))
            {
                return false;
            }
        }
        return true;
    }

    public static bool Equals(PcfMetrics? objA, PcfMetrics? objB)
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
               MetricListEquals(objA, objB);
    }
}
