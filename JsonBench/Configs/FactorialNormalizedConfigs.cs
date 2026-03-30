using JsonBench.Helpers;
using JsonGenerator;

namespace JsonBench.Configs;

/// <summary>
/// Size-normalized factorial design: adjusts value sizes so that all content types
/// produce roughly the same file size for the same depth/width combination.
///
/// Target: ~5 bytes per leaf value (matching boolean's "true"/"false")
///   - Textual: StringLength = 3 (3 chars + 2 quotes = 5 bytes)
///   - Numeric: IntegerDigits = 5 (~5 bytes), FloatIntegerDigits = 3, FloatDecimalPlaces = 1 (~5 bytes)
///   - Boolean: ~4.5 bytes average (unchanged)
///
/// Dimensions: Depth (4) × Width (4) × Content (3) = 48
/// Naming: D{depth}-W{width}-{content}
/// </summary>
public static class FactorialNormalizedConfigs
{
    private static readonly (string Label, int Value)[] Depths =
    [
        ("D2", 2),
        ("D5", 5),
        ("D10", 10),
        ("D20", 20),
    ];

    private static readonly (string Label, int Value)[] Widths =
    [
        ("W5", 5),
        ("W20", 20),
        ("W50", 50),
        ("W100", 100),
    ];

    private static readonly (string Label, ContentMix Mix, int StringLen, int IntDigits, int FloatIntDigits, int FloatDecPlaces)[] ContentTypes =
    [
        ("T", new ContentMix { Textual = 1.0, Numeric = 0.0, Boolean = 0.0 }, 3, 6, 5, 2),
        ("N", new ContentMix { Textual = 0.0, Numeric = 1.0, Boolean = 0.0 }, 20, 5, 3, 1),
        ("B", new ContentMix { Textual = 0.0, Numeric = 0.0, Boolean = 1.0 }, 20, 6, 5, 2),
    ];

    public static IEnumerable<(string Id, JsonGenConfig Config)> GetAll(int seed = 42)
    {
        foreach (var (depthLabel, depth) in Depths)
        foreach (var (widthLabel, width) in Widths)
        foreach (var (contentLabel, contentMix, stringLen, intDigits, floatIntDigits, floatDecPlaces) in ContentTypes)
        {
            var id = $"{depthLabel}-{widthLabel}-{contentLabel}";

            var config = new JsonGenConfig
            {
                NestingDepth = depth,
                Width = width,
                ContentMix = contentMix,
                NestingMix = new NestingMix { Object = 1.0, Array = 0.0 },
                StringLength = stringLen,
                IntegerDigits = intDigits,
                FloatIntegerDigits = floatIntDigits,
                FloatDecimalPlaces = floatDecPlaces,
                Seed = seed,
            };

            yield return (id, config);
        }
    }

    public static void GenerateAll(string? outputDir = null, int seed = 42)
    {
        outputDir ??= SerializationHelper.TestDataPath("FactorialNormalized");
        Directory.CreateDirectory(outputDir);

        foreach (var (id, config) in GetAll(seed))
        {
            var path = Path.Combine(outputDir, $"{id}.json");

            using var fs = File.Create(path);
            var result = new JsonTreeBuilder(config).Generate(fs);

            Console.WriteLine($"{id}: {result}");
        }
    }
}
