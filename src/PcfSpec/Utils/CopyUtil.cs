namespace PcfSpec.Utils;

internal interface ICopyable<out T>
{
    T Copy();

    T DeepCopy();
}

internal static class CopyUtil
{
    public static List<List<byte>> DeepCopyBitmap(List<List<byte>> source)
    {
        var copied = new List<List<byte>>(source.Count);
        foreach (var sourceRow in source)
        {
            copied.Add([.. sourceRow]);
        }
        return copied;
    }

    public static List<T> DeepCopyList<T>(IList<T> source) where T : ICopyable<T>
    {
        var copied = new List<T>(source.Count);
        foreach (var item in source)
        {
            copied.Add(item.DeepCopy());
        }
        return copied;
    }

    public static List<List<List<byte>>> DeepCopyList(IList<List<List<byte>>> source)
    {
        var copied = new List<List<List<byte>>>(source.Count);
        foreach (var item in source)
        {
            copied.Add(DeepCopyBitmap(item));
        }
        return copied;
    }
}
