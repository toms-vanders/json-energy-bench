using BenchmarkDotNet.Running;
using JsonBench;
using JsonBench.Configs;

// Uncomment ONE section at a time

// --- Generate test data ---
GenerateTestData();

// --- Run benchmarks ---
// BenchmarkRunner.Run<RawPocoStringBench>();
// BenchmarkRunner.Run<RawPocoByteBench>();
// BenchmarkRunner.Run<NodeStringBench>();
// BenchmarkRunner.Run<NodeByteBench>();
// BenchmarkRunner.Run<RawPocoBench>();
// BenchmarkRunner.Run<NodeBench>();

void GenerateTestData()
{
    MinimalFactorialConfigs.GenerateAll();
    // SmokeTestConfigs.GenerateAll();
    // FactorialConfigs.GenerateAll();
}
