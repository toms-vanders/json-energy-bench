using JsonGenerator;

namespace JsonBench.Configs.Isolation;

/// <summary>
/// Isolation test: varies object count using log10 scaling (6 levels: 1 to 100,000).
/// Baseline: D5, W20, Textual, Object-only, ASCII, R0
/// Count > 1 wraps objects in {"Items": [...]}.
/// </summary>
public class SizeIsolationConfig : IsolationBaseConfig
{
    private static readonly (string Label, int Count)[] Levels =
    [
        ("C10", 10),
        ("C100", 100),
        ("C1K", 1_000),
        ("C10K", 10_000),
        ("C100K", 100_000),
    ];

    protected override string SubDir => "IsoSize";

    public override IEnumerable<(string Id, JsonGenConfig Config)> GetAll()
    {
        foreach (var (label, count) in Levels)
            yield return (label, BaseConfig with { Count = count });
    }
}
