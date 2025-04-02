using PcfSpec.Table;

namespace PcfSpec;

public class PcfFont : Dictionary<PcfTableType, IPcfTable>
{
    private static readonly Dictionary<PcfTableType, (Type ClassType, IPcfTable.ParseDelegate Parse)> FactoryRegistry = new()
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

    public static PcfFont Parse(Stream stream)
    {
        // TODO
        return new PcfFont();
    }

    public static PcfFont Parse(byte[] buffer)
    {
        using var stream = new MemoryStream(buffer);
        return Parse(stream);
    }

    public static PcfFont Load(string path)
    {
        using var stream = File.OpenRead(path);
        return Parse(stream);
    }

    public void Dump(Stream stream)
    {
        // TODO
    }

    public byte[] DumpToBytes()
    {
        using var stream = new MemoryStream();
        Dump(stream);
        return stream.ToArray();
    }

    public void Save(string path)
    {
        using var stream = File.OpenWrite(path);
        Dump(stream);
    }
}
