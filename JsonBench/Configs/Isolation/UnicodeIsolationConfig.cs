using JsonGenerator;

namespace JsonBench.Configs.Isolation;

/// <summary>
/// Isolation test: varies literal Unicode density (5 levels: 0%, 10%, 30%, 50%, 70%).
/// Unicode ranges: Latin Extended, Cyrillic, CJK subset, Arabic (direct UTF-8 multi-byte chars).
/// Baseline: D5, W20, Textual, Object-only, ASCII, R0
/// </summary>
public class UnicodeIsolationConfig : IsolationBaseConfig
{
    private static readonly (string Label, double UnicodeRatio)[] Levels =
    [
        ("U0", 0.0),
        ("U10", 0.1),
        ("U30", 0.3),
        ("U50", 0.5),
        ("U70", 0.7),
    ];

    protected override string SubDir => "IsoUnicode";

    public override IEnumerable<(string Id, JsonGenConfig Config)> GetAll()
    {
        foreach (var (label, unicode) in Levels)
            yield return (label, BaseConfig with
            {
                StringMix = new StringMix { Ascii = 1.0 - unicode, Unicode = unicode }
            });
    }
}
