namespace PcfSpec;

public interface IPcfTable
{
    delegate IPcfTable ParseDelegate(Stream stream, PcfHeader header, PcfFont font);

    PcfTableFormat TableFormat { get; set; }

    uint Dump(Stream stream, uint tableOffset, PcfFont font);
}
