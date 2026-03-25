using JsonGenerator;

namespace JsonBench.Configs.Isolation;

/// <summary>
/// Isolation test: varies value redundancy ratio (5 levels: 0% to 95% duplicates).
/// Baseline: D5, W20, Textual, Object-only, ASCII
/// </summary>
public class RedundancyIsolationConfig : IsolationBaseConfig
{
    private static readonly (string Label, double Ratio)[] Levels =
    [
        ("R0", 0.0),
        ("R25", 0.25),
        ("R50", 0.50),
        ("R75", 0.75),
        ("R95", 0.95),
    ];

    protected override string SubDir => "IsoRedundancy";

    public override IEnumerable<(string Id, JsonGenConfig Config)> GetAll()
    {
        foreach (var (label, ratio) in Levels)
            yield return (label, BaseConfig with { RedundancyRatio = ratio });
    }
}
