using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Configs;
using JsonBench.Models.MinimalFactorial;
using Serialization.Bench;
using Serialization.Bench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench;

[Config(typeof(BenchConfig))]public class RawPocoBench
{
    [ParamsSource(nameof(WorkloadIds))]
    public string WorkloadId { get; set; } = "";

    private byte[] _jsonBytes = null!;
    private string _jsonString = null!;
    private IOps _ops = null!;

    public static IEnumerable<string> WorkloadIds
        => MinimalFactorialConfigs.GetAll().Select(x => x.Id);

    [GlobalSetup]
    public void Setup()
    {
        var path = SerializationHelper.TestDataFile("MinimalFactorial", $"{WorkloadId}.json");
        _jsonBytes = File.ReadAllBytes(path);
        _jsonString = Encoding.UTF8.GetString(_jsonBytes);

        _ops = WorkloadId switch
        {
            "D2-W10-T" => new Ops<D2_W10_T>(),
            "D2-W10-N" => new Ops<D2_W10_N>(),
            "D2-W10-B" => new Ops<D2_W10_B>(),
            "D2-W100-T" => new Ops<D2_W100_T>(),
            "D2-W100-N" => new Ops<D2_W100_N>(),
            "D2-W100-B" => new Ops<D2_W100_B>(),
            "D10-W10-T" => new Ops<D10_W10_T>(),
            "D10-W10-N" => new Ops<D10_W10_N>(),
            "D10-W10-B" => new Ops<D10_W10_B>(),
            "D10-W100-T" => new Ops<D10_W100_T>(),
            "D10-W100-N" => new Ops<D10_W100_N>(),
            "D10-W100-B" => new Ops<D10_W100_B>(),
            _ => throw new ArgumentException($"Unknown workload: {WorkloadId}")
        };

        _ops.PreDeserialize(_jsonBytes);
    }

    // --- Deserialization ---

    [Benchmark, BenchmarkCategory("Deserialize")]
    public object STJ_Deserialize() => _ops.StjDeserialize(_jsonBytes);

    [Benchmark, BenchmarkCategory("Deserialize")]
    public object Newtonsoft_Deserialize() => _ops.NewtonsoftDeserialize(_jsonString);

    [Benchmark, BenchmarkCategory("Deserialize")]
    public object SpanJson_Deserialize() => _ops.SpanJsonDeserialize(_jsonBytes);

    [Benchmark, BenchmarkCategory("Deserialize")]
    public object Utf8Json_Deserialize() => _ops.Utf8JsonDeserialize(_jsonBytes);

    [Benchmark, BenchmarkCategory("Deserialize")]
    public object Jil_Deserialize() => _ops.JilDeserialize(_jsonString);

    // --- Serialization ---

    [Benchmark, BenchmarkCategory("Serialize")]
    public string STJ_Serialize() => _ops.StjSerialize();

    [Benchmark, BenchmarkCategory("Serialize")]
    public string Newtonsoft_Serialize() => _ops.NewtonsoftSerialize();

    [Benchmark, BenchmarkCategory("Serialize")]
    public byte[] SpanJson_Serialize() => _ops.SpanJsonSerialize();

    [Benchmark, BenchmarkCategory("Serialize")]
    public byte[] Utf8Json_Serialize() => _ops.Utf8JsonSerialize();

    [Benchmark, BenchmarkCategory("Serialize")]
    public string Jil_Serialize() => _ops.JilSerialize();

    // --- Type dispatch helper ---

    private interface IOps
    {
        void PreDeserialize(byte[] bytes);
        object StjDeserialize(byte[] bytes);
        object NewtonsoftDeserialize(string json);
        object SpanJsonDeserialize(byte[] bytes);
        object Utf8JsonDeserialize(byte[] bytes);
        object JilDeserialize(string json);
        string StjSerialize();
        string NewtonsoftSerialize();
        byte[] SpanJsonSerialize();
        byte[] Utf8JsonSerialize();
        string JilSerialize();
    }

    private class Ops<T> : IOps where T : class
    {
        private T _obj = null!;

        public void PreDeserialize(byte[] bytes)
            => _obj = JsonSerializer.Deserialize<T>(bytes)!;

        public object StjDeserialize(byte[] bytes)
            => JsonSerializer.Deserialize<T>(bytes)!;
        public object NewtonsoftDeserialize(string json)
            => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json)!;
        public object SpanJsonDeserialize(byte[] bytes)
            => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<T>(bytes)!;
        public object Utf8JsonDeserialize(byte[] bytes)
            => Utf8Json.JsonSerializer.Deserialize<T>(bytes)!;
        public object JilDeserialize(string json)
            => Jil.JSON.Deserialize<T>(json)!;

        public string StjSerialize()
            => JsonSerializer.Serialize(_obj);
        public string NewtonsoftSerialize()
            => Newtonsoft.Json.JsonConvert.SerializeObject(_obj);
        public byte[] SpanJsonSerialize()
            => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_obj);
        public byte[] Utf8JsonSerialize()
            => Utf8Json.JsonSerializer.Serialize(_obj);
        public string JilSerialize()
            => Jil.JSON.Serialize(_obj);
    }
}
