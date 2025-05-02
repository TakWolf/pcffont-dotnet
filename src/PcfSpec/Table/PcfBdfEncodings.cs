using System.Buffers.Binary;
using System.Collections;
using PcfSpec.Util;

namespace PcfSpec.Table;

public class PcfBdfEncodings : IDictionary<ushort, ushort>, IPcfTable
{
    public const ushort NoGlyphIndex = 0xFFFF;

    public static PcfBdfEncodings Parse(Stream stream, PcfHeader header, PcfFont font)
    {
        var tableFormat = header.ReadAndCheckTableFormat(stream);

        var minByte2 = stream.ReadUInt16(tableFormat.MsByteFirst);
        var maxByte2 = stream.ReadUInt16(tableFormat.MsByteFirst);
        var minByte1 = stream.ReadUInt16(tableFormat.MsByteFirst);
        var maxByte1 = stream.ReadUInt16(tableFormat.MsByteFirst);
        var defaultChar = stream.ReadUInt16(tableFormat.MsByteFirst);

        var glyphsCount = (maxByte2 - minByte2 + 1) * (maxByte1 - minByte1 + 1);
        var glyphIndices = Enumerable.Range(0, glyphsCount).Select(_ => stream.ReadUInt16(tableFormat.MsByteFirst)).ToList();

        var encodings = new Dictionary<ushort, ushort>();
        if (minByte1 == 0 && maxByte1 == 0)
        {
            foreach (ushort encoding in Enumerable.Range(minByte2, maxByte2 + 1 - minByte2))
            {
                var glyphIndex = glyphIndices[encoding - minByte2];
                encodings[encoding] = glyphIndex;
            }
        }
        else
        {
            foreach (byte byte1 in Enumerable.Range(minByte1, maxByte1 + 1 - minByte1))
            {
                foreach (byte byte2 in Enumerable.Range(minByte2, maxByte2 + 1 - minByte2))
                {
                    var encoding = BinaryPrimitives.ReadUInt16BigEndian([byte1, byte2]);
                    var glyphIndex = glyphIndices[(byte1 - minByte1) * (maxByte2 - minByte2 + 1) + byte2 - minByte2];
                    encodings[encoding] = glyphIndex;
                }
            }
        }

        return new PcfBdfEncodings(tableFormat, defaultChar, encodings);
    }

    private readonly SortedDictionary<ushort, ushort> _dictionary = new();

    public PcfTableFormat TableFormat { get; set; }
    public ushort DefaultChar;

    public PcfBdfEncodings(
        PcfTableFormat? tableFormat = null,
        ushort defaultChar = NoGlyphIndex,
        IDictionary<ushort, ushort>? encodings = null)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
        DefaultChar = defaultChar;
        if (encodings is not null)
        {
            foreach (var (encoding, glyphIndex) in encodings)
            {
                this[encoding] = glyphIndex;
            }
        }
    }

    public int Count => _dictionary.Count;

    bool ICollection<KeyValuePair<ushort, ushort>>.IsReadOnly => false;

    public ICollection<ushort> Keys => _dictionary.Keys;

    public ICollection<ushort> Values => _dictionary.Values;

    public IEnumerator<KeyValuePair<ushort, ushort>> GetEnumerator() => _dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public ushort this[ushort key]
    {
        get => _dictionary[key];
        set
        {
            if (value == NoGlyphIndex)
            {
                _dictionary.Remove(key);
                return;
            }
            _dictionary[key] = value;
        }
    }

    public bool TryGetValue(ushort key, out ushort value)
    {
        var found = _dictionary.TryGetValue(key, out value);
        if (!found)
        {
            value = NoGlyphIndex;
        }
        return found;
    }

    public bool ContainsKey(ushort key) => _dictionary.ContainsKey(key);

    public bool ContainsValue(ushort value) => _dictionary.ContainsValue(value);

    bool ICollection<KeyValuePair<ushort, ushort>>.Contains(KeyValuePair<ushort, ushort> item) => (_dictionary as ICollection<KeyValuePair<ushort, ushort>>).Contains(item);

    public void Add(ushort key, ushort value)
    {
        if (value == NoGlyphIndex && !_dictionary.ContainsKey(key))
        {
            return;
        }
        _dictionary.Add(key, value);
    }

    void ICollection<KeyValuePair<ushort, ushort>>.Add(KeyValuePair<ushort, ushort> item)
    {
        if (item.Value == NoGlyphIndex && !_dictionary.ContainsKey(item.Key))
        {
            return;
        }
        (_dictionary as ICollection<KeyValuePair<ushort, ushort>>).Add(item);
    }

    public bool Remove(ushort key) => _dictionary.Remove(key);

    bool ICollection<KeyValuePair<ushort, ushort>>.Remove(KeyValuePair<ushort, ushort> item) => (_dictionary as ICollection<KeyValuePair<ushort, ushort>>).Remove(item);

    public void Clear() => _dictionary.Clear();

    void ICollection<KeyValuePair<ushort, ushort>>.CopyTo(KeyValuePair<ushort, ushort>[] array, int arrayIndex) => (_dictionary as ICollection<KeyValuePair<ushort, ushort>>).CopyTo(array, arrayIndex);

    public uint Dump(Stream stream, uint tableOffset, PcfFont font)
    {
        byte minByte2 = 0xFF;
        byte maxByte2 = 0;
        byte minByte1 = 0xFF;
        byte maxByte1 = 0;
        foreach (var encoding in Keys)
        {
            var bs = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(bs, encoding);
            var byte1 = bs[0];
            var byte2 = bs[1];
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
        stream.WriteUInt32(TableFormat.Value);
        stream.WriteUInt16(minByte2, TableFormat.MsByteFirst);
        stream.WriteUInt16(maxByte2, TableFormat.MsByteFirst);
        stream.WriteUInt16(minByte1, TableFormat.MsByteFirst);
        stream.WriteUInt16(maxByte1, TableFormat.MsByteFirst);
        stream.WriteUInt16(DefaultChar, TableFormat.MsByteFirst);

        if (minByte1 == 0 && maxByte1 == 0)
        {
            foreach (ushort encoding in Enumerable.Range(minByte2, maxByte2 + 1 - minByte2))
            {
                TryGetValue(encoding, out var glyphIndex);
                stream.WriteUInt16(glyphIndex, TableFormat.MsByteFirst);
            }
        }
        else
        {
            foreach (byte byte1 in Enumerable.Range(minByte1, maxByte1 + 1 - minByte1))
            {
                foreach (byte byte2 in Enumerable.Range(minByte2, maxByte2 + 1 - minByte2))
                {
                    var encoding = BinaryPrimitives.ReadUInt16BigEndian([byte1, byte2]);
                    TryGetValue(encoding, out var glyphIndex);
                    stream.WriteUInt16(glyphIndex, TableFormat.MsByteFirst);
                }
            }
        }

        stream.AlignTo4ByteWithNulls();

        var tableSize = stream.Position - tableOffset;
        return (uint)tableSize;
    }

    public static bool Equals(PcfBdfEncodings? objA, PcfBdfEncodings? objB)
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
               objA.DefaultChar == objB.DefaultChar &&
               objA.SequenceEqual(objB);
    }
}
