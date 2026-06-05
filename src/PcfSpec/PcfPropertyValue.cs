namespace PcfSpec;

public readonly struct PcfPropertyValue : IEquatable<PcfPropertyValue>
{
    private readonly int _intValue;
    private readonly string? _stringValue;

    public PcfPropertyValue(int value)
    {
        _intValue = value;
        _stringValue = null;
    }

    public PcfPropertyValue(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _intValue = 0;
        _stringValue = value;
    }

    public bool IsInt => _stringValue is null;

    public bool IsString => _stringValue is not null;

    public int AsInt() => IsInt ? _intValue : throw new InvalidOperationException("The value is not int.");

    public string AsString() => IsString ? _stringValue! : throw new InvalidOperationException("The value is not string.");

    public static implicit operator PcfPropertyValue(int value) => new(value);

    public static implicit operator PcfPropertyValue(string value) => new(value);

    public static explicit operator int(PcfPropertyValue value) => value.AsInt();

    public static explicit operator string(PcfPropertyValue value) => value.AsString();

    public void Deconstruct(out int? intValue, out string? stringValue)
    {
        if (IsInt)
        {
            intValue = _intValue;
            stringValue = null;
        }
        else
        {
            intValue = null;
            stringValue = _stringValue;
        }
    }

    public bool Equals(PcfPropertyValue other) => _intValue == other._intValue && _stringValue == other._stringValue;

    public override bool Equals(object? obj) => obj is PcfPropertyValue other && Equals(other);

    public static bool operator ==(PcfPropertyValue left, PcfPropertyValue right) => left.Equals(right);

    public static bool operator !=(PcfPropertyValue left, PcfPropertyValue right) => !(left == right);

    public override int GetHashCode() => IsInt ? HashCode.Combine(1, _intValue) : HashCode.Combine(2, _stringValue);

    public override string ToString() => IsInt ? _intValue.ToString() : _stringValue!;
}
