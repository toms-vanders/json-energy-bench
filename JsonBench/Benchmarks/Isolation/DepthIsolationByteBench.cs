using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Depth isolation benchmark: varies depth (7 levels), byte[] I/O.
/// Baseline: W20, Textual, Object-only, ASCII, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class DepthIsolationByteBench
{
    private byte[] _d1_b = null!; private Node20<string> _d1 = null!;
    private byte[] _d2_b = null!; private Node20<string> _d2 = null!;
    private byte[] _d4_b = null!; private Node20<string> _d4 = null!;
    private byte[] _d8_b = null!; private Node20<string> _d8 = null!;
    private byte[] _d15_b = null!; private Node20<string> _d15 = null!;
    private byte[] _d25_b = null!; private Node20<string> _d25 = null!;
    private byte[] _d40_b = null!; private Node20<string> _d40 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _d1_b = Load("D1"); _d1 = JsonSerializer.Deserialize<Node20<string>>(_d1_b)!;
        _d2_b = Load("D2"); _d2 = JsonSerializer.Deserialize<Node20<string>>(_d2_b)!;
        _d4_b = Load("D4"); _d4 = JsonSerializer.Deserialize<Node20<string>>(_d4_b)!;
        _d8_b = Load("D8"); _d8 = JsonSerializer.Deserialize<Node20<string>>(_d8_b)!;
        _d15_b = Load("D15"); _d15 = JsonSerializer.Deserialize<Node20<string>>(_d15_b)!;
        _d25_b = Load("D25"); _d25 = JsonSerializer.Deserialize<Node20<string>>(_d25_b)!;
        _d40_b = Load("D40"); _d40 = JsonSerializer.Deserialize<Node20<string>>(_d40_b)!;
    }

    private static byte[] Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoDepth", $"{id}.json");
        return File.ReadAllBytes(path);
    }

    // ===================== D1 =====================

    [Benchmark, BenchmarkCategory("Deserialize-D1")]
    public Node20<string> STJRefGen_Deser_D1() => JsonSerializer.Deserialize<Node20<string>>(_d1_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-D1")]
    public Node20<string> STJSrcGen_Deser_D1() => JsonSerializer.Deserialize(_d1_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D1")]
    public Node20<string> Newtonsoft_Deser_D1() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_d1_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-D1")]
    public Node20<string> SpanJson_Deser_D1() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_d1_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-D1")]
    public Node20<string> Utf8Json_Deser_D1() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_d1_b)!;

    [Benchmark, BenchmarkCategory("Serialize-D1")]
    public byte[] STJRefGen_Ser_D1() => JsonSerializer.SerializeToUtf8Bytes(_d1);
    [Benchmark, BenchmarkCategory("Serialize-D1")]
    public byte[] STJSrcGen_Ser_D1() => JsonSerializer.SerializeToUtf8Bytes(_d1, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-D1")]
    public byte[] Newtonsoft_Ser_D1() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_d1));
    [Benchmark, BenchmarkCategory("Serialize-D1")]
    public byte[] SpanJson_Ser_D1() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_d1);
    [Benchmark, BenchmarkCategory("Serialize-D1")]
    public byte[] Utf8Json_Ser_D1() => Utf8Json.JsonSerializer.Serialize(_d1);

    // ===================== D2 =====================

    [Benchmark, BenchmarkCategory("Deserialize-D2")]
    public Node20<string> STJRefGen_Deser_D2() => JsonSerializer.Deserialize<Node20<string>>(_d2_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2")]
    public Node20<string> STJSrcGen_Deser_D2() => JsonSerializer.Deserialize(_d2_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2")]
    public Node20<string> Newtonsoft_Deser_D2() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_d2_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-D2")]
    public Node20<string> SpanJson_Deser_D2() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_d2_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2")]
    public Node20<string> Utf8Json_Deser_D2() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_d2_b)!;

    [Benchmark, BenchmarkCategory("Serialize-D2")]
    public byte[] STJRefGen_Ser_D2() => JsonSerializer.SerializeToUtf8Bytes(_d2);
    [Benchmark, BenchmarkCategory("Serialize-D2")]
    public byte[] STJSrcGen_Ser_D2() => JsonSerializer.SerializeToUtf8Bytes(_d2, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-D2")]
    public byte[] Newtonsoft_Ser_D2() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_d2));
    [Benchmark, BenchmarkCategory("Serialize-D2")]
    public byte[] SpanJson_Ser_D2() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_d2);
    [Benchmark, BenchmarkCategory("Serialize-D2")]
    public byte[] Utf8Json_Ser_D2() => Utf8Json.JsonSerializer.Serialize(_d2);

    // ===================== D4 =====================

    [Benchmark, BenchmarkCategory("Deserialize-D4")]
    public Node20<string> STJRefGen_Deser_D4() => JsonSerializer.Deserialize<Node20<string>>(_d4_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-D4")]
    public Node20<string> STJSrcGen_Deser_D4() => JsonSerializer.Deserialize(_d4_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D4")]
    public Node20<string> Newtonsoft_Deser_D4() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_d4_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-D4")]
    public Node20<string> SpanJson_Deser_D4() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_d4_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-D4")]
    public Node20<string> Utf8Json_Deser_D4() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_d4_b)!;

    [Benchmark, BenchmarkCategory("Serialize-D4")]
    public byte[] STJRefGen_Ser_D4() => JsonSerializer.SerializeToUtf8Bytes(_d4);
    [Benchmark, BenchmarkCategory("Serialize-D4")]
    public byte[] STJSrcGen_Ser_D4() => JsonSerializer.SerializeToUtf8Bytes(_d4, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-D4")]
    public byte[] Newtonsoft_Ser_D4() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_d4));
    [Benchmark, BenchmarkCategory("Serialize-D4")]
    public byte[] SpanJson_Ser_D4() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_d4);
    [Benchmark, BenchmarkCategory("Serialize-D4")]
    public byte[] Utf8Json_Ser_D4() => Utf8Json.JsonSerializer.Serialize(_d4);

    // ===================== D8 =====================

    [Benchmark, BenchmarkCategory("Deserialize-D8")]
    public Node20<string> STJRefGen_Deser_D8() => JsonSerializer.Deserialize<Node20<string>>(_d8_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-D8")]
    public Node20<string> STJSrcGen_Deser_D8() => JsonSerializer.Deserialize(_d8_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D8")]
    public Node20<string> Newtonsoft_Deser_D8() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_d8_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-D8")]
    public Node20<string> SpanJson_Deser_D8() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_d8_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-D8")]
    public Node20<string> Utf8Json_Deser_D8() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_d8_b)!;

    [Benchmark, BenchmarkCategory("Serialize-D8")]
    public byte[] STJRefGen_Ser_D8() => JsonSerializer.SerializeToUtf8Bytes(_d8);
    [Benchmark, BenchmarkCategory("Serialize-D8")]
    public byte[] STJSrcGen_Ser_D8() => JsonSerializer.SerializeToUtf8Bytes(_d8, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-D8")]
    public byte[] Newtonsoft_Ser_D8() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_d8));
    [Benchmark, BenchmarkCategory("Serialize-D8")]
    public byte[] SpanJson_Ser_D8() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_d8);
    [Benchmark, BenchmarkCategory("Serialize-D8")]
    public byte[] Utf8Json_Ser_D8() => Utf8Json.JsonSerializer.Serialize(_d8);

    // ===================== D15 =====================

    [Benchmark, BenchmarkCategory("Deserialize-D15")]
    public Node20<string> STJRefGen_Deser_D15() => JsonSerializer.Deserialize<Node20<string>>(_d15_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-D15")]
    public Node20<string> STJSrcGen_Deser_D15() => JsonSerializer.Deserialize(_d15_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D15")]
    public Node20<string> Newtonsoft_Deser_D15() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_d15_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-D15")]
    public Node20<string> SpanJson_Deser_D15() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_d15_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-D15")]
    public Node20<string> Utf8Json_Deser_D15() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_d15_b)!;

    [Benchmark, BenchmarkCategory("Serialize-D15")]
    public byte[] STJRefGen_Ser_D15() => JsonSerializer.SerializeToUtf8Bytes(_d15);
    [Benchmark, BenchmarkCategory("Serialize-D15")]
    public byte[] STJSrcGen_Ser_D15() => JsonSerializer.SerializeToUtf8Bytes(_d15, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-D15")]
    public byte[] Newtonsoft_Ser_D15() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_d15));
    [Benchmark, BenchmarkCategory("Serialize-D15")]
    public byte[] SpanJson_Ser_D15() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_d15);
    [Benchmark, BenchmarkCategory("Serialize-D15")]
    public byte[] Utf8Json_Ser_D15() => Utf8Json.JsonSerializer.Serialize(_d15);

    // ===================== D25 =====================

    [Benchmark, BenchmarkCategory("Deserialize-D25")]
    public Node20<string> STJRefGen_Deser_D25() => JsonSerializer.Deserialize<Node20<string>>(_d25_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-D25")]
    public Node20<string> STJSrcGen_Deser_D25() => JsonSerializer.Deserialize(_d25_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D25")]
    public Node20<string> Newtonsoft_Deser_D25() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_d25_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-D25")]
    public Node20<string> SpanJson_Deser_D25() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_d25_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-D25")]
    public Node20<string> Utf8Json_Deser_D25() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_d25_b)!;

    [Benchmark, BenchmarkCategory("Serialize-D25")]
    public byte[] STJRefGen_Ser_D25() => JsonSerializer.SerializeToUtf8Bytes(_d25);
    [Benchmark, BenchmarkCategory("Serialize-D25")]
    public byte[] STJSrcGen_Ser_D25() => JsonSerializer.SerializeToUtf8Bytes(_d25, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-D25")]
    public byte[] Newtonsoft_Ser_D25() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_d25));
    [Benchmark, BenchmarkCategory("Serialize-D25")]
    public byte[] SpanJson_Ser_D25() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_d25);
    [Benchmark, BenchmarkCategory("Serialize-D25")]
    public byte[] Utf8Json_Ser_D25() => Utf8Json.JsonSerializer.Serialize(_d25);

    // ===================== D40 =====================

    [Benchmark, BenchmarkCategory("Deserialize-D40")]
    public Node20<string> STJRefGen_Deser_D40() => JsonSerializer.Deserialize<Node20<string>>(_d40_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-D40")]
    public Node20<string> STJSrcGen_Deser_D40() => JsonSerializer.Deserialize(_d40_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D40")]
    public Node20<string> Newtonsoft_Deser_D40() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_d40_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-D40")]
    public Node20<string> SpanJson_Deser_D40() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_d40_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-D40")]
    public Node20<string> Utf8Json_Deser_D40() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_d40_b)!;

    [Benchmark, BenchmarkCategory("Serialize-D40")]
    public byte[] STJRefGen_Ser_D40() => JsonSerializer.SerializeToUtf8Bytes(_d40);
    [Benchmark, BenchmarkCategory("Serialize-D40")]
    public byte[] STJSrcGen_Ser_D40() => JsonSerializer.SerializeToUtf8Bytes(_d40, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-D40")]
    public byte[] Newtonsoft_Ser_D40() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_d40));
    [Benchmark, BenchmarkCategory("Serialize-D40")]
    public byte[] SpanJson_Ser_D40() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_d40);
    [Benchmark, BenchmarkCategory("Serialize-D40")]
    public byte[] Utf8Json_Ser_D40() => Utf8Json.JsonSerializer.Serialize(_d40);
}
