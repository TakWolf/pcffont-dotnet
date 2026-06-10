using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace PcfSpec.Utils;

internal class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IList<KeyValuePair<TKey, TValue>> where TKey : notnull
{
    private readonly List<KeyValuePair<TKey, TValue>> _items;
    private readonly Dictionary<TKey, int> _indices;
    private readonly IEqualityComparer<TKey> _comparer;
    private KeyCollection? _keys;
    private ValueCollection? _values;

    public OrderedDictionary() : this(0, null) { }

    public OrderedDictionary(int capacity) : this(capacity, null) { }

    public OrderedDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer) { }

    public OrderedDictionary(int capacity, IEqualityComparer<TKey>? comparer)
    {
        _items = new List<KeyValuePair<TKey, TValue>>(capacity);
        _indices = new Dictionary<TKey, int>(capacity, comparer);
        _comparer = comparer ?? EqualityComparer<TKey>.Default;
    }

    public OrderedDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null) { }

    public OrderedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer) : this(dictionary.Count, comparer)
    {
        foreach (var (key, value) in dictionary)
        {
            Add(key, value);
        }
    }

    public OrderedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, null) { }

    public OrderedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer) :
        this((collection as ICollection<KeyValuePair<TKey, TValue>>)?.Count ?? 0, comparer)
    {
        foreach (var (key, value) in collection)
        {
            Add(key, value);
        }
    }

    public int Count => _items.Count;

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

    public KeyCollection Keys => _keys ??= new KeyCollection(_items, _indices);

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

    public ValueCollection Values => _values ??= new ValueCollection(_items);

    ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public TValue this[TKey key]
    {
        get
        {
            if (_indices.TryGetValue(key, out var index))
            {
                return _items[index].Value;
            }
            throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary.");
        }
        set
        {
            if (_indices.TryGetValue(key, out var index))
            {
                var oldKey = _items[index].Key;
                _items[index] = new KeyValuePair<TKey, TValue>(oldKey, value);
            }
            else
            {
                _indices.Add(key, _items.Count);
                _items.Add(new KeyValuePair<TKey, TValue>(key, value));
            }
        }
    }

    KeyValuePair<TKey, TValue> IList<KeyValuePair<TKey, TValue>>.this[int index]
    {
        get => _items[index];
        set
        {
            var newKey = value.Key;

            if (_indices.TryGetValue(newKey, out var existsIndex) && existsIndex != index)
            {
                throw new ArgumentException($"An element with the same key '{newKey}' already exists in the dictionary.");
            }

            var oldKey = _items[index].Key;
            _items[index] = value;

            if (!_comparer.Equals(oldKey, newKey))
            {
                _indices.Remove(oldKey);
                _indices[newKey] = index;
            }
        }
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (_indices.TryGetValue(key, out var index))
        {
            value = _items[index].Value;
            return true;
        }
        value = default;
        return false;
    }

    public bool ContainsKey(TKey key) => _indices.ContainsKey(key);

    public bool ContainsValue(TValue value)
    {
        foreach (var item in _items)
        {
            if (EqualityComparer<TValue>.Default.Equals(item.Value, value))
            {
                return true;
            }
        }
        return false;
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        if (_indices.TryGetValue(item.Key, out var index))
        {
            return EqualityComparer<TValue>.Default.Equals(_items[index].Value, item.Value);
        }
        return false;
    }

    int IList<KeyValuePair<TKey, TValue>>.IndexOf(KeyValuePair<TKey, TValue> item)
    {
        if (_indices.TryGetValue(item.Key, out var index) && EqualityComparer<TValue>.Default.Equals(_items[index].Value, item.Value))
        {
            return index;
        }
        return -1;
    }

    public void Add(TKey key, TValue value)
    {
        if (_indices.ContainsKey(key))
        {
            throw new ArgumentException($"An element with the same key '{key}' already exists in the dictionary.");
        }
        _indices.Add(key, _items.Count);
        _items.Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

    void IList<KeyValuePair<TKey, TValue>>.Insert(int index, KeyValuePair<TKey, TValue> item)
    {
        if (_indices.ContainsKey(item.Key))
        {
            throw new ArgumentException($"An element with the same key '{item.Key}' already exists in the dictionary.");
        }
        _items.Insert(index, item);
        RebuildIndicesFrom(index);
    }

    public bool Remove(TKey key)
    {
        if (!_indices.TryGetValue(key, out var index))
        {
            return false;
        }
        _items.RemoveAt(index);
        _indices.Remove(key);
        RebuildIndicesFrom(index);
        return true;
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        if (!_indices.TryGetValue(item.Key, out var index) || !EqualityComparer<TValue>.Default.Equals(_items[index].Value, item.Value))
        {
            return false;
        }
        _items.RemoveAt(index);
        _indices.Remove(item.Key);
        RebuildIndicesFrom(index);
        return true;
    }

    void IList<KeyValuePair<TKey, TValue>>.RemoveAt(int index)
    {
        var key = _items[index].Key;
        _items.RemoveAt(index);
        _indices.Remove(key);
        RebuildIndicesFrom(index);
    }

    public void Clear()
    {
        _items.Clear();
        _indices.Clear();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    private void RebuildIndicesFrom(int startIndex)
    {
        for (var index = startIndex; index < _items.Count; index++)
        {
            _indices[_items[index].Key] = index;
        }
    }

    public class KeyCollection : ICollection<TKey>
    {
        private readonly List<KeyValuePair<TKey, TValue>> _items;
        private readonly Dictionary<TKey, int> _indices;

        public KeyCollection(List<KeyValuePair<TKey, TValue>> items, Dictionary<TKey, int> indices)
        {
            _items = items;
            _indices = indices;
        }

        public int Count => _items.Count;

        bool ICollection<TKey>.IsReadOnly => true;

        public IEnumerator<TKey> GetEnumerator()
        {
            foreach (var item in _items)
            {
                yield return item.Key;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(TKey key) => _indices.ContainsKey(key);

        void ICollection<TKey>.Add(TKey item) => throw new NotSupportedException();

        bool ICollection<TKey>.Remove(TKey item) => throw new NotSupportedException();

        void ICollection<TKey>.Clear() => throw new NotSupportedException();

        public void CopyTo(TKey[] array, int arrayIndex)
        {
            foreach (var item in _items)
            {
                array[arrayIndex++] = item.Key;
            }
        }
    }

    public class ValueCollection : ICollection<TValue>
    {
        private readonly List<KeyValuePair<TKey, TValue>> _items;

        public ValueCollection(List<KeyValuePair<TKey, TValue>> items)
        {
            _items = items;
        }

        public int Count => _items.Count;

        bool ICollection<TValue>.IsReadOnly => true;

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (var item in _items)
            {
                yield return item.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(TValue value)
        {
            foreach (var item in _items)
            {
                if (EqualityComparer<TValue>.Default.Equals(item.Value, value))
                {
                    return true;
                }
            }
            return false;
        }

        void ICollection<TValue>.Add(TValue item) => throw new NotSupportedException();

        bool ICollection<TValue>.Remove(TValue item) => throw new NotSupportedException();

        void ICollection<TValue>.Clear() => throw new NotSupportedException();

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            foreach (var item in _items)
            {
                array[arrayIndex++] = item.Value;
            }
        }
    }
}
