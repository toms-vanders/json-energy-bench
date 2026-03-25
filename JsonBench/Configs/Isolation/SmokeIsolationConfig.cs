using JsonGenerator;

namespace JsonBench.Configs.Isolation;

/// <summary>
/// Smoke test: single baseline config for quick sanity checks.
/// </summary>
public class SmokeIsolationConfig : IsolationBaseConfig
{
    protected override string SubDir => "Smoke";

    public override IEnumerable<(string Id, JsonGenConfig Config)> GetAll()
    {
        yield return ("Smoke", BaseConfig);
    }
}
