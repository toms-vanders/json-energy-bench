using JsonGenerator;

namespace JsonBench.Configs.Isolation;

/// <summary>
/// Isolation test: varies \uXXXX Unicode escape density (5 levels: 0%, 10%, 30%, 50%, 70%).
/// Same character ranges as literal Unicode but encoded as \uXXXX in JSON wire format.
/// Tests parser cost of hex decoding + UTF-8 emission.
/// Baseline: D5, W20, Textual, Object-only, ASCII, R0
/// </summary>
public class UnicodeEscapeIsolationConfig : IsolationBaseConfig
{
    private static readonly (string Label, double UnicodeEscapeRatio)[] Levels =
    [
        ("UE0", 0.0),
        ("UE10", 0.1),
        ("UE30", 0.3),
        ("UE50", 0.5),
        ("UE70", 0.7),
    ];

    protected override string SubDir => "IsoUnicodeEscape";

    public override IEnumerable<(string Id, JsonGenConfig Config)> GetAll()
    {
        foreach (var (label, unicodeEscape) in Levels)
            yield return (label, BaseConfig with
            {
                StringMix = new StringMix { Ascii = 1.0 - unicodeEscape, UnicodeEscape = unicodeEscape }
            });
    }
}
