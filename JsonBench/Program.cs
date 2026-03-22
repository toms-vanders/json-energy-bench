using BenchmarkDotNet.Running;
using JsonBench.Benchmarks.Factorial;
using JsonBench.Configs;
using Serialization.Bench.Helpers;

// Uncomment ONE section at a time

// --- Generate test data ---
// GenerateTestData();

// --- Run factorial benchmarks ---
// BenchmarkRunner.Run<FactorialStringBench>();
// BenchmarkRunner.Run<FactorialByteBench>();

void GenerateTestData()
{
    FactorialConfigs.GenerateAll();
}
