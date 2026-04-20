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
public class SmokeBenchString
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
    public Node20<string> STJRefGen_Deser() => JsonSerializer.Deserialize<Node20<string>>(_json)!;
    [Benchmark, BenchmarkCategory("Deserialize")]
    public Node20<string> STJSrcGen_Deser() => JsonSerializer.Deserialize(_json, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize")]
    public Node20<string> Newtonsoft_Deser() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_json)!;
    [Benchmark, BenchmarkCategory("Deserialize")]
    public Node20<string> SpanJson_Deser() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_json)!;
    [Benchmark, BenchmarkCategory("Deserialize")]
    public Node20<string> Utf8Json_Deser() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_json)!;

    // ===================== Serialize =====================

    [Benchmark, BenchmarkCategory("Serialize")]
    public string STJRefGen_Ser() => JsonSerializer.Serialize(_obj);
    [Benchmark, BenchmarkCategory("Serialize")]
    public string STJSrcGen_Ser() => JsonSerializer.Serialize(_obj, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize")]
    public string Newtonsoft_Ser() => Newtonsoft.Json.JsonConvert.SerializeObject(_obj);
    [Benchmark, BenchmarkCategory("Serialize")]
    public string SpanJson_Ser() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_obj);
    [Benchmark, BenchmarkCategory("Serialize")]
    public string Utf8Json_Ser() => Utf8Json.JsonSerializer.ToJsonString(_obj);
}
