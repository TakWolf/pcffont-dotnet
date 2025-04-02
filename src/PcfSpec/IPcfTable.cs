namespace PcfSpec;

public interface IPcfTable
{
    delegate IPcfTable ParseDelegate(Stream stream, PcfFont font, PcfHeader header);

    PcfTableFormat TableFormat { get; set; }

    uint Dump(Stream stream, PcfFont font, uint tableOffset);
}
