using PcfSpec.Internal;

namespace PcfSpec.Table;

public class PcfMetrics : List<PcfMetric>, IPcfTable
{
    public static PcfMetrics Parse(Stream stream, PcfFont font, PcfHeader header)
    {
        var tableFormat = header.ReadAndCheckTableFormat(stream);

        uint glyphsCount;
        if (tableFormat.InkBoundsOrCompressedMetrics)
        {
            glyphsCount = stream.ReadUInt16(tableFormat.MsByteFirst);
        }
        else
        {
            glyphsCount = stream.ReadUInt32(tableFormat.MsByteFirst);
        }

        var metrics = new PcfMetrics(tableFormat);
        foreach (var _ in Enumerable.Range(0, (int)glyphsCount))
        {
            var metric = PcfMetric.Parse(stream, tableFormat.MsByteFirst, tableFormat.InkBoundsOrCompressedMetrics);
            metrics.Add(metric);
        }
        return metrics;
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

    public PcfMetric CalculateMinBounds()
    {
        PcfMetric? minBounds = null;
        foreach (var metric in this)
        {
            if (minBounds is null)
            {
                minBounds = new PcfMetric(
                    metric.LeftSideBearing,
                    metric.RightSideBearing,
                    metric.CharacterWidth,
                    metric.Ascent,
                    metric.Descent);
            }
            else
            {
                minBounds.LeftSideBearing = short.Min(minBounds.LeftSideBearing, metric.LeftSideBearing);
                minBounds.RightSideBearing = short.Min(minBounds.RightSideBearing, metric.RightSideBearing);
                minBounds.CharacterWidth = short.Min(minBounds.CharacterWidth, metric.CharacterWidth);
                minBounds.Ascent = short.Min(minBounds.Ascent, metric.Ascent);
                minBounds.Descent = short.Min(minBounds.Descent, metric.Descent);
            }
        }
        return minBounds ?? new PcfMetric(0, 0, 0, 0, 0);
    }

    public PcfMetric CalculateMaxBounds()
    {
        PcfMetric? maxBounds = null;
        foreach (var metric in this)
        {
            if (maxBounds is null)
            {
                maxBounds = new PcfMetric(
                    metric.LeftSideBearing,
                    metric.RightSideBearing,
                    metric.CharacterWidth,
                    metric.Ascent,
                    metric.Descent);
            }
            else
            {
                maxBounds.LeftSideBearing = short.Max(maxBounds.LeftSideBearing, metric.LeftSideBearing);
                maxBounds.RightSideBearing = short.Max(maxBounds.RightSideBearing, metric.RightSideBearing);
                maxBounds.CharacterWidth = short.Max(maxBounds.CharacterWidth, metric.CharacterWidth);
                maxBounds.Ascent = short.Max(maxBounds.Ascent, metric.Ascent);
                maxBounds.Descent = short.Max(maxBounds.Descent, metric.Descent);
            }
        }
        return maxBounds ?? new PcfMetric(0, 0, 0, 0, 0);
    }

    public int CalculateMaxOverlap() => Count > 0 ? this.Max(metric => metric.RightSideBearing - metric.CharacterWidth) : 0;

    public bool CalculateCompressible() => this.All(metric => metric.Compressible);

    public uint Dump(Stream stream, PcfFont font, uint tableOffset)
    {
        var glyphsCount = (uint)Count;

        stream.Seek(tableOffset, SeekOrigin.Begin);
        stream.WriteUInt32(TableFormat.Value);
        if (TableFormat.InkBoundsOrCompressedMetrics)
        {
            stream.WriteUInt16((ushort)glyphsCount, TableFormat.MsByteFirst);
        }
        else
        {
            stream.WriteUInt32(glyphsCount, TableFormat.MsByteFirst);
        }
        foreach (var metric in this)
        {
            metric.Dump(stream, TableFormat.MsByteFirst, TableFormat.InkBoundsOrCompressedMetrics);
        }
        stream.AlignToBit32WithNulls();

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
