using System.Collections;
using PcfSpec.Utils;

namespace PcfSpec.Tables;

public class PcfBdfEncodings : IDictionary<ushort, ushort>, IPcfTable, ICopyable<PcfBdfEncodings>, IEquatable<PcfBdfEncodings>
{
    public const ushort NoEncoding = ushort.MaxValue;
    public const ushort NoGlyphIndex = ushort.MaxValue;

    public static PcfBdfEncodings Parse(Stream stream, PcfHeader header, PcfFont font)
    {
        var tableFormat = header.ReadAndCheckTableFormat(stream);

        var minByte2 = stream.ReadUInt16(tableFormat.MsByteFirst);
        var maxByte2 = stream.ReadUInt16(tableFormat.MsByteFirst);
        var minByte1 = stream.ReadUInt16(tableFormat.MsByteFirst);
        var maxByte1 = stream.ReadUInt16(tableFormat.MsByteFirst);
        var defaultChar = stream.ReadUInt16(tableFormat.MsByteFirst);

        var glyphsCount = (maxByte2 - minByte2 + 1) * (maxByte1 - minByte1 + 1);
        var glyphIndices = stream.ReadUInt16Array(glyphsCount, tableFormat.MsByteFirst);

        var encodings = new PcfBdfEncodings(glyphsCount, tableFormat, defaultChar);
        if (minByte1 == 0 && maxByte1 == 0)
        {
            for (var encoding = (int)minByte2; encoding <= maxByte2; encoding++)
            {
                var glyphIndex = glyphIndices[encoding - minByte2];
                encodings[(ushort)encoding] = glyphIndex;
            }
        }
        else
        {
            for (var byte1 = (int)minByte1; byte1 <= maxByte1; byte1++)
            {
                for (var byte2 = (int)minByte2; byte2 <= maxByte2; byte2++)
                {
                    var encoding = (ushort)((byte1 << 8) | byte2);
                    var glyphIndex = glyphIndices[(byte1 - minByte1) * (maxByte2 - minByte2 + 1) + byte2 - minByte2];
                    encodings[encoding] = glyphIndex;
                }
            }
        }
        return encodings;
    }

    private readonly SortedDictionary<ushort, ushort> _encodings;

    public PcfTableFormat TableFormat { get; set; }
    public ushort DefaultChar { get; set; }

    public PcfBdfEncodings(
        PcfTableFormat tableFormat = default,
        ushort defaultChar = NoEncoding) : this(0, tableFormat, defaultChar) { }

    public PcfBdfEncodings(
        int capacity,
        PcfTableFormat tableFormat = default,
        ushort defaultChar = NoEncoding)
    {
        _encodings = new SortedDictionary<ushort, ushort>();
        TableFormat = tableFormat;
        DefaultChar = defaultChar;
    }

    public PcfBdfEncodings(
        IDictionary<ushort, ushort> encodings,
        PcfTableFormat tableFormat = default,
        ushort defaultChar = NoEncoding) : this(encodings.Count, tableFormat, defaultChar)
    {
        foreach (var (encoding, glyphIndex) in encodings)
        {
            this[encoding] = glyphIndex;
        }
    }

    public PcfBdfEncodings(
        IEnumerable<KeyValuePair<ushort, ushort>> encodings,
        PcfTableFormat tableFormat = default,
        ushort defaultChar = NoEncoding) :
        this((encodings as ICollection<KeyValuePair<ushort, ushort>>)?.Count ?? 0, tableFormat, defaultChar)
    {
        foreach (var (encoding, glyphIndex) in encodings)
        {
            this[encoding] = glyphIndex;
        }
    }

    public int Count => _encodings.Count;

    bool ICollection<KeyValuePair<ushort, ushort>>.IsReadOnly => false;

    public ICollection<ushort> Keys => _encodings.Keys;

    public ICollection<ushort> Values => _encodings.Values;

    public IEnumerator<KeyValuePair<ushort, ushort>> GetEnumerator() => _encodings.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public ushort this[ushort key]
    {
        get => _encodings[key];
        set
        {
            if (value == NoGlyphIndex)
            {
                _encodings.Remove(key);
                return;
            }
            _encodings[key] = value;
        }
    }

    public bool TryGetValue(ushort key, out ushort value)
    {
        if (_encodings.TryGetValue(key, out value))
        {
            return true;
        }
        value = NoGlyphIndex;
        return false;
    }

    public bool ContainsKey(ushort key) => _encodings.ContainsKey(key);

    public bool ContainsValue(ushort value) => _encodings.ContainsValue(value);

    bool ICollection<KeyValuePair<ushort, ushort>>.Contains(KeyValuePair<ushort, ushort> item) => (_encodings as ICollection<KeyValuePair<ushort, ushort>>).Contains(item);

    public void Add(ushort key, ushort value) => this[key] = value;

    void ICollection<KeyValuePair<ushort, ushort>>.Add(KeyValuePair<ushort, ushort> item) => this[item.Key] = item.Value;

    public bool Remove(ushort key) => _encodings.Remove(key);

    bool ICollection<KeyValuePair<ushort, ushort>>.Remove(KeyValuePair<ushort, ushort> item) => (_encodings as ICollection<KeyValuePair<ushort, ushort>>).Remove(item);

    public void Clear() => _encodings.Clear();

    void ICollection<KeyValuePair<ushort, ushort>>.CopyTo(KeyValuePair<ushort, ushort>[] array, int arrayIndex) => (_encodings as ICollection<KeyValuePair<ushort, ushort>>).CopyTo(array, arrayIndex);

    public uint Dump(Stream stream, uint tableOffset, PcfFont font)
    {
        byte minByte2 = 0xFF;
        byte maxByte2 = 0;
        byte minByte1 = 0xFF;
        byte maxByte1 = 0;
        foreach (var encoding in Keys)
        {
            var byte1 = (byte)(encoding >> 8);
            var byte2 = (byte)encoding;
            if (byte1 < minByte1)
            {
                minByte1 = byte1;
            }
            if (byte1 > maxByte1)
            {
                maxByte1 = byte1;
            }
            if (byte2 < minByte2)
            {
                minByte2 = byte2;
            }
            if (byte2 > maxByte2)
            {
                maxByte2 = byte2;
            }
        }

        stream.Seek(tableOffset, SeekOrigin.Begin);
        stream.WriteUInt32(TableFormat);
        stream.WriteUInt16(minByte2, TableFormat.MsByteFirst);
        stream.WriteUInt16(maxByte2, TableFormat.MsByteFirst);
        stream.WriteUInt16(minByte1, TableFormat.MsByteFirst);
        stream.WriteUInt16(maxByte1, TableFormat.MsByteFirst);
        stream.WriteUInt16(DefaultChar, TableFormat.MsByteFirst);

        if (minByte1 == 0 && maxByte1 == 0)
        {
            for (int encoding = minByte2; encoding <= maxByte2; encoding++)
            {
                TryGetValue((ushort)encoding, out var glyphIndex);
                stream.WriteUInt16(glyphIndex, TableFormat.MsByteFirst);
            }
        }
        else
        {
            for (int byte1 = minByte1; byte1 <= maxByte1; byte1++)
            {
                for (int byte2 = minByte2; byte2 <= maxByte2; byte2++)
                {
                    var encoding = (ushort)((byte1 << 8) | byte2);
                    TryGetValue(encoding, out var glyphIndex);
                    stream.WriteUInt16(glyphIndex, TableFormat.MsByteFirst);
                }
            }
        }

        stream.AlignTo4Bytes();

        var tableSize = stream.Position - tableOffset;
        return (uint)tableSize;
    }

    public PcfBdfEncodings Copy() => new(
        _encodings,
        TableFormat,
        DefaultChar);

    public PcfBdfEncodings DeepCopy() => Copy();

    public bool Equals(PcfBdfEncodings? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return TableFormat == other.TableFormat &&
               DefaultChar == other.DefaultChar &&
               EqualUtil.DictionaryEquals(_encodings, other._encodings);
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
        return Equals((PcfBdfEncodings)other);
    }

    public override int GetHashCode() => 0;
}
