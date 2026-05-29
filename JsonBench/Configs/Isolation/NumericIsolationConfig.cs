using JsonGenerator;

namespace JsonBench.Configs.Isolation;

/// <summary>
/// Isolation test: varies numeric value length (5 levels: 2, 3, 5, 7, 10 digits) in
/// two variants — integer (Int32, labels I2..I10) and float (double, labels F2..F10) —
/// compared at matched digit counts. Content is 100% numeric to isolate number
/// parsing/formatting from string/bool handling.
/// Baseline: D5, W20, Object-only, ASCII, R0
/// </summary>
public class NumericIsolationConfig : IsolationBaseConfig
{
    // Significant-digit / digit-count levels, matched across both variants. Capped at
    // 10: the generator clamps 10-digit integers to Int32 (max 2,147,483,646).
    private static readonly int[] Lengths = [2, 3, 5, 7, 10];

    protected override string SubDir => "IsoNumeric";

    public override IEnumerable<(string Id, JsonGenConfig Config)> GetAll()
    {
        foreach (var len in Lengths)
            yield return ($"I{len}", BaseConfig with
            {
                ContentMix = new ContentMix { Textual = 0.0, Numeric = 1.0, Boolean = 0.0 },
                NumericMix = new NumericMix { Integer = 1.0, Float = 0.0 },
                IntegerDigits = len
            });

        // Float of length L = 1 integer digit + (L-1) decimals → L significant digits.
        foreach (var len in Lengths)
            yield return ($"F{len}", BaseConfig with
            {
                ContentMix = new ContentMix { Textual = 0.0, Numeric = 1.0, Boolean = 0.0 },
                NumericMix = new NumericMix { Integer = 0.0, Float = 1.0 },
                FloatIntegerDigits = 1,
                FloatDecimalPlaces = len - 1
            });
    }
}
