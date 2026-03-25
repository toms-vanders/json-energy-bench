using JsonGenerator;

namespace JsonBench.Configs.Isolation;

/// <summary>
/// Isolation test: varies simple escape density (5 levels: 0%, 10%, 30%, 50%, 70%).
/// Escape sequences: \n, \t, \\, \", \r, \b, \f
/// Baseline: D5, W20, Textual, Object-only, ASCII, R0
/// </summary>
public class EscapeIsolationConfig : IsolationBaseConfig
{
    private static readonly (string Label, double EscapeRatio)[] Levels =
    [
        ("E0", 0.0),
        ("E10", 0.1),
        ("E30", 0.3),
        ("E50", 0.5),
        ("E70", 0.7),
    ];

    protected override string SubDir => "IsoEscape";

    public override IEnumerable<(string Id, JsonGenConfig Config)> GetAll()
    {
        foreach (var (label, escape) in Levels)
            yield return (label, BaseConfig with
            {
                StringMix = new StringMix { Ascii = 1.0 - escape, Escape = escape }
            });
    }
}
