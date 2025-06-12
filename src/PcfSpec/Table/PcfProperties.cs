using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using PcfSpec.Error;
using PcfSpec.Util;

namespace PcfSpec.Table;

public partial class PcfProperties : IDictionary<string, object>, IList<KeyValuePair<string, object>>, IPcfTable
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

    private static readonly string[] StringValueKeys = [
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
    ];

    private static readonly string[] IntValueKeys = [
        KeyPixelSize,
        KeyPointSize,
        KeyResolutionX,
        KeyResolutionY,
        KeyAverageWidth,
        KeyXHeight,
        KeyCapHeight,
        KeyUnderlinePosition,
        KeyUnderlineThickness
    ];

    private static readonly string[] XlfdStringValueKeys = [
        KeyFoundry,
        KeyFamilyName,
        KeyWeightName,
        KeySlant,
        KeySetWidthName,
        KeyAddStyleName,
        KeySpacing,
        KeyCharsetRegistry,
        KeyCharsetEncoding
    ];

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
            throw new PcfKeyException("Contains illegal characters.");
        }
    }

    private static void CheckValue(string key, object value)
    {
        if (StringValueKeys.Contains(key))
        {
            if (value is not string)
            {
                throw new PcfValueException($"Expected type 'string', got '{value.GetType()}' instead.");
            }
        }
        else if (IntValueKeys.Contains(key))
        {
            if (value is not int)
            {
                throw new PcfValueException($"Expected type 'int', got '{value.GetType()}' instead.");
            }
        }
        else
        {
            if (value is not string && value is not int)
            {
                throw new PcfValueException($"Expected type 'string' or 'int', got '{value.GetType()}' instead.");
            }
        }

        if (XlfdStringValueKeys.Contains(key))
        {
            if (RegexXlfdValue().IsMatch((string)value))
            {
                throw new PcfValueException("Contains illegal characters.");
            }
        }
    }

    public static PcfProperties Parse(Stream stream, PcfHeader header, PcfFont font)
    {
        var tableFormat = header.ReadAndCheckTableFormat(stream);

        var propsCount = stream.ReadUInt32(tableFormat.MsByteFirst);

        var propInfos = new List<(uint, bool, long)>();
        foreach (var _ in Enumerable.Range(0, (int)propsCount))
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

        var properties = new Dictionary<string, object>();
        foreach (var (keyOffset, isStringProp, valueOrOffset) in propInfos)
        {
            stream.Seek(stringsStart + keyOffset, SeekOrigin.Begin);
            var key = stream.ReadString();
            object value;
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

        return new PcfProperties(tableFormat, properties);
    }

    private readonly OrderedDictionary<string, object> _dictionary = new();

    public PcfTableFormat TableFormat { get; set; }

    public PcfProperties(
        PcfTableFormat? tableFormat = null,
        IDictionary<string, object>? properties = null)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
        if (properties is not null)
        {
            foreach (var (key, value) in properties)
            {
                this[key] = value;
            }
        }
    }

    public int Count => _dictionary.Count;

    bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;

    public ICollection<string> Keys => _dictionary.Keys;

    public ICollection<object> Values => _dictionary.Values;

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public object this[string key]
    {
        get => _dictionary[key.ToUpper()];
        set
        {
            key = key.ToUpper();
            CheckKey(key);
            CheckValue(key, value);
            _dictionary[key] = value;
        }
    }

    KeyValuePair<string, object> IList<KeyValuePair<string, object>>.this[int index]
    {
        get => (_dictionary as IList<KeyValuePair<string, object>>)[index];
        set
        {
            var key = value.Key.ToUpper();
            CheckKey(key);
            CheckValue(key, value.Value);
            (_dictionary as IList<KeyValuePair<string, object>>)[index] = new KeyValuePair<string, object>(key, value.Value);
        }
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value) => _dictionary.TryGetValue(key.ToUpper(), out value);

    public bool ContainsKey(string key) => _dictionary.ContainsKey(key.ToUpper());

    public bool ContainsValue(object value) => _dictionary.ContainsValue(value);

    bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item) => (_dictionary as ICollection<KeyValuePair<string, object>>).Contains(new KeyValuePair<string, object>(item.Key.ToUpper(), item.Value));

    int IList<KeyValuePair<string, object>>.IndexOf(KeyValuePair<string, object> item) => (_dictionary as IList<KeyValuePair<string, object>>).IndexOf(new KeyValuePair<string, object>(item.Key.ToUpper(), item.Value));

    public void Add(string key, object value)
    {
        key = key.ToUpper();
        CheckKey(key);
        CheckValue(key, value);
        _dictionary.Add(key, value);
    }

    void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
    {
        var key = item.Key.ToUpper();
        CheckKey(key);
        CheckValue(key, item.Value);
        (_dictionary as ICollection<KeyValuePair<string, object>>).Add(new KeyValuePair<string, object>(key, item.Value));
    }

    void IList<KeyValuePair<string, object>>.Insert(int index, KeyValuePair<string, object> item)
    {
        var key = item.Key.ToUpper();
        CheckKey(key);
        CheckValue(key, item.Value);
        (_dictionary as IList<KeyValuePair<string, object>>).Insert(index, new KeyValuePair<string, object>(key, item.Value));
    }

    public bool Remove(string key) => _dictionary.Remove(key.ToUpper());

    bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item) => (_dictionary as ICollection<KeyValuePair<string, object>>).Remove(new KeyValuePair<string, object>(item.Key.ToUpper(), item.Value));

    void IList<KeyValuePair<string, object>>.RemoveAt(int index) => (_dictionary as IList<KeyValuePair<string, object>>).RemoveAt(index);

    public void Clear() => _dictionary.Clear();

    void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => (_dictionary as ICollection<KeyValuePair<string, object>>).CopyTo(array, arrayIndex);

    public object? GetValue(string key) => TryGetValue(key, out var value) ? value : null;

    public string? GetStringValue(string key) => (string?)GetValue(key);

    public int? GetIntValue(string key) => (int?)GetValue(key);

    public void SetValue(string key, object? value)
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

    public string? Foundry
    {
        get => GetStringValue(KeyFoundry);
        set => SetValue(KeyFoundry, value);
    }

    public string? FamilyName
    {
        get => GetStringValue(KeyFamilyName);
        set => SetValue(KeyFamilyName, value);
    }

    public string? WeightName
    {
        get => GetStringValue(KeyWeightName);
        set => SetValue(KeyWeightName, value);
    }

    public string? Slant
    {
        get => GetStringValue(KeySlant);
        set => SetValue(KeySlant, value);
    }

    public string? SetWidthName
    {
        get => GetStringValue(KeySetWidthName);
        set => SetValue(KeySetWidthName, value);
    }

    public string? AddStyleName
    {
        get => GetStringValue(KeyAddStyleName);
        set => SetValue(KeyAddStyleName, value);
    }

    public int? PixelSize
    {
        get => GetIntValue(KeyPixelSize);
        set => SetValue(KeyPixelSize, value);
    }

    public int? PointSize
    {
        get => GetIntValue(KeyPointSize);
        set => SetValue(KeyPointSize, value);
    }

    public int? ResolutionX
    {
        get => GetIntValue(KeyResolutionX);
        set => SetValue(KeyResolutionX, value);
    }

    public int? ResolutionY
    {
        get => GetIntValue(KeyResolutionY);
        set => SetValue(KeyResolutionY, value);
    }

    public string? Spacing
    {
        get => GetStringValue(KeySpacing);
        set => SetValue(KeySpacing, value);
    }

    public int? AverageWidth
    {
        get => GetIntValue(KeyAverageWidth);
        set => SetValue(KeyAverageWidth, value);
    }

    public string? CharsetRegistry
    {
        get => GetStringValue(KeyCharsetRegistry);
        set => SetValue(KeyCharsetRegistry, value);
    }

    public string? CharsetEncoding
    {
        get => GetStringValue(KeyCharsetEncoding);
        set => SetValue(KeyCharsetEncoding, value);
    }

    public int? XHeight
    {
        get => GetIntValue(KeyXHeight);
        set => SetValue(KeyXHeight, value);
    }

    public int? CapHeight
    {
        get => GetIntValue(KeyCapHeight);
        set => SetValue(KeyCapHeight, value);
    }

    public int? UnderlinePosition
    {
        get => GetIntValue(KeyUnderlinePosition);
        set => SetValue(KeyUnderlinePosition, value);
    }

    public int? UnderlineThickness
    {
        get => GetIntValue(KeyUnderlineThickness);
        set => SetValue(KeyUnderlineThickness, value);
    }

    public string? Font
    {
        get => GetStringValue(KeyFont);
        set => SetValue(KeyFont, value);
    }

    public string? FontVersion
    {
        get => GetStringValue(KeyFontVersion);
        set => SetValue(KeyFontVersion, value);
    }

    public string? Copyright
    {
        get => GetStringValue(KeyCopyright);
        set => SetValue(KeyCopyright, value);
    }

    public string? Notice
    {
        get => GetStringValue(KeyNotice);
        set => SetValue(KeyNotice, value);
    }

    public void GenerateXlfd()
    {
        List<string> tokens = [""];
        tokens.AddRange(XlfdKeysOrder.Select(key => GetValue(key)?.ToString() ?? ""));
        Font = string.Join("-", tokens);
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
        var tokens = font[1..font.Length].Split('-');
        if (tokens.Length != 14)
        {
            throw new PcfXlfdException("Must be 14 '-'.");
        }
        foreach (var (key, token) in XlfdKeysOrder.Zip(tokens))
        {
            object? value;
            if ("".Equals(token))
            {
                value = null;
            }
            else
            {
                if (XlfdStringValueKeys.Contains(key))
                {
                    value = token;
                }
                else
                {
                    value = Convert.ToInt32(token);
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
        var propInfos = new List<(uint, object, uint)>();
        stream.Seek(stringsStart, SeekOrigin.Begin);
        foreach (var (key, value) in this)
        {
            var keyOffset = stringsSize;
            stringsSize += (uint)stream.WriteString(key);
            var valueOffset = stringsSize;
            if (value is string stringValue)
            {
                stringsSize += (uint)stream.WriteString(stringValue);
            }
            propInfos.Add((keyOffset, value, valueOffset));
        }

        stream.Seek(tableOffset, SeekOrigin.Begin);
        stream.WriteUInt32(TableFormat.Value);
        stream.WriteUInt32(propsCount, TableFormat.MsByteFirst);
        foreach (var (keyOffset, value, valueOffset) in propInfos)
        {
            stream.WriteUInt32(keyOffset, TableFormat.MsByteFirst);
            if (value is string)
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
        stream.WriteNulls(padding);
        stream.WriteUInt32(stringsSize, TableFormat.MsByteFirst);
        stream.Seek(stringsSize, SeekOrigin.Current);
        stream.AlignTo4ByteWithNulls();

        var tableSize = stream.Position - tableOffset;
        return (uint)tableSize;
    }

    public static bool Equals(PcfProperties? objA, PcfProperties? objB)
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
