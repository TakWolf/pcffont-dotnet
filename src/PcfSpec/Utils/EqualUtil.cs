namespace PcfSpec.Utils;

internal static class EqualUtil
{
    public static bool BitmapEquals(List<List<byte>> left, List<List<byte>> right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }
        if (left.Count != right.Count)
        {
            return false;
        }
        for (var y = 0; y < left.Count; y++)
        {
            var leftRow = left[y];
            var rightRow = right[y];
            if (ReferenceEquals(leftRow, rightRow))
            {
                continue;
            }
            if (leftRow.Count != rightRow.Count)
            {
                return false;
            }
            for (var x = 0; x < leftRow.Count; x++)
            {
                if (leftRow[x] != rightRow[x])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static bool ListEquals<T>(IList<T> left, IList<T> right) where T : IEquatable<T>
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }
        if (left.Count != right.Count)
        {
            return false;
        }
        for (var i = 0; i < left.Count; i++)
        {
            if (!left[i].Equals(right[i]))
            {
                return false;
            }
        }
        return true;
    }

    public static bool ListEquals(IList<List<List<byte>>> left, IList<List<List<byte>>> right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }
        if (left.Count != right.Count)
        {
            return false;
        }
        for (var i = 0; i < left.Count; i++)
        {
            if (!BitmapEquals(left[i], right[i]))
            {
                return false;
            }
        }
        return true;
    }

    public static bool DictionaryEquals<TKey, TValue>(IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right) where TValue : IEquatable<TValue>
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }
        if (left.Count != right.Count)
        {
            return false;
        }
        foreach (var (key, leftValue) in left)
        {
            if (!right.TryGetValue(key, out var rightValue) || !leftValue.Equals(rightValue))
            {
                return false;
            }
        }
        return true;
    }

    public static bool DictionaryEquals(IDictionary<PcfTableType, IPcfTable> left, IDictionary<PcfTableType, IPcfTable> right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }
        if (left.Count != right.Count)
        {
            return false;
        }
        foreach (var (key, leftValue) in left)
        {
            if (!right.TryGetValue(key, out var rightValue) || !EqualityComparer<IPcfTable>.Default.Equals(leftValue, rightValue))
            {
                return false;
            }
        }
        return true;
    }
}
