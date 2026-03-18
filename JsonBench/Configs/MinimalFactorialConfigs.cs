using JsonGenerator;
using Serialization.Bench.Helpers;

namespace JsonBench.Configs;

/// <summary>
/// Reduced factorial config for local testing.
/// Dimensions: Depth (2) × Width (2) × Content (3) = 12 configs
///
/// Naming: D{depth}-W{width}-{content}
/// </summary>
public static class MinimalFactorialConfigs
{
    private static readonly int[] Depths = [2, 10];

    private static readonly int[] Widths = [10, 100];

    private static readonly (string Label, ContentMix Mix)[] ContentTypes =
    [
        ("T", new ContentMix { Textual = 1.0, Numeric = 0.0, Boolean = 0.0 }),
        ("N", new ContentMix { Textual = 0.0, Numeric = 1.0, Boolean = 0.0 }),
        ("B", new ContentMix { Textual = 0.0, Numeric = 0.0, Boolean = 1.0 }),
    ];

    public static IEnumerable<(string Id, JsonGenConfig Config)> GetAll(int seed = 42)
    {
        foreach (var depth in Depths)
        foreach (var width in Widths)
        foreach (var (contentLabel, contentMix) in ContentTypes)
        {
            var id = $"D{depth}-W{width}-{contentLabel}";

            var config = new JsonGenConfig
            {
                NestingDepth = depth,
                Width = width,
                ContentMix = contentMix,
                NestingMix = new NestingMix { Object = 1.0, Array = 0.0 },
                Seed = seed,
            };

            yield return (id, config);
        }
    }

    public static void GenerateAll(string? outputDir = null, int seed = 42)
    {
        outputDir ??= SerializationHelper.TestDataPath("MinimalFactorial");
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
