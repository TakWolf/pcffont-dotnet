using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using PcfSpec.Errors;
using PcfSpec.Utils;

namespace PcfSpec.Tables;

public partial class PcfProperties : IDictionary<string, PcfPropertyValue>, IList<KeyValuePair<string, PcfPropertyValue>>, IPcfTable, ICopyable<PcfProperties>, IEquatable<PcfProperties>
{
    private const string KeyFoundry = "FOUNDRY";
    private const string KeyFamilyName = "FAMILY_NAME";
    private const string KeyWeightName = "WEIGHT_NAME";
    private const string KeySlant = "SLANT";
    private const string KeySetWidthName = "SETWIDTH_NAME";
    private const string KeyAddStyleName = "ADD_STYLE_NAME";
    private const string KeyPixelSize = "PIXEL_SIZE";
    private const string KeyPointSize = "POINT_SIZE";
    private const string KeyResolutionX = "RESOLUTION_X";
    private const string KeyResolutionY = "RESOLUTION_Y";
    private const string KeySpacing = "SPACING";
    private const string KeyAverageWidth = "AVERAGE_WIDTH";
    private const string KeyCharsetRegistry = "CHARSET_REGISTRY";
    private const string KeyCharsetEncoding = "CHARSET_ENCODING";

    private const string KeyXHeight = "X_HEIGHT";
    private const string KeyCapHeight = "CAP_HEIGHT";
    private const string KeyUnderlinePosition = "UNDERLINE_POSITION";
    private const string KeyUnderlineThickness = "UNDERLINE_THICKNESS";

    private const string KeyFont = "FONT";
    private const string KeyFontVersion = "FONT_VERSION";
    private const string KeyCopyright = "COPYRIGHT";
    private const string KeyNotice = "NOTICE";

    private static readonly HashSet<string> StringValueKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        KeyFoundry,
        KeyFamilyName,
        KeyWeightName,
        KeySlant,
        KeySetWidthName,
        KeyAddStyleName,
        KeySpacing,
        KeyCharsetRegistry,
        KeyCharsetEncoding,
        KeyFont,
        KeyFontVersion,
        KeyCopyright,
        KeyNotice
    };

    private static readonly HashSet<string> IntValueKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        KeyPixelSize,
        KeyPointSize,
        KeyResolutionX,
        KeyResolutionY,
        KeyAverageWidth,
        KeyXHeight,
        KeyCapHeight,
        KeyUnderlinePosition,
        KeyUnderlineThickness
    };

    private static readonly HashSet<string> XlfdStringValueKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        KeyFoundry,
        KeyFamilyName,
        KeyWeightName,
        KeySlant,
        KeySetWidthName,
        KeyAddStyleName,
        KeySpacing,
        KeyCharsetRegistry,
        KeyCharsetEncoding
    };

    private static readonly string[] XlfdKeysOrder = [
        KeyFoundry,
        KeyFamilyName,
        KeyWeightName,
        KeySlant,
        KeySetWidthName,
        KeyAddStyleName,
        KeyPixelSize,
        KeyPointSize,
        KeyResolutionX,
        KeyResolutionY,
        KeySpacing,
        KeyAverageWidth,
        KeyCharsetRegistry,
        KeyCharsetEncoding
    ];

    [GeneratedRegex("^[a-zA-Z0-9_]*$")]
    private static partial Regex RegexPropKey();

    [GeneratedRegex("[-?*,\"]")]
    private static partial Regex RegexXlfdValue();

    private static void CheckKey(string key)
    {
        if (!RegexPropKey().IsMatch(key))
        {
            throw new PcfKeyException("Key contains illegal characters.");
        }
    }

    private static void CheckValue(string key, PcfPropertyValue value)
    {
        if (StringValueKeys.Contains(key) && !value.IsString)
        {
            throw new PcfValueException($"Value of '{key}' must be 'string'.");
        }
        if (IntValueKeys.Contains(key) && !value.IsInt)
        {
            throw new PcfValueException($"Value of '{key}' must be 'int'.");
        }
    }

    private static void CheckXlfdStringValue(string key, string value)
    {
        var match = RegexXlfdValue().Match(value);
        if (match.Success)
        {
            throw new PcfValueException($"Value of '{key}' contains illegal characters '{match.Value}'.");
        }
    }

    public static PcfProperties Parse(Stream stream, PcfHeader header, PcfFont font)
    {
        var tableFormat = header.ReadAndCheckTableFormat(stream);

        var propsCount = stream.ReadUInt32(tableFormat.MsByteFirst);

        var propInfos = new List<(uint, bool, long)>((int)propsCount);
        for (var i = 0; i < propsCount; i++)
        {
            var keyOffset = stream.ReadUInt32(tableFormat.MsByteFirst);
            var isStringProp = stream.ReadBool();
            if (isStringProp)
            {
                var valueOffset = stream.ReadUInt32(tableFormat.MsByteFirst);
                propInfos.Add((keyOffset, isStringProp, valueOffset));
            }
            else
            {
                var value = stream.ReadInt32(tableFormat.MsByteFirst);
                propInfos.Add((keyOffset, isStringProp, value));
            }
        }

        // Pad to next int32 boundary
        var padding = 3 - ((4 + 1 + 4) * propsCount + 3) % 4;
        stream.Seek(padding, SeekOrigin.Current);

        stream.Seek(4, SeekOrigin.Current);  // stringsSize
        var stringsStart = stream.Position;

        var properties = new PcfProperties((int)propsCount, tableFormat);
        foreach (var (keyOffset, isStringProp, valueOrOffset) in propInfos)
        {
            stream.Seek(stringsStart + keyOffset, SeekOrigin.Begin);
            var key = stream.ReadString();
            PcfPropertyValue value;
            if (isStringProp)
            {
                stream.Seek(stringsStart + valueOrOffset, SeekOrigin.Begin);
                value = stream.ReadString();
            }
            else
            {
                value = (int)valueOrOffset;
            }
            properties[key] = value;
        }
        return properties;
    }

    private readonly OrderedDictionary<string, PcfPropertyValue> _properties;

    public PcfTableFormat TableFormat { get; set; }

    public PcfProperties(PcfTableFormat? tableFormat = null) : this(0, tableFormat) { }

    public PcfProperties(
        int capacity,
        PcfTableFormat? tableFormat = null)
    {
        _properties = new OrderedDictionary<string, PcfPropertyValue>(capacity, StringComparer.OrdinalIgnoreCase);
        TableFormat = tableFormat ?? new PcfTableFormat();
    }

    public PcfProperties(
        IDictionary<string, PcfPropertyValue> properties,
        PcfTableFormat? tableFormat = null) : this(properties.Count, tableFormat)
    {
        foreach (var (key, value) in properties)
        {
            this[key] = value;
        }
    }

    public PcfProperties(
        IEnumerable<KeyValuePair<string, PcfPropertyValue>> properties,
        PcfTableFormat? tableFormat = null) :
        this((properties as ICollection<KeyValuePair<string, PcfPropertyValue>>)?.Count ?? 0, tableFormat)
    {
        foreach (var (key, value) in properties)
        {
            this[key] = value;
        }
    }

    public int Count => _properties.Count;

    bool ICollection<KeyValuePair<string, PcfPropertyValue>>.IsReadOnly => false;

    public ICollection<string> Keys => _properties.Keys;

    public ICollection<PcfPropertyValue> Values => _properties.Values;

    public IEnumerator<KeyValuePair<string, PcfPropertyValue>> GetEnumerator() => _properties.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public PcfPropertyValue this[string key]
    {
        get => _properties[key];
        set
        {
            CheckKey(key);
            CheckValue(key, value);
            _properties[key.ToUpperInvariant()] = value;
        }
    }

    KeyValuePair<string, PcfPropertyValue> IList<KeyValuePair<string, PcfPropertyValue>>.this[int index]
    {
        get => (_properties as IList<KeyValuePair<string, PcfPropertyValue>>)[index];
        set
        {
            var key = value.Key;
            CheckKey(key);
            CheckValue(key, value.Value);
            (_properties as IList<KeyValuePair<string, PcfPropertyValue>>)[index] = new KeyValuePair<string, PcfPropertyValue>(key.ToUpperInvariant(), value.Value);
        }
    }

    public bool TryGetValue(string key, out PcfPropertyValue value) => _properties.TryGetValue(key, out value);

    public bool ContainsKey(string key) => _properties.ContainsKey(key);

    public bool ContainsValue(PcfPropertyValue value) => _properties.ContainsValue(value);

    bool ICollection<KeyValuePair<string, PcfPropertyValue>>.Contains(KeyValuePair<string, PcfPropertyValue> item) => (_properties as ICollection<KeyValuePair<string, PcfPropertyValue>>).Contains(new KeyValuePair<string, PcfPropertyValue>(item.Key, item.Value));

    int IList<KeyValuePair<string, PcfPropertyValue>>.IndexOf(KeyValuePair<string, PcfPropertyValue> item) => (_properties as IList<KeyValuePair<string, PcfPropertyValue>>).IndexOf(new KeyValuePair<string, PcfPropertyValue>(item.Key, item.Value));

    public void Add(string key, PcfPropertyValue value)
    {
        CheckKey(key);
        CheckValue(key, value);
        _properties.Add(key.ToUpperInvariant(), value);
    }

    void ICollection<KeyValuePair<string, PcfPropertyValue>>.Add(KeyValuePair<string, PcfPropertyValue> item)
    {
        var key = item.Key;
        CheckKey(key);
        CheckValue(key, item.Value);
        (_properties as ICollection<KeyValuePair<string, PcfPropertyValue>>).Add(new KeyValuePair<string, PcfPropertyValue>(key.ToUpperInvariant(), item.Value));
    }

    void IList<KeyValuePair<string, PcfPropertyValue>>.Insert(int index, KeyValuePair<string, PcfPropertyValue> item)
    {
        var key = item.Key;
        CheckKey(key);
        CheckValue(key, item.Value);
        (_properties as IList<KeyValuePair<string, PcfPropertyValue>>).Insert(index, new KeyValuePair<string, PcfPropertyValue>(key.ToUpperInvariant(), item.Value));
    }

    public bool Remove(string key) => _properties.Remove(key);

    bool ICollection<KeyValuePair<string, PcfPropertyValue>>.Remove(KeyValuePair<string, PcfPropertyValue> item) => (_properties as ICollection<KeyValuePair<string, PcfPropertyValue>>).Remove(new KeyValuePair<string, PcfPropertyValue>(item.Key, item.Value));

    void IList<KeyValuePair<string, PcfPropertyValue>>.RemoveAt(int index) => (_properties as IList<KeyValuePair<string, PcfPropertyValue>>).RemoveAt(index);

    public void Clear() => _properties.Clear();

    void ICollection<KeyValuePair<string, PcfPropertyValue>>.CopyTo(KeyValuePair<string, PcfPropertyValue>[] array, int arrayIndex) => (_properties as ICollection<KeyValuePair<string, PcfPropertyValue>>).CopyTo(array, arrayIndex);

    public PcfPropertyValue? GetValue(string key) => TryGetValue(key, out var value) ? value : (PcfPropertyValue?)null;

    public string? GetStringValue(string key) => GetValue(key)?.AsString();

    public int? GetIntValue(string key) => GetValue(key)?.AsInt();

    public void SetValue(string key, PcfPropertyValue? value)
    {
        if (value is null)
        {
            Remove(key);
        }
        else
        {
            this[key] = value.Value;
        }
    }

    public void SetStringValue(string key, string? value)
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

    public void SetIntValue(string key, int? value)
    {
        if (value is null)
        {
            Remove(key);
        }
        else
        {
            this[key] = value.Value;
        }
    }

    public string? Foundry
    {
        get => GetStringValue(KeyFoundry);
        set => SetStringValue(KeyFoundry, value);
    }

    public string? FamilyName
    {
        get => GetStringValue(KeyFamilyName);
        set => SetStringValue(KeyFamilyName, value);
    }

    public string? WeightName
    {
        get => GetStringValue(KeyWeightName);
        set => SetStringValue(KeyWeightName, value);
    }

    public string? Slant
    {
        get => GetStringValue(KeySlant);
        set => SetStringValue(KeySlant, value);
    }

    public string? SetWidthName
    {
        get => GetStringValue(KeySetWidthName);
        set => SetStringValue(KeySetWidthName, value);
    }

    public string? AddStyleName
    {
        get => GetStringValue(KeyAddStyleName);
        set => SetStringValue(KeyAddStyleName, value);
    }

    public int? PixelSize
    {
        get => GetIntValue(KeyPixelSize);
        set => SetIntValue(KeyPixelSize, value);
    }

    public int? PointSize
    {
        get => GetIntValue(KeyPointSize);
        set => SetIntValue(KeyPointSize, value);
    }

    public int? ResolutionX
    {
        get => GetIntValue(KeyResolutionX);
        set => SetIntValue(KeyResolutionX, value);
    }

    public int? ResolutionY
    {
        get => GetIntValue(KeyResolutionY);
        set => SetIntValue(KeyResolutionY, value);
    }

    public string? Spacing
    {
        get => GetStringValue(KeySpacing);
        set => SetStringValue(KeySpacing, value);
    }

    public int? AverageWidth
    {
        get => GetIntValue(KeyAverageWidth);
        set => SetIntValue(KeyAverageWidth, value);
    }

    public string? CharsetRegistry
    {
        get => GetStringValue(KeyCharsetRegistry);
        set => SetStringValue(KeyCharsetRegistry, value);
    }

    public string? CharsetEncoding
    {
        get => GetStringValue(KeyCharsetEncoding);
        set => SetStringValue(KeyCharsetEncoding, value);
    }

    public int? XHeight
    {
        get => GetIntValue(KeyXHeight);
        set => SetIntValue(KeyXHeight, value);
    }

    public int? CapHeight
    {
        get => GetIntValue(KeyCapHeight);
        set => SetIntValue(KeyCapHeight, value);
    }

    public int? UnderlinePosition
    {
        get => GetIntValue(KeyUnderlinePosition);
        set => SetIntValue(KeyUnderlinePosition, value);
    }

    public int? UnderlineThickness
    {
        get => GetIntValue(KeyUnderlineThickness);
        set => SetIntValue(KeyUnderlineThickness, value);
    }

    public string? Font
    {
        get => GetStringValue(KeyFont);
        set => SetStringValue(KeyFont, value);
    }

    public string? FontVersion
    {
        get => GetStringValue(KeyFontVersion);
        set => SetStringValue(KeyFontVersion, value);
    }

    public string? Copyright
    {
        get => GetStringValue(KeyCopyright);
        set => SetStringValue(KeyCopyright, value);
    }

    public string? Notice
    {
        get => GetStringValue(KeyNotice);
        set => SetStringValue(KeyNotice, value);
    }

    public void GenerateXlfd()
    {
        var xlfd = new StringBuilder();
        foreach (var key in XlfdKeysOrder)
        {
            var value = GetValue(key)?.ToString() ?? "";
            if (XlfdStringValueKeys.Contains(key))
            {
                CheckXlfdStringValue(key, value);
            }
            xlfd.Append('-');
            xlfd.Append(value);
        }
        Font = xlfd.ToString();
    }

    public void UpdateByXlfd()
    {
        var font = Font;
        if (font is null)
        {
            throw new PcfXlfdException($"'{KeyFont}' not set.");
        }
        if (!font.StartsWith('-'))
        {
            throw new PcfXlfdException("Not starts with '-'.");
        }
        var parts = font[1..].Split('-');
        if (parts.Length != 14)
        {
            throw new PcfXlfdException("Must be 14 '-'.");
        }
        for (var i = 0; i < XlfdKeysOrder.Length; i++)
        {
            var key = XlfdKeysOrder[i];
            var part = parts[i];
            PcfPropertyValue? value;
            if (part is "")
            {
                value = null;
            }
            else
            {
                if (XlfdStringValueKeys.Contains(key))
                {
                    CheckXlfdStringValue(key, part);
                    value = part;
                }
                else
                {
                    value = int.Parse(part);
                }
            }
            SetValue(key, value);
        }
    }

    public uint Dump(Stream stream, uint tableOffset, PcfFont font)
    {
        var propsCount = (uint)Count;

        // Pad to next int32 boundary
        var padding = 3 - ((4 + 1 + 4) * propsCount + 3) % 4;

        var stringsStart = tableOffset + 4 + 4 + (4 + 1 + 4) * propsCount + padding + 4;
        var stringsSize = 0u;
        var propInfos = new List<(uint, PcfPropertyValue, uint)>((int)propsCount);
        stream.Seek(stringsStart, SeekOrigin.Begin);
        foreach (var (key, value) in this)
        {
            var keyOffset = stringsSize;
            stringsSize += (uint)stream.WriteString(key);
            var valueOffset = stringsSize;
            if (value.IsString)
            {
                stringsSize += (uint)stream.WriteString(value.AsString());
            }
            propInfos.Add((keyOffset, value, valueOffset));
        }

        stream.Seek(tableOffset, SeekOrigin.Begin);
        stream.WriteUInt32(TableFormat.Value);
        stream.WriteUInt32(propsCount, TableFormat.MsByteFirst);
        foreach (var (keyOffset, value, valueOffset) in propInfos)
        {
            stream.WriteUInt32(keyOffset, TableFormat.MsByteFirst);
            if (value.IsString)
            {
                stream.WriteBool(true);
                stream.WriteUInt32(valueOffset, TableFormat.MsByteFirst);
            }
            else
            {
                stream.WriteBool(false);
                stream.WriteInt32((int)value, TableFormat.MsByteFirst);
            }
        }
        stream.WriteNulls((int)padding);
        stream.WriteUInt32(stringsSize, TableFormat.MsByteFirst);
        stream.Seek(stringsSize, SeekOrigin.Current);
        stream.AlignTo4Bytes();

        var tableSize = stream.Position - tableOffset;
        return (uint)tableSize;
    }

    public PcfProperties Copy() => new(_properties, TableFormat);

    public PcfProperties DeepCopy() => new(_properties, TableFormat.DeepCopy());

    public bool Equals(PcfProperties? other)
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
               EqualUtil.DictionaryEquals(_properties, other._properties);
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
        return Equals((PcfProperties)other);
    }

    public override int GetHashCode() => 0;
}
