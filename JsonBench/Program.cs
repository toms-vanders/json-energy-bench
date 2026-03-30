// Usage (target frameworks: net8.0, net10.0):
//
//   Generate test data (force regenerate all):
//     dotnet run -c Release -f net10.0 -- generate
//
//   Run specific benchmark by filter:
//     dotnet run -c Release -f net10.0 -- --filter *Smoke*
//     dotnet run -c Release -f net10.0 -- --filter *DepthIsolationString*
//     dotnet run -c Release -f net10.0 -- --filter *Redundancy*
//     dotnet run -c Release -f net10.0 -- --filter *Factorial*
//
//   List all available benchmarks:
//     dotnet run -c Release -f net10.0 -- --list flat
//
//   Interactive menu (no args):
//     dotnet run -c Release -f net10.0

using BenchmarkDotNet.Running;
using JsonBench.Benchmarks;
using JsonBench.Benchmarks.Factorial;
using JsonBench.Benchmarks.Isolation;
using JsonBench.Helpers;

if (args.Length > 0 && args[0] == "generate")
{
    TestDataGenerator.GenerateAll();
    return;
}

TestDataGenerator.EnsureAllGenerated();

BenchmarkSwitcher.FromTypes(
[
    // Smoke
    typeof(SmokeBench),
    // Factorial
    typeof(FactorialStringBench),
    typeof(FactorialByteBench),
    // Factorial (size-normalized)
    typeof(FactorialNormalizedStringBench),
    // Isolation: Depth
    typeof(DepthIsolationStringBench),
    typeof(DepthIsolationByteBench),
    // Isolation: Width
    typeof(WidthIsolationStringBench),
    typeof(WidthIsolationByteBench),
    // Isolation: Escape density
    typeof(EscapeIsolationStringBench),
    typeof(EscapeIsolationByteBench),
    // Isolation: Unicode density
    typeof(UnicodeIsolationStringBench),
    typeof(UnicodeIsolationByteBench),
    // Isolation: Unicode escape density
    typeof(UnicodeEscapeIsolationStringBench),
    typeof(UnicodeEscapeIsolationByteBench),
    // Isolation: Numeric (integer/float ratio)
    typeof(NumericIsolationStringBench),
    typeof(NumericIsolationByteBench),
    // Isolation: Redundancy
    typeof(RedundancyIsolationStringBench),
    typeof(RedundancyIsolationByteBench),
    // Isolation: Size (object count)
    typeof(SizeIsolationStringBench),
    typeof(SizeIsolationByteBench),
]).Run(args);
