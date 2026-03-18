using JsonGenerator;
using Serialization.Bench.Helpers;

namespace JsonBench.Configs;
/// <summary>
/// Minimal configurations for local testing / smoke tests.
/// Small, fast to generate and benchmark — just verifies everything runs end-to-end.
/// </summary>
public static class SmokeTestConfigs
{
    public static IEnumerable<(string Id, JsonGenConfig Config)> GetAll(int seed = 42)
    {
        yield return ("Smoke-Flat-T", new JsonGenConfig
        {
            NestingDepth = 2,
            Width = 5,
            ContentMix = new ContentMix { Textual = 1.0, Numeric = 0.0, Boolean = 0.0 },
            Seed = seed,
        });

        yield return ("Smoke-Deep-N", new JsonGenConfig
        {
            NestingDepth = 4,
            Width = 3,
            ContentMix = new ContentMix { Textual = 0.0, Numeric = 1.0, Boolean = 0.0 },
            RedundancyRatio = 0.25,
            Seed = seed,
        });
    }

    public static void GenerateAll(string? outputDir = null, int seed = 42)
    {
        outputDir ??= SerializationHelper.TestDataPath("Smoke");
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
