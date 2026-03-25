using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Escape isolation benchmark: varies simple escape density (5 levels), byte[] I/O.
/// Baseline: D5, W20, Textual, Object-only, ASCII, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class EscapeIsolationByteBench
{
    private byte[] _e0_b = null!; private Node20<string> _e0 = null!;
    private byte[] _e10_b = null!; private Node20<string> _e10 = null!;
    private byte[] _e30_b = null!; private Node20<string> _e30 = null!;
    private byte[] _e50_b = null!; private Node20<string> _e50 = null!;
    private byte[] _e70_b = null!; private Node20<string> _e70 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _e0_b = Load("E0"); _e0 = JsonSerializer.Deserialize<Node20<string>>(_e0_b)!;
        _e10_b = Load("E10"); _e10 = JsonSerializer.Deserialize<Node20<string>>(_e10_b)!;
        _e30_b = Load("E30"); _e30 = JsonSerializer.Deserialize<Node20<string>>(_e30_b)!;
        _e50_b = Load("E50"); _e50 = JsonSerializer.Deserialize<Node20<string>>(_e50_b)!;
        _e70_b = Load("E70"); _e70 = JsonSerializer.Deserialize<Node20<string>>(_e70_b)!;
    }

    private static byte[] Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoEscape", $"{id}.json");
        return File.ReadAllBytes(path);
    }

    // ===================== E0 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E0")]
    public Node20<string> STJ_Deser_E0() => JsonSerializer.Deserialize<Node20<string>>(_e0_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E0")]
    public Node20<string> Newtonsoft_Deser_E0() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_e0_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-E0")]
    public Node20<string> SpanJson_Deser_E0() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_e0_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E0")]
    public Node20<string> Utf8Json_Deser_E0() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_e0_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E0")]
    public Node20<string> Jil_Deser_E0() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_e0_b))!;

    [Benchmark, BenchmarkCategory("Serialize-E0")]
    public byte[] STJ_Ser_E0() => JsonSerializer.SerializeToUtf8Bytes(_e0);
    [Benchmark, BenchmarkCategory("Serialize-E0")]
    public byte[] Newtonsoft_Ser_E0() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_e0));
    [Benchmark, BenchmarkCategory("Serialize-E0")]
    public byte[] SpanJson_Ser_E0() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_e0);
    [Benchmark, BenchmarkCategory("Serialize-E0")]
    public byte[] Utf8Json_Ser_E0() => Utf8Json.JsonSerializer.Serialize(_e0);
    [Benchmark, BenchmarkCategory("Serialize-E0")]
    public byte[] Jil_Ser_E0() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_e0));

    // ===================== E10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> STJ_Deser_E10() => JsonSerializer.Deserialize<Node20<string>>(_e10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> Newtonsoft_Deser_E10() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_e10_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> SpanJson_Deser_E10() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_e10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> Utf8Json_Deser_E10() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_e10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> Jil_Deser_E10() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_e10_b))!;

    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public byte[] STJ_Ser_E10() => JsonSerializer.SerializeToUtf8Bytes(_e10);
    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public byte[] Newtonsoft_Ser_E10() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_e10));
    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public byte[] SpanJson_Ser_E10() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_e10);
    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public byte[] Utf8Json_Ser_E10() => Utf8Json.JsonSerializer.Serialize(_e10);
    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public byte[] Jil_Ser_E10() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_e10));

    // ===================== E30 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E30")]
    public Node20<string> STJ_Deser_E30() => JsonSerializer.Deserialize<Node20<string>>(_e30_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E30")]
    public Node20<string> Newtonsoft_Deser_E30() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_e30_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-E30")]
    public Node20<string> SpanJson_Deser_E30() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_e30_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E30")]
    public Node20<string> Utf8Json_Deser_E30() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_e30_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E30")]
    public Node20<string> Jil_Deser_E30() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_e30_b))!;

    [Benchmark, BenchmarkCategory("Serialize-E30")]
    public byte[] STJ_Ser_E30() => JsonSerializer.SerializeToUtf8Bytes(_e30);
    [Benchmark, BenchmarkCategory("Serialize-E30")]
    public byte[] Newtonsoft_Ser_E30() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_e30));
    [Benchmark, BenchmarkCategory("Serialize-E30")]
    public byte[] SpanJson_Ser_E30() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_e30);
    [Benchmark, BenchmarkCategory("Serialize-E30")]
    public byte[] Utf8Json_Ser_E30() => Utf8Json.JsonSerializer.Serialize(_e30);
    [Benchmark, BenchmarkCategory("Serialize-E30")]
    public byte[] Jil_Ser_E30() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_e30));

    // ===================== E50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> STJ_Deser_E50() => JsonSerializer.Deserialize<Node20<string>>(_e50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> Newtonsoft_Deser_E50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_e50_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> SpanJson_Deser_E50() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_e50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> Utf8Json_Deser_E50() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_e50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> Jil_Deser_E50() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_e50_b))!;

    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public byte[] STJ_Ser_E50() => JsonSerializer.SerializeToUtf8Bytes(_e50);
    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public byte[] Newtonsoft_Ser_E50() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_e50));
    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public byte[] SpanJson_Ser_E50() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_e50);
    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public byte[] Utf8Json_Ser_E50() => Utf8Json.JsonSerializer.Serialize(_e50);
    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public byte[] Jil_Ser_E50() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_e50));

    // ===================== E70 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E70")]
    public Node20<string> STJ_Deser_E70() => JsonSerializer.Deserialize<Node20<string>>(_e70_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E70")]
    public Node20<string> Newtonsoft_Deser_E70() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_e70_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-E70")]
    public Node20<string> SpanJson_Deser_E70() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_e70_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E70")]
    public Node20<string> Utf8Json_Deser_E70() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_e70_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E70")]
    public Node20<string> Jil_Deser_E70() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_e70_b))!;

    [Benchmark, BenchmarkCategory("Serialize-E70")]
    public byte[] STJ_Ser_E70() => JsonSerializer.SerializeToUtf8Bytes(_e70);
    [Benchmark, BenchmarkCategory("Serialize-E70")]
    public byte[] Newtonsoft_Ser_E70() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_e70));
    [Benchmark, BenchmarkCategory("Serialize-E70")]
    public byte[] SpanJson_Ser_E70() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_e70);
    [Benchmark, BenchmarkCategory("Serialize-E70")]
    public byte[] Utf8Json_Ser_E70() => Utf8Json.JsonSerializer.Serialize(_e70);
    [Benchmark, BenchmarkCategory("Serialize-E70")]
    public byte[] Jil_Ser_E70() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_e70));
}
