using BenchmarkDotNet.Attributes;
using JsonBench.Configs;
using JsonBench.Models.MinimalFactorial;
using Serialization.Bench;
using Serialization.Bench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench;

/// <summary>
/// Node with IOps interface dispatch — for comparing overhead against direct benchmarks.
/// STJ only.
/// </summary>
[Config(typeof(BenchConfig))]
public class NodeBench
{
    [ParamsSource(nameof(WorkloadIds))]
    public string WorkloadId { get; set; } = "";

    private byte[] _jsonBytes = null!;
    private IOps _ops = null!;

    public static IEnumerable<string> WorkloadIds
        => MinimalFactorialConfigs.GetAll().Select(x => x.Id);

    [GlobalSetup]
    public void Setup()
    {
        var path = SerializationHelper.TestDataFile("MinimalFactorial", $"{WorkloadId}.json");
        _jsonBytes = File.ReadAllBytes(path);

        _ops = WorkloadId switch
        {
            "D2-W10-T" or "D10-W10-T" => new Ops<Node10<string>>(),
            "D2-W10-N" or "D10-W10-N" => new Ops<Node10<int>>(),
            "D2-W10-B" or "D10-W10-B" => new Ops<Node10<bool>>(),
            "D2-W100-T" or "D10-W100-T" => new Ops<Node100<string>>(),
            "D2-W100-N" or "D10-W100-N" => new Ops<Node100<int>>(),
            "D2-W100-B" or "D10-W100-B" => new Ops<Node100<bool>>(),
            _ => throw new ArgumentException($"Unknown workload: {WorkloadId}")
        };

        _ops.PreDeserialize(_jsonBytes);
    }

    [Benchmark, BenchmarkCategory("Deserialize")]
    public object STJ_Deserialize() => _ops.StjDeserialize(_jsonBytes);

    [Benchmark, BenchmarkCategory("Serialize")]
    public string STJ_Serialize() => _ops.StjSerialize();

    private interface IOps
    {
        void PreDeserialize(byte[] bytes);
        object StjDeserialize(byte[] bytes);
        string StjSerialize();
    }

    private class Ops<T> : IOps where T : class
    {
        private T _obj = null!;

        public void PreDeserialize(byte[] bytes)
            => _obj = JsonSerializer.Deserialize<T>(bytes)!;

        public object StjDeserialize(byte[] bytes)
            => JsonSerializer.Deserialize<T>(bytes)!;

        public string StjSerialize()
            => JsonSerializer.Serialize(_obj);
    }
}
