using System.Buffers.Binary;
using PcfSpec.Errors;
using PcfSpec.Utils;

namespace PcfSpec;

public class PcfHeader : IComparable<PcfHeader>, ICopyable<PcfHeader>, IEquatable<PcfHeader>
{
    private static readonly uint FileVersion = BinaryPrimitives.ReadUInt32LittleEndian("\u0001fcp"u8);

    public static List<PcfHeader> Parse(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        if (stream.ReadUInt32() != FileVersion)
        {
            throw new PcfParseException("Not a valid PCF font.");
        }

        var tablesCount = stream.ReadUInt32();
        var tableTypes = new HashSet<PcfTableType>((int)tablesCount);
        var headers = new List<PcfHeader>((int)tablesCount);
        for (var i = 0; i < tablesCount; i++)
        {
            var tableType = (PcfTableType)stream.ReadUInt32();
            if (!tableTypes.Add(tableType))
            {
                throw new PcfParseException($"Duplicate table '{tableType}'.");
            }
            PcfTableFormat tableFormat = stream.ReadUInt32();
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
        stream.WriteUInt32(FileVersion);

        stream.WriteUInt32((uint)headers.Count);
        foreach (var header in headers)
        {
            stream.WriteUInt32((uint)header.TableType);
            stream.WriteUInt32(header.TableFormat);
            stream.WriteUInt32(header.TableSize);
            stream.WriteUInt32(header.TableOffset);
        }
    }

    public PcfTableType TableType { get; set; }
    public PcfTableFormat TableFormat { get; set; }
    public uint TableSize { get; set; }
    public uint TableOffset { get; set; }

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
        PcfTableFormat value = stream.ReadUInt32();
        if (value != TableFormat)
        {
            throw new PcfParseException($"Inconsistent table format: '{TableType}'");
        }
        return TableFormat;
    }

    public int CompareTo(PcfHeader? other) => other is null ? 1 : TableType.CompareTo(other.TableType);

    public PcfHeader Copy() => new(
        TableType,
        TableFormat,
        TableSize,
        TableOffset);

    public PcfHeader DeepCopy() => Copy();

    public bool Equals(PcfHeader? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return TableType == other.TableType &&
               TableFormat == other.TableFormat &&
               TableSize == other.TableSize &&
               TableOffset == other.TableOffset;
    }

    public override bool Equals(object? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        if (other.GetType() != GetType())
        {
            return false;
        }
        return Equals((PcfHeader)other);
    }

    public override int GetHashCode() => 0;
}
