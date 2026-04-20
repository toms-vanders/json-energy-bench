using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks;

/// <summary>
/// Quick smoke benchmark (byte[] I/O): baseline config (D5, W20, Textual, Object-only, ASCII, R0).
/// 5 libraries × 2 operations = 10 methods. UTF-8 bytes; UTF-16 native libs (Newtonsoft) include encoding conversion cost.
/// </summary>
[Config(typeof(BenchConfig))]
public class SmokeBenchByte
{
    private byte[] _bytes = null!;
    private Node20<string> _obj = null!;

    [GlobalSetup]
    public void Setup()
    {
        var path = SerializationHelper.TestDataFile("Smoke", "Smoke.json");
        _bytes = File.ReadAllBytes(path);
        _obj = JsonSerializer.Deserialize<Node20<string>>(_bytes)!;
    }

    // ===================== Deserialize =====================

    [Benchmark, BenchmarkCategory("Deserialize")]
    public Node20<string> STJRefGen_Deser() => JsonSerializer.Deserialize<Node20<string>>(_bytes)!;
    [Benchmark, BenchmarkCategory("Deserialize")]
    public Node20<string> STJSrcGen_Deser() => JsonSerializer.Deserialize(_bytes, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize")]
    public Node20<string> Newtonsoft_Deser() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_bytes))!;
    [Benchmark, BenchmarkCategory("Deserialize")]
    public Node20<string> SpanJson_Deser() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_bytes)!;
    [Benchmark, BenchmarkCategory("Deserialize")]
    public Node20<string> Utf8Json_Deser() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_bytes)!;

    // ===================== Serialize =====================

    [Benchmark, BenchmarkCategory("Serialize")]
    public byte[] STJRefGen_Ser() => JsonSerializer.SerializeToUtf8Bytes(_obj);
    [Benchmark, BenchmarkCategory("Serialize")]
    public byte[] STJSrcGen_Ser() => JsonSerializer.SerializeToUtf8Bytes(_obj, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize")]
    public byte[] Newtonsoft_Ser() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_obj));
    [Benchmark, BenchmarkCategory("Serialize")]
    public byte[] SpanJson_Ser() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_obj);
    [Benchmark, BenchmarkCategory("Serialize")]
    public byte[] Utf8Json_Ser() => Utf8Json.JsonSerializer.Serialize(_obj);
}
