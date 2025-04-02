namespace PcfSpec.Table;

public class PcfGlyphNames : IPcfTable
{
    public static PcfGlyphNames Parse(Stream stream, PcfFont font, PcfHeader header)
    {
        // TODO
        return new PcfGlyphNames();
    }

    public PcfTableFormat TableFormat { get; set; }

    public PcfGlyphNames(PcfTableFormat? tableFormat = null)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
    }

    public uint Dump(Stream stream, PcfFont font, uint tableOffset)
    {
        // TODO
        return 0;
    }
}
