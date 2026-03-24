using JsonGenerator;

namespace JsonBench.Configs.Isolation;

/// <summary>
/// Isolation test: varies nesting depth (7 levels, geometric spacing).
/// Baseline: W20, Textual, Object-only, ASCII, R0
/// </summary>
public class DepthIsolationConfig : IsolationBaseConfig
{
    private static readonly (string Label, int Value)[] Depths =
    [
        ("D1", 1),
        ("D2", 2),
        ("D4", 4),
        ("D8", 8),
        ("D15", 15),
        ("D25", 25),
        ("D40", 40),
    ];

    protected override string SubDir => "IsoDepth";

    public override IEnumerable<(string Id, JsonGenConfig Config)> GetAll()
    {
        foreach (var (label, depth) in Depths)
            yield return (label, BaseConfig with { NestingDepth = depth });
    }
}
