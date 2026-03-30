using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks;

/// <summary>
/// Quick smoke benchmark: baseline config (D5, W20, Textual, Object-only, ASCII, R0).
/// 5 libraries × 2 operations = 10 methods. String I/O only.
/// </summary>
[Config(typeof(BenchConfig))]
public class SmokeBench
{
    private string _json = null!;
    private Node20<string> _obj = null!;

    [GlobalSetup]
    public void Setup()
    {
        var path = SerializationHelper.TestDataFile("Smoke", "Smoke.json");
        _json = File.ReadAllText(path);
        _obj = JsonSerializer.Deserialize<Node20<string>>(_json)!;
    }

    // ===================== Deserialize =====================

    [Benchmark, BenchmarkCategory("Deserialize")]
    public Node20<string> STJ_Deser() => JsonSerializer.Deserialize<Node20<string>>(_json)!;
    [Benchmark, BenchmarkCategory("Deserialize")]
    public Node20<string> Newtonsoft_Deser() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_json)!;
    [Benchmark, BenchmarkCategory("Deserialize")]
    public Node20<string> SpanJson_Deser() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_json)!;
    [Benchmark, BenchmarkCategory("Deserialize")]
    public Node20<string> Utf8Json_Deser() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_json)!;
    [Benchmark, BenchmarkCategory("Deserialize")]
    public Node20<string> Jil_Deser() => Jil.JSON.Deserialize<Node20<string>>(_json)!;

    // ===================== Serialize =====================

    [Benchmark, BenchmarkCategory("Serialize")]
    public string STJ_Ser() => JsonSerializer.Serialize(_obj);
    [Benchmark, BenchmarkCategory("Serialize")]
    public string Newtonsoft_Ser() => Newtonsoft.Json.JsonConvert.SerializeObject(_obj);
    [Benchmark, BenchmarkCategory("Serialize")]
    public string SpanJson_Ser() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_obj);
    [Benchmark, BenchmarkCategory("Serialize")]
    public string Utf8Json_Ser() => Utf8Json.JsonSerializer.ToJsonString(_obj);
    [Benchmark, BenchmarkCategory("Serialize")]
    public string Jil_Ser() => Jil.JSON.Serialize(_obj);
}
