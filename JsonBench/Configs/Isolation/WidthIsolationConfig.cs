using JsonGenerator;

namespace JsonBench.Configs.Isolation;

/// <summary>
/// Isolation test: varies structural width (7 levels, geometric spacing).
/// Baseline: D5, Textual, Object-only, ASCII, R0
/// </summary>
public class WidthIsolationConfig : IsolationBaseConfig
{
    private static readonly (string Label, int Value)[] Widths =
    [
        ("W2", 2),
        ("W5", 5),
        ("W10", 10),
        ("W20", 20),
        ("W50", 50),
        ("W100", 100),
        ("W200", 200),
    ];

    protected override string SubDir => "IsoWidth";

    public override IEnumerable<(string Id, JsonGenConfig Config)> GetAll()
    {
        foreach (var (label, width) in Widths)
            yield return (label, BaseConfig with { Width = width });
    }
}
