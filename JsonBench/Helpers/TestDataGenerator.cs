using JsonBench.Configs;
using JsonBench.Configs.Isolation;

namespace JsonBench.Helpers;

public static class TestDataGenerator
{
    public static void GenerateAll()
    {
        FactorialConfigs.GenerateAll();
        FactorialNormalizedConfigs.GenerateAll();
        new SmokeIsolationConfig().GenerateAll();
        new DepthIsolationConfig().GenerateAll();
        new WidthIsolationConfig().GenerateAll();
        new EscapeIsolationConfig().GenerateAll();
        new UnicodeIsolationConfig().GenerateAll();
        new UnicodeEscapeIsolationConfig().GenerateAll();
        new NumericIsolationConfig().GenerateAll();
        new RedundancyIsolationConfig().GenerateAll();
        new SizeIsolationConfig().GenerateAll();
    }

    public static void EnsureAllGenerated()
    {
        Ensure("Factorial", () => FactorialConfigs.GenerateAll());
        Ensure("FactorialNormalized", () => FactorialNormalizedConfigs.GenerateAll());
        Ensure("Smoke", () => new SmokeIsolationConfig().GenerateAll());
        Ensure("IsoDepth", () => new DepthIsolationConfig().GenerateAll());
        Ensure("IsoWidth", () => new WidthIsolationConfig().GenerateAll());
        Ensure("IsoEscape", () => new EscapeIsolationConfig().GenerateAll());
        Ensure("IsoUnicode", () => new UnicodeIsolationConfig().GenerateAll());
        Ensure("IsoUnicodeEscape", () => new UnicodeEscapeIsolationConfig().GenerateAll());
        Ensure("IsoNumeric", () => new NumericIsolationConfig().GenerateAll());
        Ensure("IsoRedundancy", () => new RedundancyIsolationConfig().GenerateAll());
        Ensure("IsoSize", () => new SizeIsolationConfig().GenerateAll());
    }

    private static void Ensure(string subDir, Action generate)
    {
        if (!SerializationHelper.TestDataExists(subDir))
        {
            Console.WriteLine($"Generating missing test data: {subDir}...");
            generate();
        }
    }
}
