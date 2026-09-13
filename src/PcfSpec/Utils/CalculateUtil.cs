using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PcfSpec.Tests")]
namespace PcfSpec.Utils;

internal static class CalculateUtil
{
    public static int CalculateMaxOverlap(IList<PcfMetric> metrics)
    {
        int? maxOverlap = null;
        foreach (var metric in metrics)
        {
            var overlap = metric.RightSideBearing - metric.CharacterWidth;
            maxOverlap = maxOverlap is null ? overlap : Math.Max(maxOverlap.Value, overlap);
        }
        return maxOverlap ?? 0;
    }

    public static PcfMetric CalculateMinBounds(IList<PcfMetric> metrics)
    {
        PcfMetric? minBounds = null;
        ushort? attributes = null;
        foreach (var metric in metrics)
        {
            attributes = attributes is null ? metric.Attributes : (ushort)(attributes & metric.Attributes);
            if (metric is { LeftSideBearing: 0, RightSideBearing: 0, CharacterWidth: 0, Ascent: 0, Descent: 0 })
            {
                continue;
            }
            if (minBounds is null)
            {
                minBounds = metric.DeepCopy();
            }
            else
            {
                minBounds.LeftSideBearing = Math.Min(minBounds.LeftSideBearing, metric.LeftSideBearing);
                minBounds.RightSideBearing = Math.Min(minBounds.RightSideBearing, metric.RightSideBearing);
                minBounds.CharacterWidth = Math.Min(minBounds.CharacterWidth, metric.CharacterWidth);
                minBounds.Ascent = Math.Min(minBounds.Ascent, metric.Ascent);
                minBounds.Descent = Math.Min(minBounds.Descent, metric.Descent);
            }
        }
        minBounds ??= new PcfMetric();
        minBounds.Attributes = attributes ?? 0;
        return minBounds;
    }

    public static PcfMetric CalculateMaxBounds(IList<PcfMetric> metrics)
    {
        PcfMetric? maxBounds = null;
        ushort? attributes = null;
        foreach (var metric in metrics)
        {
            attributes = attributes is null ? metric.Attributes : (ushort)(attributes | metric.Attributes);
            if (metric is { LeftSideBearing: 0, RightSideBearing: 0, CharacterWidth: 0, Ascent: 0, Descent: 0 })
            {
                continue;
            }
            if (maxBounds is null)
            {
                maxBounds = metric.DeepCopy();
            }
            else
            {
                maxBounds.LeftSideBearing = Math.Max(maxBounds.LeftSideBearing, metric.LeftSideBearing);
                maxBounds.RightSideBearing = Math.Max(maxBounds.RightSideBearing, metric.RightSideBearing);
                maxBounds.CharacterWidth = Math.Max(maxBounds.CharacterWidth, metric.CharacterWidth);
                maxBounds.Ascent = Math.Max(maxBounds.Ascent, metric.Ascent);
                maxBounds.Descent = Math.Max(maxBounds.Descent, metric.Descent);
            }
        }
        maxBounds ??= new PcfMetric();
        maxBounds.Attributes = attributes ?? 0;
        return maxBounds;
    }
}
