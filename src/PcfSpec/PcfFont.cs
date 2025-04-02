using System.Collections;
using System.Diagnostics.CodeAnalysis;
using PcfSpec.Table;

namespace PcfSpec;

public class PcfFont : IDictionary<PcfTableType, IPcfTable>
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

    private static void CheckTableType(PcfTableType tableType, IPcfTable table)
    {
        if (FactoryRegistry[tableType].ClassType != table.GetType())
        {
            throw new ArgumentException($"Table type mismatch: '{FactoryRegistry[tableType].ClassType}' -> '{table.GetType()}'");
        }
    }

    public static PcfFont Parse(Stream stream)
    {
        var font = new PcfFont();
        var headers = PcfHeader.Parse(stream);
        foreach (var header in headers)
        {
            var table = FactoryRegistry[header.TableType].Parse(stream, font, header);
            font[header.TableType] = table;
        }
        return font;
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

    private readonly SortedDictionary<PcfTableType, IPcfTable> _dictionary = new();

    public int Count => _dictionary.Count;

    bool ICollection<KeyValuePair<PcfTableType, IPcfTable>>.IsReadOnly => false;

    public ICollection<PcfTableType> Keys => _dictionary.Keys;

    public ICollection<IPcfTable> Values => _dictionary.Values;

    public IEnumerator<KeyValuePair<PcfTableType, IPcfTable>> GetEnumerator() => _dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IPcfTable this[PcfTableType key]
    {
        get => _dictionary[key];
        set
        {
            CheckTableType(key, value);
            _dictionary[key] = value;
        }
    }

    public bool TryGetValue(PcfTableType key, [MaybeNullWhen(false)] out IPcfTable value) => _dictionary.TryGetValue(key, out value);

    public bool ContainsKey(PcfTableType key) => _dictionary.ContainsKey(key);

    public bool ContainsValue(IPcfTable value) => _dictionary.ContainsValue(value);

    bool ICollection<KeyValuePair<PcfTableType, IPcfTable>>.Contains(KeyValuePair<PcfTableType, IPcfTable> item) => (_dictionary as ICollection<KeyValuePair<PcfTableType, IPcfTable>>).Contains(item);

    public void Add(PcfTableType key, IPcfTable value)
    {
        CheckTableType(key, value);
        _dictionary.Add(key, value);
    }

    void ICollection<KeyValuePair<PcfTableType, IPcfTable>>.Add(KeyValuePair<PcfTableType, IPcfTable> item)
    {
        CheckTableType(item.Key, item.Value);
        (_dictionary as ICollection<KeyValuePair<PcfTableType, IPcfTable>>).Add(item);
    }

    public bool Remove(PcfTableType key) => _dictionary.Remove(key);

    bool ICollection<KeyValuePair<PcfTableType, IPcfTable>>.Remove(KeyValuePair<PcfTableType, IPcfTable> item) => (_dictionary as ICollection<KeyValuePair<PcfTableType, IPcfTable>>).Remove(item);

    public void Clear() => _dictionary.Clear();

    void ICollection<KeyValuePair<PcfTableType, IPcfTable>>.CopyTo(KeyValuePair<PcfTableType, IPcfTable>[] array, int arrayIndex) => (_dictionary as ICollection<KeyValuePair<PcfTableType, IPcfTable>>).CopyTo(array, arrayIndex);

    public T? GetTable<T>(PcfTableType key) where T : IPcfTable
    {
        TryGetValue(key, out var value);
        return (T?)value;
    }

    public void SetTable(PcfTableType key, IPcfTable? value)
    {
        if (value is null)
        {
            Remove(key);
        }
        else
        {
            this[key] = value;
        }
    }

    public PcfProperties? Properties
    {
        get => GetTable<PcfProperties>(PcfTableType.Properties);
        set => SetTable(PcfTableType.Properties, value);
    }

    public PcfAccelerators? Accelerators
    {
        get => GetTable<PcfAccelerators>(PcfTableType.Accelerators);
        set => SetTable(PcfTableType.Accelerators, value);
    }

    public PcfMetrics? Metrics
    {
        get => GetTable<PcfMetrics>(PcfTableType.Metrics);
        set => SetTable(PcfTableType.Metrics, value);
    }

    public PcfBitmaps? Bitmaps
    {
        get => GetTable<PcfBitmaps>(PcfTableType.Bitmaps);
        set => SetTable(PcfTableType.Bitmaps, value);
    }

    public PcfMetrics? InkMetrics
    {
        get => GetTable<PcfMetrics>(PcfTableType.InkMetrics);
        set => SetTable(PcfTableType.InkMetrics, value);
    }

    public PcfBdfEncodings? BdfEncodings
    {
        get => GetTable<PcfBdfEncodings>(PcfTableType.BdfEncodings);
        set => SetTable(PcfTableType.BdfEncodings, value);
    }

    public PcfScalableWidths? ScalableWidths
    {
        get => GetTable<PcfScalableWidths>(PcfTableType.ScalableWidths);
        set => SetTable(PcfTableType.ScalableWidths, value);
    }

    public PcfGlyphNames? GlyphNames
    {
        get => GetTable<PcfGlyphNames>(PcfTableType.GlyphNames);
        set => SetTable(PcfTableType.GlyphNames, value);
    }

    public PcfAccelerators? BdfAccelerators
    {
        get => GetTable<PcfAccelerators>(PcfTableType.BdfAccelerators);
        set => SetTable(PcfTableType.BdfAccelerators, value);
    }

    public void Dump(Stream stream)
    {
        var headers = new List<PcfHeader>();
        var tableOffset = (uint)(4 + 4 + 4 * 4 * Count);
        foreach (var (tableType, table) in this)
        {
            var tableSize = table.Dump(stream, this, tableOffset);
            headers.Add(new PcfHeader(tableType, table.TableFormat, tableSize, tableOffset));
            tableOffset += tableSize;
        }
        PcfHeader.Dump(stream, headers);
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
