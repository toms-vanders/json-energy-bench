using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Redundancy isolation benchmark: varies duplicate value ratio (5 levels), byte[] I/O.
/// Baseline: D5, W20, Textual, Object-only, ASCII
/// </summary>
[Config(typeof(BenchConfig))]
public class RedundancyIsolationByteBench
{
    private byte[] _r0_b = null!; private Node20<string> _r0 = null!;
    private byte[] _r25_b = null!; private Node20<string> _r25 = null!;
    private byte[] _r50_b = null!; private Node20<string> _r50 = null!;
    private byte[] _r75_b = null!; private Node20<string> _r75 = null!;
    private byte[] _r95_b = null!; private Node20<string> _r95 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _r0_b = Load("R0"); _r0 = JsonSerializer.Deserialize<Node20<string>>(_r0_b)!;
        _r25_b = Load("R25"); _r25 = JsonSerializer.Deserialize<Node20<string>>(_r25_b)!;
        _r50_b = Load("R50"); _r50 = JsonSerializer.Deserialize<Node20<string>>(_r50_b)!;
        _r75_b = Load("R75"); _r75 = JsonSerializer.Deserialize<Node20<string>>(_r75_b)!;
        _r95_b = Load("R95"); _r95 = JsonSerializer.Deserialize<Node20<string>>(_r95_b)!;
    }

    private static byte[] Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoRedundancy", $"{id}.json");
        return File.ReadAllBytes(path);
    }

    // ===================== R0 =====================

    [Benchmark, BenchmarkCategory("Deserialize-R0")]
    public Node20<string> STJ_Deser_R0() => JsonSerializer.Deserialize<Node20<string>>(_r0_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R0")]
    public Node20<string> Newtonsoft_Deser_R0() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_r0_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-R0")]
    public Node20<string> SpanJson_Deser_R0() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_r0_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R0")]
    public Node20<string> Utf8Json_Deser_R0() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_r0_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R0")]
    public Node20<string> Jil_Deser_R0() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_r0_b))!;

    [Benchmark, BenchmarkCategory("Serialize-R0")]
    public byte[] STJ_Ser_R0() => JsonSerializer.SerializeToUtf8Bytes(_r0);
    [Benchmark, BenchmarkCategory("Serialize-R0")]
    public byte[] Newtonsoft_Ser_R0() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_r0));
    [Benchmark, BenchmarkCategory("Serialize-R0")]
    public byte[] SpanJson_Ser_R0() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_r0);
    [Benchmark, BenchmarkCategory("Serialize-R0")]
    public byte[] Utf8Json_Ser_R0() => Utf8Json.JsonSerializer.Serialize(_r0);
    [Benchmark, BenchmarkCategory("Serialize-R0")]
    public byte[] Jil_Ser_R0() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_r0));

    // ===================== R25 =====================

    [Benchmark, BenchmarkCategory("Deserialize-R25")]
    public Node20<string> STJ_Deser_R25() => JsonSerializer.Deserialize<Node20<string>>(_r25_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R25")]
    public Node20<string> Newtonsoft_Deser_R25() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_r25_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-R25")]
    public Node20<string> SpanJson_Deser_R25() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_r25_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R25")]
    public Node20<string> Utf8Json_Deser_R25() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_r25_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R25")]
    public Node20<string> Jil_Deser_R25() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_r25_b))!;

    [Benchmark, BenchmarkCategory("Serialize-R25")]
    public byte[] STJ_Ser_R25() => JsonSerializer.SerializeToUtf8Bytes(_r25);
    [Benchmark, BenchmarkCategory("Serialize-R25")]
    public byte[] Newtonsoft_Ser_R25() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_r25));
    [Benchmark, BenchmarkCategory("Serialize-R25")]
    public byte[] SpanJson_Ser_R25() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_r25);
    [Benchmark, BenchmarkCategory("Serialize-R25")]
    public byte[] Utf8Json_Ser_R25() => Utf8Json.JsonSerializer.Serialize(_r25);
    [Benchmark, BenchmarkCategory("Serialize-R25")]
    public byte[] Jil_Ser_R25() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_r25));

    // ===================== R50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-R50")]
    public Node20<string> STJ_Deser_R50() => JsonSerializer.Deserialize<Node20<string>>(_r50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R50")]
    public Node20<string> Newtonsoft_Deser_R50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_r50_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-R50")]
    public Node20<string> SpanJson_Deser_R50() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_r50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R50")]
    public Node20<string> Utf8Json_Deser_R50() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_r50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R50")]
    public Node20<string> Jil_Deser_R50() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_r50_b))!;

    [Benchmark, BenchmarkCategory("Serialize-R50")]
    public byte[] STJ_Ser_R50() => JsonSerializer.SerializeToUtf8Bytes(_r50);
    [Benchmark, BenchmarkCategory("Serialize-R50")]
    public byte[] Newtonsoft_Ser_R50() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_r50));
    [Benchmark, BenchmarkCategory("Serialize-R50")]
    public byte[] SpanJson_Ser_R50() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_r50);
    [Benchmark, BenchmarkCategory("Serialize-R50")]
    public byte[] Utf8Json_Ser_R50() => Utf8Json.JsonSerializer.Serialize(_r50);
    [Benchmark, BenchmarkCategory("Serialize-R50")]
    public byte[] Jil_Ser_R50() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_r50));

    // ===================== R75 =====================

    [Benchmark, BenchmarkCategory("Deserialize-R75")]
    public Node20<string> STJ_Deser_R75() => JsonSerializer.Deserialize<Node20<string>>(_r75_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R75")]
    public Node20<string> Newtonsoft_Deser_R75() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_r75_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-R75")]
    public Node20<string> SpanJson_Deser_R75() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_r75_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R75")]
    public Node20<string> Utf8Json_Deser_R75() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_r75_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R75")]
    public Node20<string> Jil_Deser_R75() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_r75_b))!;

    [Benchmark, BenchmarkCategory("Serialize-R75")]
    public byte[] STJ_Ser_R75() => JsonSerializer.SerializeToUtf8Bytes(_r75);
    [Benchmark, BenchmarkCategory("Serialize-R75")]
    public byte[] Newtonsoft_Ser_R75() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_r75));
    [Benchmark, BenchmarkCategory("Serialize-R75")]
    public byte[] SpanJson_Ser_R75() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_r75);
    [Benchmark, BenchmarkCategory("Serialize-R75")]
    public byte[] Utf8Json_Ser_R75() => Utf8Json.JsonSerializer.Serialize(_r75);
    [Benchmark, BenchmarkCategory("Serialize-R75")]
    public byte[] Jil_Ser_R75() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_r75));

    // ===================== R95 =====================

    [Benchmark, BenchmarkCategory("Deserialize-R95")]
    public Node20<string> STJ_Deser_R95() => JsonSerializer.Deserialize<Node20<string>>(_r95_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R95")]
    public Node20<string> Newtonsoft_Deser_R95() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_r95_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-R95")]
    public Node20<string> SpanJson_Deser_R95() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_r95_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R95")]
    public Node20<string> Utf8Json_Deser_R95() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_r95_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-R95")]
    public Node20<string> Jil_Deser_R95() => Jil.JSON.Deserialize<Node20<string>>(Encoding.UTF8.GetString(_r95_b))!;

    [Benchmark, BenchmarkCategory("Serialize-R95")]
    public byte[] STJ_Ser_R95() => JsonSerializer.SerializeToUtf8Bytes(_r95);
    [Benchmark, BenchmarkCategory("Serialize-R95")]
    public byte[] Newtonsoft_Ser_R95() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_r95));
    [Benchmark, BenchmarkCategory("Serialize-R95")]
    public byte[] SpanJson_Ser_R95() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_r95);
    [Benchmark, BenchmarkCategory("Serialize-R95")]
    public byte[] Utf8Json_Ser_R95() => Utf8Json.JsonSerializer.Serialize(_r95);
    [Benchmark, BenchmarkCategory("Serialize-R95")]
    public byte[] Jil_Ser_R95() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_r95));
}
