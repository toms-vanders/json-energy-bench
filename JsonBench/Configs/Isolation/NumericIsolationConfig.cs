using JsonGenerator;

namespace JsonBench.Configs.Isolation;

/// <summary>
/// Isolation test: varies integer/float ratio (5 levels: 100% int to 100% float).
/// Content is 100% numeric to isolate number parsing from string/bool handling.
/// Baseline: D5, W20, Object-only, ASCII, R0
/// </summary>
public class NumericIsolationConfig : IsolationBaseConfig
{
    private static readonly (string Label, double IntegerRatio)[] Levels =
    [
        ("I100", 1.0),
        ("I70", 0.7),
        ("I50", 0.5),
        ("I30", 0.3),
        ("F100", 0.0),
    ];

    protected override string SubDir => "IsoNumeric";

    public override IEnumerable<(string Id, JsonGenConfig Config)> GetAll()
    {
        foreach (var (label, intRatio) in Levels)
            yield return (label, BaseConfig with
            {
                ContentMix = new ContentMix { Textual = 0.0, Numeric = 1.0, Boolean = 0.0 },
                NumericMix = new NumericMix { Integer = intRatio, Float = 1.0 - intRatio }
            });
    }
}
