using JsonGenerator;
using JsonBench.Helpers;

namespace JsonBench.Configs.Isolation;

/// <summary>
/// Base class for isolation tests. Each isolation test varies one dimension
/// while holding all others at these baseline values:
///   Depth: 5, Width: 20, Content: Textual, NestingMode: Object-only,
///   StringSubtype: ASCII, Redundancy: 0%
/// </summary>
public abstract class IsolationBaseConfig
{
    protected const int BaseDepth = 5;
    protected const int BaseWidth = 20;
    protected const int BaseSeed = 42;
    protected const double BaseRedundancy = 0.0;

    protected static readonly ContentMix BaseContent =
        new() { Textual = 1.0, Numeric = 0.0, Boolean = 0.0 };

    protected static readonly NestingMix BaseNestingMix =
        new() { Object = 1.0, Array = 0.0 };

    protected static readonly StringMix BaseStringMix =
        new() { Ascii = 1.0, Unicode = 0.0, Escape = 0.0 };

    /// <summary>
    /// Creates a config with all baseline values. Use 'with' expression to override properties.
    /// </summary>
    protected static JsonGenConfig BaseConfig => new()
    {
        NestingDepth = BaseDepth,
        Width = BaseWidth,
        ContentMix = BaseContent,
        NestingMix = BaseNestingMix,
        StringMix = BaseStringMix,
        RedundancyRatio = BaseRedundancy,
        Seed = BaseSeed,
    };

    /// <summary>
    /// Returns all configurations for this isolation test.
    /// </summary>
    public abstract IEnumerable<(string Id, JsonGenConfig Config)> GetAll();

    /// <summary>
    /// The subdirectory name under TestData for this isolation test.
    /// </summary>
    protected abstract string SubDir { get; }

    /// <summary>
    /// Generates all JSON files for this isolation test.
    /// </summary>
    public void GenerateAll(string? outputDir = null)
    {
        outputDir ??= SerializationHelper.TestDataPath(SubDir);
        Directory.CreateDirectory(outputDir);

        foreach (var (id, config) in GetAll())
        {
            var path = Path.Combine(outputDir, $"{id}.json");

            using var fs = File.Create(path);
            var result = new JsonTreeBuilder(config).Generate(fs);

            Console.WriteLine($"{id}: {result}");
        }
    }
}
