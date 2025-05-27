using PcfSpec;

var outputsDir = Path.Combine("build");
if (Directory.Exists(outputsDir))
{
    Directory.Delete(outputsDir, true);
}
Directory.CreateDirectory(outputsDir);

var font = PcfFont.Load(Path.Combine("assets", "unifont", "unifont-16.0.03.pcf"));
Console.WriteLine($"name: {font.Properties!.Font}");
Console.WriteLine($"size: {font.Properties!.PointSize}");
Console.WriteLine($"ascent: {font.Accelerators!.FontAscent}");
Console.WriteLine($"descent: {font.Accelerators!.FontDescent}");
Console.WriteLine();
foreach (var (encoding, glyphIndex) in font.BdfEncodings!)
{
    var glyphName = font.GlyphNames![glyphIndex];
    var metric = font.Metrics![glyphIndex];
    var bitmap = font.Bitmaps![glyphIndex];
    Console.WriteLine($"char: {char.ConvertFromUtf32(encoding)} ({encoding:X4})");
    Console.WriteLine($"glyphName: {glyphName}");
    Console.WriteLine($"advanceWidth: {metric.CharacterWidth}");
    Console.WriteLine($"dimensions: {metric.Dimensions}");
    Console.WriteLine($"offset: {metric.Offset}");
    foreach (var bitmapRow in bitmap)
    {
        var text = string.Join("", bitmapRow).Replace("0", "  ").Replace("1", "██");
        Console.WriteLine($"{text}*");
    }
    Console.WriteLine();
}
font.Save(Path.Combine(outputsDir, "unifont-16.0.03.pcf"));
