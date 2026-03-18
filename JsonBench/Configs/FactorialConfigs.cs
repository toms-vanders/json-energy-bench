using JsonGenerator;
using Serialization.Bench.Helpers;

namespace JsonBench.Configs;

/// <summary>
/// Generates the 36-configuration factorial design for benchmarking.
/// Dimensions: Depth (3) × Width (2) × Content (3) × Redundancy (2) = 36
///
/// Naming: D{depth}-W{width}-{content}-R{redundancy}
/// Example: D2-W5-T-R0 = depth 2, width 5, textual, non-redundant
/// </summary>
public static class FactorialConfigs
{
    private static readonly (string Label, int Value)[] Depths =
    [
        ("D2", 2),
        ("D6", 6),
        ("D10", 10),
    ];

    private static readonly (string Label, int Value)[] Widths =
    [
        ("W5", 5),
        ("W10", 10),
    ];

    private static readonly (string Label, ContentMix Mix)[] ContentTypes =
    [
        ("T", new ContentMix { Textual = 1.0, Numeric = 0.0, Boolean = 0.0 }),
        ("N", new ContentMix { Textual = 0.0, Numeric = 1.0, Boolean = 0.0 }),
        ("B", new ContentMix { Textual = 0.0, Numeric = 0.0, Boolean = 1.0 }),
    ];

    private static readonly (string Label, double Ratio)[] Redundancies =
    [
        ("R0", 0.0),
        ("R25", 0.25),
    ];

    /// <summary>
    /// Returns all 36 factorial configurations.
    /// </summary>
    public static IEnumerable<(string Id, JsonGenConfig Config)> GetAll(int seed = 42)
    {
        foreach (var (depthLabel, depth) in Depths)
        foreach (var (widthLabel, width) in Widths)
        foreach (var (contentLabel, contentMix) in ContentTypes)
        foreach (var (redLabel, redRatio) in Redundancies)
        {
            var id = $"{depthLabel}-{widthLabel}-{contentLabel}-{redLabel}";

            var config = new JsonGenConfig
            {
                NestingDepth = depth,
                Width = width,
                ContentMix = contentMix,
                NestingMix = new NestingMix { Object = 0.5, Array = 0.5 },
                RedundancyRatio = redRatio,
                Seed = seed,
            };

            yield return (id, config);
        }
    }

    /// <summary>
    /// Generates all 36 JSON files to the TestData directory and prints metadata.
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
