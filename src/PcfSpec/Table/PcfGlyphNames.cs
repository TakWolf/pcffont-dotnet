using PcfSpec.Internal;

namespace PcfSpec.Table;

public class PcfGlyphNames : List<string>, IPcfTable
{
    public static PcfGlyphNames Parse(Stream stream, PcfFont font, PcfHeader header)
    {
        var tableFormat = header.ReadAndCheckTableFormat(stream);

        var glyphsCount = stream.ReadUInt32(tableFormat.MsByteFirst);
        var nameOffsets = stream.ReadUInt32List((int)glyphsCount, tableFormat.MsByteFirst);
        stream.Seek(4, SeekOrigin.Current);  // stringsSize
        var stringsStart = stream.Position;

        var names = new PcfGlyphNames(tableFormat);
        foreach (var nameOffset in nameOffsets)
        {
            stream.Seek(stringsStart + nameOffset, SeekOrigin.Begin);
            var name = stream.ReadString();
            names.Add(name);
        }
        return names;
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

    public uint Dump(Stream stream, PcfFont font, uint tableOffset)
    {
        var glyphsCount = (uint)Count;

        var stringsStart = tableOffset + 4 + 4 + 4 * glyphsCount + 4;
        var stringsSize = 0;
        var nameOffsets = new List<uint>();
        stream.Seek(stringsStart, SeekOrigin.Begin);
        foreach (var name in this)
        {
            nameOffsets.Add((uint)stringsSize);
            stringsSize += stream.WriteString(name);
        }

        stream.Seek(tableOffset, SeekOrigin.Begin);
        stream.WriteUInt32(TableFormat.Value);
        stream.WriteUInt32(glyphsCount, TableFormat.MsByteFirst);
        stream.WriteUInt32List(nameOffsets, TableFormat.MsByteFirst);
        stream.WriteUInt32((uint)stringsSize, TableFormat.MsByteFirst);
        stream.Seek(stringsSize, SeekOrigin.Current);
        stream.AlignToBit32WithNulls();

        var tableSize = stream.Position - tableOffset;
        return (uint)tableSize;
    }

    public static bool Equals(PcfGlyphNames? objA, PcfGlyphNames? objB)
    {
        if (objA == objB)
        {
            return true;
        }
        if (objA is null || objB is null)
        {
            return false;
        }
        return objA.TableFormat.Value == objB.TableFormat.Value &&
               objA.SequenceEqual(objB);
    }
}
