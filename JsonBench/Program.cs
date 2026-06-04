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
using JsonBench;
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
    typeof(SmokeBenchByte),
    // Factorial
    typeof(FactorialByteBench),
    // Factorial (size-normalized)
    typeof(FactorialNormalizedByteBench),
    // Isolation: Depth
    typeof(DepthIsolationByteBench),
    // Isolation: Width
    typeof(WidthIsolationByteBench),
    // Isolation: String composition (Unicode / Escape / UnicodeEscape density × shared ASCII baseline)
    typeof(StringCompositionIsolationByteBench),
    // Isolation: Numeric (value length, int & float)
    typeof(NumericIsolationByteBench),
    // Isolation: Redundancy
    typeof(RedundancyIsolationByteBench),
    // Isolation: Size (object count)
    typeof(SizeIsolationByteBench),
    // Isolation: String length (string value length)
    typeof(StringLengthIsolationByteBench),
]).Run(args);
