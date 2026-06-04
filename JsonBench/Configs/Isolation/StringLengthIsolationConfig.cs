using JsonGenerator;

namespace JsonBench.Configs.Isolation;

/// <summary>
/// Isolation test: varies string value length (7 levels, geometric spacing).
/// Baseline: D5, W20, Textual, Object-only, ASCII, R0
/// </summary>
public class StringLengthIsolationConfig : IsolationBaseConfig
{
    private static readonly (string Label, int Value)[] Lengths =
    [
        ("L5", 5),
        ("L20", 20),
        ("L50", 50),
        ("L200", 200),
        ("L500", 500),
        ("L2000", 2_000),
        ("L10000", 10_000),
    ];

    protected override string SubDir => "IsoStringLength";

    public override IEnumerable<(string Id, JsonGenConfig Config)> GetAll()
    {
        foreach (var (label, length) in Lengths)
            yield return (label, BaseConfig with { StringLength = length });
    }
}
