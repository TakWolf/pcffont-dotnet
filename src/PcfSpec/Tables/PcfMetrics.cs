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
                    metric.Descent,
                    metric.Attributes);
            }
            else
            {
                if (metric.LeftSideBearing != 0 || metric.RightSideBearing != 0 || metric.CharacterWidth != 0 || metric.Ascent != 0 || metric.Descent != 0) 
                {
                    minBounds.LeftSideBearing = short.Min(minBounds.LeftSideBearing, metric.LeftSideBearing);
                    minBounds.RightSideBearing = short.Min(minBounds.RightSideBearing, metric.RightSideBearing);
                    minBounds.CharacterWidth = short.Min(minBounds.CharacterWidth, metric.CharacterWidth);
                    minBounds.Ascent = short.Min(minBounds.Ascent, metric.Ascent);
                    minBounds.Descent = short.Min(minBounds.Descent, metric.Descent);
                }
                minBounds.Attributes &= metric.Attributes;
            }
        }
        return minBounds ?? new PcfMetric();
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
                    metric.Descent,
                    metric.Attributes);
            }
            else
            {
                if (metric.LeftSideBearing != 0 || metric.RightSideBearing != 0 || metric.CharacterWidth != 0 || metric.Ascent != 0 || metric.Descent != 0)
                {
                    maxBounds.LeftSideBearing = short.Max(maxBounds.LeftSideBearing, metric.LeftSideBearing);
                    maxBounds.RightSideBearing = short.Max(maxBounds.RightSideBearing, metric.RightSideBearing);
                    maxBounds.CharacterWidth = short.Max(maxBounds.CharacterWidth, metric.CharacterWidth);
                    maxBounds.Ascent = short.Max(maxBounds.Ascent, metric.Ascent);
                    maxBounds.Descent = short.Max(maxBounds.Descent, metric.Descent);   
                }
                maxBounds.Attributes |= metric.Attributes;
            }
        }
        return maxBounds ?? new PcfMetric();
    }

    public int CalculateMaxOverlap() => Count > 0 ? this.Max(metric => metric.RightSideBearing - metric.CharacterWidth) : 0;

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
