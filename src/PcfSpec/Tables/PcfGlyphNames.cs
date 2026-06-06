using System.Runtime.InteropServices;
using PcfSpec.Utils;

namespace PcfSpec.Tables;

public class PcfGlyphNames : List<string>, IPcfTable, ICopyable<PcfGlyphNames>, IEquatable<PcfGlyphNames>
{
    public static PcfGlyphNames Parse(Stream stream, PcfHeader header, PcfFont font)
    {
        var tableFormat = header.ReadAndCheckTableFormat(stream);

        var glyphsCount = stream.ReadUInt32(tableFormat.MsByteFirst);
        var nameOffsets = stream.ReadUInt32Array((int)glyphsCount, tableFormat.MsByteFirst);
        stream.Seek(4, SeekOrigin.Current);  // stringsSize
        var stringsStart = stream.Position;

        var names = new List<string>((int)glyphsCount);
        foreach (var nameOffset in nameOffsets)
        {
            stream.Seek(stringsStart + nameOffset, SeekOrigin.Begin);
            var name = stream.ReadString();
            names.Add(name);
        }

        return new PcfGlyphNames(tableFormat, names);
    }

    public PcfTableFormat TableFormat { get; set; }

    public PcfGlyphNames(
        PcfTableFormat? tableFormat = null,
        IEnumerable<string>? names = null)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
        if (names is not null)
        {
            AddRange(names);
        }
    }

    public uint Dump(Stream stream, uint tableOffset, PcfFont font)
    {
        var glyphsCount = (uint)Count;

        var stringsStart = tableOffset + 4 + 4 + 4 * glyphsCount + 4;
        var stringsSize = 0;
        var nameOffsets = new List<uint>((int)glyphsCount);
        stream.Seek(stringsStart, SeekOrigin.Begin);
        foreach (var name in this)
        {
            nameOffsets.Add((uint)stringsSize);
            stringsSize += stream.WriteString(name);
        }

        stream.Seek(tableOffset, SeekOrigin.Begin);
        stream.WriteUInt32(TableFormat.Value);
        stream.WriteUInt32(glyphsCount, TableFormat.MsByteFirst);
        stream.WriteUInt32Array(CollectionsMarshal.AsSpan(nameOffsets), TableFormat.MsByteFirst);
        stream.WriteUInt32((uint)stringsSize, TableFormat.MsByteFirst);
        stream.Seek(stringsSize, SeekOrigin.Current);
        stream.AlignTo4Bytes();

        var tableSize = stream.Position - tableOffset;
        return (uint)tableSize;
    }

    public PcfGlyphNames Copy() => new(TableFormat, this);

    public PcfGlyphNames DeepCopy() => new(TableFormat.DeepCopy(), this);

    public bool Equals(PcfGlyphNames? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return TableFormat.Equals(other.TableFormat) &&
               EqualUtil.ListEquals(this, other);
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
        return Equals((PcfGlyphNames)other);
    }

    public override int GetHashCode() => 0;
}
