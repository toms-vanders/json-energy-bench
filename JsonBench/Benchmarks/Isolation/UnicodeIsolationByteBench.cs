using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Unicode isolation benchmark: varies literal Unicode density (5 levels), byte[] I/O.
/// Baseline: D5, W20, Textual, Object-only, ASCII, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class UnicodeIsolationByteBench
{
    private byte[] _u0_b = null!; private Node20<string> _u0 = null!;
    private byte[] _u10_b = null!; private Node20<string> _u10 = null!;
    private byte[] _u30_b = null!; private Node20<string> _u30 = null!;
    private byte[] _u50_b = null!; private Node20<string> _u50 = null!;
    private byte[] _u70_b = null!; private Node20<string> _u70 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _u0_b = Load("U0"); _u0 = JsonSerializer.Deserialize<Node20<string>>(_u0_b)!;
        _u10_b = Load("U10"); _u10 = JsonSerializer.Deserialize<Node20<string>>(_u10_b)!;
        _u30_b = Load("U30"); _u30 = JsonSerializer.Deserialize<Node20<string>>(_u30_b)!;
        _u50_b = Load("U50"); _u50 = JsonSerializer.Deserialize<Node20<string>>(_u50_b)!;
        _u70_b = Load("U70"); _u70 = JsonSerializer.Deserialize<Node20<string>>(_u70_b)!;
    }

    private static byte[] Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoUnicode", $"{id}.json");
        return File.ReadAllBytes(path);
    }

    // ===================== U0 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U0")]
    public Node20<string> STJ_Deser_U0() => JsonSerializer.Deserialize<Node20<string>>(_u0_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U0")]
    public Node20<string> Newtonsoft_Deser_U0() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_u0_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-U0")]
    public Node20<string> SpanJson_Deser_U0() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_u0_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U0")]
    public Node20<string> Utf8Json_Deser_U0() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_u0_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U0")]
    public Node20<string> Jil_Deser_U0() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_u0_b))!;

    [Benchmark, BenchmarkCategory("Serialize-U0")]
    public byte[] STJ_Ser_U0() => JsonSerializer.SerializeToUtf8Bytes(_u0);
    [Benchmark, BenchmarkCategory("Serialize-U0")]
    public byte[] Newtonsoft_Ser_U0() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_u0));
    [Benchmark, BenchmarkCategory("Serialize-U0")]
    public byte[] SpanJson_Ser_U0() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_u0);
    [Benchmark, BenchmarkCategory("Serialize-U0")]
    public byte[] Utf8Json_Ser_U0() => Utf8Json.JsonSerializer.Serialize(_u0);
    [Benchmark, BenchmarkCategory("Serialize-U0")]
    public byte[] Jil_Ser_U0() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_u0));

    // ===================== U10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> STJ_Deser_U10() => JsonSerializer.Deserialize<Node20<string>>(_u10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> Newtonsoft_Deser_U10() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_u10_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> SpanJson_Deser_U10() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_u10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> Utf8Json_Deser_U10() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_u10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> Jil_Deser_U10() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_u10_b))!;

    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public byte[] STJ_Ser_U10() => JsonSerializer.SerializeToUtf8Bytes(_u10);
    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public byte[] Newtonsoft_Ser_U10() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_u10));
    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public byte[] SpanJson_Ser_U10() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_u10);
    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public byte[] Utf8Json_Ser_U10() => Utf8Json.JsonSerializer.Serialize(_u10);
    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public byte[] Jil_Ser_U10() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_u10));

    // ===================== U30 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U30")]
    public Node20<string> STJ_Deser_U30() => JsonSerializer.Deserialize<Node20<string>>(_u30_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U30")]
    public Node20<string> Newtonsoft_Deser_U30() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_u30_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-U30")]
    public Node20<string> SpanJson_Deser_U30() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_u30_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U30")]
    public Node20<string> Utf8Json_Deser_U30() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_u30_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U30")]
    public Node20<string> Jil_Deser_U30() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_u30_b))!;

    [Benchmark, BenchmarkCategory("Serialize-U30")]
    public byte[] STJ_Ser_U30() => JsonSerializer.SerializeToUtf8Bytes(_u30);
    [Benchmark, BenchmarkCategory("Serialize-U30")]
    public byte[] Newtonsoft_Ser_U30() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_u30));
    [Benchmark, BenchmarkCategory("Serialize-U30")]
    public byte[] SpanJson_Ser_U30() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_u30);
    [Benchmark, BenchmarkCategory("Serialize-U30")]
    public byte[] Utf8Json_Ser_U30() => Utf8Json.JsonSerializer.Serialize(_u30);
    [Benchmark, BenchmarkCategory("Serialize-U30")]
    public byte[] Jil_Ser_U30() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_u30));

    // ===================== U50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> STJ_Deser_U50() => JsonSerializer.Deserialize<Node20<string>>(_u50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> Newtonsoft_Deser_U50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_u50_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> SpanJson_Deser_U50() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_u50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> Utf8Json_Deser_U50() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_u50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> Jil_Deser_U50() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_u50_b))!;

    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public byte[] STJ_Ser_U50() => JsonSerializer.SerializeToUtf8Bytes(_u50);
    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public byte[] Newtonsoft_Ser_U50() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_u50));
    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public byte[] SpanJson_Ser_U50() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_u50);
    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public byte[] Utf8Json_Ser_U50() => Utf8Json.JsonSerializer.Serialize(_u50);
    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public byte[] Jil_Ser_U50() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_u50));

    // ===================== U70 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U70")]
    public Node20<string> STJ_Deser_U70() => JsonSerializer.Deserialize<Node20<string>>(_u70_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U70")]
    public Node20<string> Newtonsoft_Deser_U70() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_u70_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-U70")]
    public Node20<string> SpanJson_Deser_U70() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_u70_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U70")]
    public Node20<string> Utf8Json_Deser_U70() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_u70_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U70")]
    public Node20<string> Jil_Deser_U70() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_u70_b))!;

    [Benchmark, BenchmarkCategory("Serialize-U70")]
    public byte[] STJ_Ser_U70() => JsonSerializer.SerializeToUtf8Bytes(_u70);
    [Benchmark, BenchmarkCategory("Serialize-U70")]
    public byte[] Newtonsoft_Ser_U70() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_u70));
    [Benchmark, BenchmarkCategory("Serialize-U70")]
    public byte[] SpanJson_Ser_U70() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_u70);
    [Benchmark, BenchmarkCategory("Serialize-U70")]
    public byte[] Utf8Json_Ser_U70() => Utf8Json.JsonSerializer.Serialize(_u70);
    [Benchmark, BenchmarkCategory("Serialize-U70")]
    public byte[] Jil_Ser_U70() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_u70));
}
