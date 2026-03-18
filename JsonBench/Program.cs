using BenchmarkDotNet.Running;
using JsonBench;
using JsonBench.Configs;

// Generate test data (uncomment as needed)
MinimalFactorialConfigs.GenerateAll();
// SmokeTestConfigs.GenerateAll();
// FactorialConfigs.GenerateAll();

// Run benchmarks (uncomment one at a time)
// BenchmarkRunner.Run<RawPocoStringBench>();
// BenchmarkRunner.Run<RawPocoByteBench>();
// BenchmarkRunner.Run<NodeStringBench>();
// BenchmarkRunner.Run<NodeByteBench>();
// BenchmarkRunner.Run<RawPocoBench>();
// BenchmarkRunner.Run<NodeBench>();
// BenchmarkRunner.Run<JsonSerializationBench>();
