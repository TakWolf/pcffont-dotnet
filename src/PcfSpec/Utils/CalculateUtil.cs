using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PcfSpec.Tests")]
namespace PcfSpec.Utils;

internal static class CalculateUtil
{
    public static int CalculateMaxOverlap(IList<PcfMetric> metrics) => metrics.Count > 0 ? metrics.Max(metric => metric.RightSideBearing - metric.CharacterWidth) : 0;

    public static PcfMetric CalculateMinBounds(IList<PcfMetric> metrics)
    {
        PcfMetric? minBounds = null;
        foreach (var metric in metrics)
        {
            if (minBounds is null)
            {
                minBounds = metric.DeepCopy();
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

    public static PcfMetric CalculateMaxBounds(IList<PcfMetric> metrics)
    {
        PcfMetric? maxBounds = null;
        foreach (var metric in metrics)
        {
            if (maxBounds is null)
            {
                maxBounds = metric.DeepCopy();
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
}
