using PcfSpec.Tables;

namespace PcfSpec;

public static class PcfTableFactory
{
    private static readonly Dictionary<PcfTableType, (Type ClassType, IPcfTable.ParseDelegate Parse)> Registry = new()
    {
        { PcfTableType.Properties, (typeof(PcfProperties), PcfProperties.Parse) },
        { PcfTableType.Accelerators, (typeof(PcfAccelerators), PcfAccelerators.Parse) },
        { PcfTableType.Metrics, (typeof(PcfMetrics), PcfMetrics.Parse) },
        { PcfTableType.Bitmaps, (typeof(PcfBitmaps), PcfBitmaps.Parse) },
        { PcfTableType.InkMetrics, (typeof(PcfMetrics), PcfMetrics.Parse) },
        { PcfTableType.BdfEncodings, (typeof(PcfBdfEncodings), PcfBdfEncodings.Parse) },
        { PcfTableType.ScalableWidths, (typeof(PcfScalableWidths), PcfScalableWidths.Parse) },
        { PcfTableType.GlyphNames, (typeof(PcfGlyphNames), PcfGlyphNames.Parse) },
        { PcfTableType.BdfAccelerators, (typeof(PcfAccelerators), PcfAccelerators.Parse) }
    };

    public static Type GetClassType(PcfTableType tableType) => Registry[tableType].ClassType;

    public static IPcfTable.ParseDelegate GetParse(PcfTableType tableType) => Registry[tableType].Parse;
}
