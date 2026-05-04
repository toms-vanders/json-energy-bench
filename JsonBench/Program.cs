// Usage:
//
//   Generate test data (force regenerate all):
//     dotnet run -c Release -- generate
//
//   Run specific benchmark by filter:
//     dotnet run -c Release -- --filter *Smoke*
//     dotnet run -c Release -- --filter *DepthIsolationString*
//     dotnet run -c Release -- --filter *Redundancy*
//     dotnet run -c Release -- --filter *Factorial*
//
//   List all available benchmarks:
//     dotnet run -c Release -- --list flat
//
//   Interactive menu (no args):
//     dotnet run -c Release

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
    typeof(SmokeBenchString),
    typeof(SmokeBenchByte),
    // Factorial
    typeof(FactorialStringBench),
    typeof(FactorialByteBench),
    // Factorial (size-normalized)
    typeof(FactorialNormalizedStringBench),
    typeof(FactorialNormalizedByteBench),
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
