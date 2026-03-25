using JsonBench.Helpers;
using JsonGenerator;

namespace JsonBench.Configs;

/// <summary>
/// Generates the 48-configuration factorial design for benchmarking.
/// Dimensions: Depth (4) × Width (4) × Content (3) = 48
///
/// Naming: D{depth}-W{width}-{content}
/// Example: D2-W5-T = depth 2, width 5, textual
/// </summary>
public static class FactorialConfigs
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

    private static readonly (string Label, ContentMix Mix)[] ContentTypes =
    [
        ("T", new ContentMix { Textual = 1.0, Numeric = 0.0, Boolean = 0.0 }),
        ("N", new ContentMix { Textual = 0.0, Numeric = 1.0, Boolean = 0.0 }),
        ("B", new ContentMix { Textual = 0.0, Numeric = 0.0, Boolean = 1.0 }),
    ];

    /// <summary>
    /// Returns all 48 factorial configurations.
    /// </summary>
    public static IEnumerable<(string Id, JsonGenConfig Config)> GetAll(int seed = 42)
    {
        foreach (var (depthLabel, depth) in Depths)
        foreach (var (widthLabel, width) in Widths)
        foreach (var (contentLabel, contentMix) in ContentTypes)
        {
            var id = $"{depthLabel}-{widthLabel}-{contentLabel}";

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

    /// <summary>
    /// Generates all 48 JSON files to the TestData directory and prints metadata.
    /// </summary>
    public static void GenerateAll(string? outputDir = null, int seed = 42)
    {
        outputDir ??= SerializationHelper.TestDataPath("Factorial");
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
