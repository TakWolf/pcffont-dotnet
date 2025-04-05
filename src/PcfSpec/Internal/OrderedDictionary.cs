using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace PcfSpec.Internal;

internal class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IList<KeyValuePair<TKey, TValue>>
{
    private readonly List<TKey> _keysData = [];
    private readonly List<TValue> _valuesData = [];
    private KeyCollection? _keys;
    private ValueCollection? _values;

    public int Count => _keysData.Count;

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

    public KeyCollection Keys => _keys ??= new KeyCollection(_keysData);

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

    public ValueCollection Values => _values ??= new ValueCollection(_valuesData);

    ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var (key, value) in _keysData.Zip(_valuesData))
        {
            yield return new KeyValuePair<TKey, TValue>(key, value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public TValue this[TKey key]
    {
        get
        {
            var index = _keysData.IndexOf(key);
            if (index >= 0)
            {
                return _valuesData[index];
            }
            throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary.");
        }
        set
        {
            var index = _keysData.IndexOf(key);
            if (index >= 0)
            {
                _valuesData[index] = value;
            }
            else
            {
                _keysData.Add(key);
                _valuesData.Add(value);
            }
        }
    }

    KeyValuePair<TKey, TValue> IList<KeyValuePair<TKey, TValue>>.this[int index]
    {
        get => new(_keysData[index], _valuesData[index]);
        set
        {
            var existsIndex = _keysData.IndexOf(value.Key);
            if (existsIndex >= 0 && existsIndex != index)
            {
                throw new ArgumentException($"An element with the same key '{value.Key}' already exists in the dictionary.");
            }
            _keysData[index] = value.Key;
            _valuesData[index] = value.Value;
        }
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        var index = _keysData.IndexOf(key);
        if (index >= 0)
        {
            value = _valuesData[index];
            return true;
        }
        value = default;
        return false;
    }

    public bool ContainsKey(TKey key) => _keysData.Contains(key);

    public bool ContainsValue(TValue value) => _valuesData.Contains(value);

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        var index = _keysData.IndexOf(item.Key);
        if (index >= 0)
        {
            return Equals(_valuesData[index], item.Value);
        }
        return false;
    }

    int IList<KeyValuePair<TKey, TValue>>.IndexOf(KeyValuePair<TKey, TValue> item)
    {
        var index = _keysData.IndexOf(item.Key);
        if (index >= 0)
        {
            return Equals(_valuesData[index], item.Value) ? index : -1;
        }
        return -1;
    }

    public void Add(TKey key, TValue value)
    {
        if (_keysData.Contains(key))
        {
            throw new ArgumentException($"An element with the same key '{key}' already exists in the dictionary.");
        }
        _keysData.Add(key);
        _valuesData.Add(value);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

    void IList<KeyValuePair<TKey, TValue>>.Insert(int index, KeyValuePair<TKey, TValue> item)
    {
        if (_keysData.Contains(item.Key))
        {
            throw new ArgumentException($"An element with the same key '{item.Key}' already exists in the dictionary.");
        }
        _keysData.Insert(index, item.Key);
        _valuesData.Insert(index, item.Value);
    }

    public bool Remove(TKey key)
    {
        var index = _keysData.IndexOf(key);
        if (index >= 0)
        {
            _keysData.RemoveAt(index);
            _valuesData.RemoveAt(index);
            return true;
        }
        return false;
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        var index = _keysData.IndexOf(item.Key);
        if (index >= 0 && Equals(_valuesData[index], item.Value))
        {
            _keysData.RemoveAt(index);
            _valuesData.RemoveAt(index);
            return true;
        }
        return false;
    }

    void IList<KeyValuePair<TKey, TValue>>.RemoveAt(int index)
    {
        _keysData.RemoveAt(index);
        _valuesData.RemoveAt(index);
    }

    public void Clear()
    {
        _keysData.Clear();
        _valuesData.Clear();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var pair in this)
        {
            array[arrayIndex++] = pair;
        }
    }

    public class KeyCollection : ICollection<TKey>
    {
        private readonly List<TKey> _keysData;

        public KeyCollection(List<TKey> keysData)
        {
            _keysData = keysData;
        }

        public int Count => _keysData.Count;

        bool ICollection<TKey>.IsReadOnly => true;

        public IEnumerator<TKey> GetEnumerator() => _keysData.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(TKey item) => _keysData.Contains(item);

        void ICollection<TKey>.Add(TKey item) => throw new NotSupportedException();

        bool ICollection<TKey>.Remove(TKey item) => throw new NotSupportedException();

        void ICollection<TKey>.Clear() => throw new NotSupportedException();

        public void CopyTo(TKey[] array, int arrayIndex) => _keysData.CopyTo(array, arrayIndex);
    }

    public class ValueCollection : ICollection<TValue>
    {
        private readonly List<TValue> _valuesData;

        public ValueCollection(List<TValue> valuesData)
        {
            _valuesData = valuesData;
        }

        public int Count => _valuesData.Count;

        bool ICollection<TValue>.IsReadOnly => true;

        public IEnumerator<TValue> GetEnumerator() => _valuesData.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(TValue item) => _valuesData.Contains(item);

        void ICollection<TValue>.Add(TValue item) => throw new NotSupportedException();

        bool ICollection<TValue>.Remove(TValue item) => throw new NotSupportedException();

        void ICollection<TValue>.Clear() => throw new NotSupportedException();

        public void CopyTo(TValue[] array, int arrayIndex) => _valuesData.CopyTo(array, arrayIndex);
    }
}
