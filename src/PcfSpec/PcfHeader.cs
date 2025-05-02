using PcfSpec.Error;
using PcfSpec.Util;

namespace PcfSpec;

public class PcfHeader : IComparable<PcfHeader>
{
    private static readonly byte[] FileVersion = "\u0001fcp"u8.ToArray();

    public static List<PcfHeader> Parse(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        if (!FileVersion.SequenceEqual(stream.ReadBytes(4)))
        {
            throw new PcfParseException("Data format not support.");
        }

        var headers = new List<PcfHeader>();
        var tableTypes = new HashSet<PcfTableType>();
        var tablesCount = stream.ReadUInt32();
        foreach (var _ in Enumerable.Range(0, (int)tablesCount))
        {
            var tableType = (PcfTableType)stream.ReadUInt32();
            if (!tableTypes.Add(tableType))
            {
                throw new PcfParseException($"Duplicate table '{tableType}'.");
            }
            var tableFormat = PcfTableFormat.Parse(stream.ReadUInt32());
            var tableSize = stream.ReadUInt32();
            var tableOffset = stream.ReadUInt32();
            headers.Add(new PcfHeader(tableType, tableFormat, tableSize, tableOffset));
        }
        headers.Sort();
        return headers;
    }

    public static void Dump(Stream stream, List<PcfHeader> headers)
    {
        stream.Seek(0, SeekOrigin.Begin);
        stream.WriteBytes(FileVersion);

        stream.WriteUInt32((uint)headers.Count);
        foreach (var header in headers)
        {
            stream.WriteUInt32((uint)header.TableType);
            stream.WriteUInt32(header.TableFormat.Value);
            stream.WriteUInt32(header.TableSize);
            stream.WriteUInt32(header.TableOffset);
        }
    }

    public PcfTableType TableType;
    public PcfTableFormat TableFormat;
    public uint TableSize;
    public uint TableOffset;

    public PcfHeader(
        PcfTableType tableType,
        PcfTableFormat tableFormat,
        uint tableSize,
        uint tableOffset)
    {
        TableType = tableType;
        TableFormat = tableFormat;
        TableSize = tableSize;
        TableOffset = tableOffset;
    }

    public PcfTableFormat ReadAndCheckTableFormat(Stream stream)
    {
        stream.Seek(TableOffset, SeekOrigin.Begin);
        var value = stream.ReadUInt32();
        if (value != TableFormat.Value)
        {
            throw new PcfParseException($"Inconsistent table format: '{TableType}'");
        }
        return TableFormat;
    }

    int IComparable<PcfHeader>.CompareTo(PcfHeader? other) => other is null ? 1 : TableType.CompareTo(other.TableType);
}
