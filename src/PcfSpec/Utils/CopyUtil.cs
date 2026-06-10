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

    public static void DeepCopyToList<T>(IList<T> source, IList<T> target) where T : ICopyable<T>
    {
        foreach (var item in source)
        {
            target.Add(item.DeepCopy());
        }
    }

    public static void DeepCopyToList(IList<List<List<byte>>> source, IList<List<List<byte>>> target)
    {
        foreach (var item in source)
        {
            target.Add(DeepCopyBitmap(item));
        }
    }
}
