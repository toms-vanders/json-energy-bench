using JsonGenerator;

namespace JsonBench.Configs.Isolation;

/// <summary>
/// Isolation test: varies special-character density at fixed string length (20 chars),
/// in three variants — Unicode, Escape, UnicodeEscape — compared at matched density.
/// One shared ASCII baseline (A0, density = 0) anchors all three variant curves; the
/// remaining levels are 5/10/25/50/100% density × {U, E, UE}. The low end is weighted
/// (5, 10) so a fast-path cliff is resolvable: with P(≥1 special) = 1−(1−d)^20 over
/// 20 chars saturating to ~88% by 10% density, only the 5% point sits on the rising
/// edge that distinguishes cliff from linear/additive.
/// Baseline: D5, W20, Object-only, R0
/// </summary>
public class StringCompositionIsolationConfig : IsolationBaseConfig
{
    // Densities matched across the three variants.
    private static readonly double[] Densities = [0.05, 0.10, 0.25, 0.50, 1.00];

    protected override string SubDir => "IsoStringComp";

    public override IEnumerable<(string Id, JsonGenConfig Config)> GetAll()
    {
        // Shared ASCII baseline (density = 0); each variant's 0-point reuses this file.
        yield return ("A0", BaseConfig with
        {
            StringMix = new StringMix { Ascii = 1.0 }
        });

        foreach (var d in Densities)
            yield return ($"U{Pct(d)}", BaseConfig with
            {
                StringMix = new StringMix { Ascii = 1.0 - d, Unicode = d }
            });

        foreach (var d in Densities)
            yield return ($"E{Pct(d)}", BaseConfig with
            {
                StringMix = new StringMix { Ascii = 1.0 - d, Escape = d }
            });

        foreach (var d in Densities)
            yield return ($"UE{Pct(d)}", BaseConfig with
            {
                StringMix = new StringMix { Ascii = 1.0 - d, UnicodeEscape = d }
            });
    }

    private static int Pct(double d) => (int)Math.Round(d * 100);
}
