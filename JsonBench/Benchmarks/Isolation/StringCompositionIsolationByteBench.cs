using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// String composition isolation benchmark: varies special-character density at fixed
/// 20-char strings, byte[] I/O. One shared ASCII baseline (A0, density = 0) anchors
/// three variants compared at matched density: Unicode (U5..U100), Escape (E5..E100),
/// UnicodeEscape (UE5..UE100). Content is 100% string. Baseline: D5, W20, Object-only, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class StringCompositionIsolationByteBench
{
    // Shared ASCII baseline (density = 0) for all three variants
    private byte[] _a0_b = null!; private Node20<string> _a0 = null!;

    // Unicode variant
    private byte[] _u5_b = null!; private Node20<string> _u5 = null!;
    private byte[] _u10_b = null!; private Node20<string> _u10 = null!;
    private byte[] _u25_b = null!; private Node20<string> _u25 = null!;
    private byte[] _u50_b = null!; private Node20<string> _u50 = null!;
    private byte[] _u100_b = null!; private Node20<string> _u100 = null!;

    // Escape variant
    private byte[] _e5_b = null!; private Node20<string> _e5 = null!;
    private byte[] _e10_b = null!; private Node20<string> _e10 = null!;
    private byte[] _e25_b = null!; private Node20<string> _e25 = null!;
    private byte[] _e50_b = null!; private Node20<string> _e50 = null!;
    private byte[] _e100_b = null!; private Node20<string> _e100 = null!;

    // UnicodeEscape variant
    private byte[] _ue5_b = null!; private Node20<string> _ue5 = null!;
    private byte[] _ue10_b = null!; private Node20<string> _ue10 = null!;
    private byte[] _ue25_b = null!; private Node20<string> _ue25 = null!;
    private byte[] _ue50_b = null!; private Node20<string> _ue50 = null!;
    private byte[] _ue100_b = null!; private Node20<string> _ue100 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _a0_b = Load("A0"); _a0 = JsonSerializer.Deserialize<Node20<string>>(_a0_b)!;

        _u5_b = Load("U5"); _u5 = JsonSerializer.Deserialize<Node20<string>>(_u5_b)!;
        _u10_b = Load("U10"); _u10 = JsonSerializer.Deserialize<Node20<string>>(_u10_b)!;
        _u25_b = Load("U25"); _u25 = JsonSerializer.Deserialize<Node20<string>>(_u25_b)!;
        _u50_b = Load("U50"); _u50 = JsonSerializer.Deserialize<Node20<string>>(_u50_b)!;
        _u100_b = Load("U100"); _u100 = JsonSerializer.Deserialize<Node20<string>>(_u100_b)!;

        _e5_b = Load("E5"); _e5 = JsonSerializer.Deserialize<Node20<string>>(_e5_b)!;
        _e10_b = Load("E10"); _e10 = JsonSerializer.Deserialize<Node20<string>>(_e10_b)!;
        _e25_b = Load("E25"); _e25 = JsonSerializer.Deserialize<Node20<string>>(_e25_b)!;
        _e50_b = Load("E50"); _e50 = JsonSerializer.Deserialize<Node20<string>>(_e50_b)!;
        _e100_b = Load("E100"); _e100 = JsonSerializer.Deserialize<Node20<string>>(_e100_b)!;

        _ue5_b = Load("UE5"); _ue5 = JsonSerializer.Deserialize<Node20<string>>(_ue5_b)!;
        _ue10_b = Load("UE10"); _ue10 = JsonSerializer.Deserialize<Node20<string>>(_ue10_b)!;
        _ue25_b = Load("UE25"); _ue25 = JsonSerializer.Deserialize<Node20<string>>(_ue25_b)!;
        _ue50_b = Load("UE50"); _ue50 = JsonSerializer.Deserialize<Node20<string>>(_ue50_b)!;
        _ue100_b = Load("UE100"); _ue100 = JsonSerializer.Deserialize<Node20<string>>(_ue100_b)!;
    }

    private static byte[] Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoStringComp", $"{id}.json");
        return File.ReadAllBytes(path);
    }

    // ===================== A0 (shared ASCII baseline) =====================

    [Benchmark, BenchmarkCategory("Deserialize-A0")]
    public Node20<string> STJRefGen_Deser_A0() => JsonSerializer.Deserialize<Node20<string>>(_a0_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-A0")]
    public Node20<string> STJSrcGen_Deser_A0() => JsonSerializer.Deserialize(_a0_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-A0")]
    public Node20<string> Newtonsoft_Deser_A0() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_a0_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-A0")]
    public Node20<string> SpanJson_Deser_A0() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_a0_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-A0")]
    public Node20<string> Utf8Json_Deser_A0() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_a0_b)!;

    [Benchmark, BenchmarkCategory("Serialize-A0")]
    public byte[] STJRefGen_Ser_A0() => JsonSerializer.SerializeToUtf8Bytes(_a0);
    [Benchmark, BenchmarkCategory("Serialize-A0")]
    public byte[] STJSrcGen_Ser_A0() => JsonSerializer.SerializeToUtf8Bytes(_a0, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-A0")]
    public byte[] Newtonsoft_Ser_A0() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_a0));
    [Benchmark, BenchmarkCategory("Serialize-A0")]
    public byte[] SpanJson_Ser_A0() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_a0);
    [Benchmark, BenchmarkCategory("Serialize-A0")]
    public byte[] Utf8Json_Ser_A0() => Utf8Json.JsonSerializer.Serialize(_a0);

    // ===================== U5 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U5")]
    public Node20<string> STJRefGen_Deser_U5() => JsonSerializer.Deserialize<Node20<string>>(_u5_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U5")]
    public Node20<string> STJSrcGen_Deser_U5() => JsonSerializer.Deserialize(_u5_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-U5")]
    public Node20<string> Newtonsoft_Deser_U5() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_u5_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-U5")]
    public Node20<string> SpanJson_Deser_U5() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_u5_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U5")]
    public Node20<string> Utf8Json_Deser_U5() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_u5_b)!;

    [Benchmark, BenchmarkCategory("Serialize-U5")]
    public byte[] STJRefGen_Ser_U5() => JsonSerializer.SerializeToUtf8Bytes(_u5);
    [Benchmark, BenchmarkCategory("Serialize-U5")]
    public byte[] STJSrcGen_Ser_U5() => JsonSerializer.SerializeToUtf8Bytes(_u5, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-U5")]
    public byte[] Newtonsoft_Ser_U5() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_u5));
    [Benchmark, BenchmarkCategory("Serialize-U5")]
    public byte[] SpanJson_Ser_U5() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_u5);
    [Benchmark, BenchmarkCategory("Serialize-U5")]
    public byte[] Utf8Json_Ser_U5() => Utf8Json.JsonSerializer.Serialize(_u5);

    // ===================== U10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> STJRefGen_Deser_U10() => JsonSerializer.Deserialize<Node20<string>>(_u10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> STJSrcGen_Deser_U10() => JsonSerializer.Deserialize(_u10_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> Newtonsoft_Deser_U10() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_u10_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> SpanJson_Deser_U10() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_u10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> Utf8Json_Deser_U10() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_u10_b)!;

    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public byte[] STJRefGen_Ser_U10() => JsonSerializer.SerializeToUtf8Bytes(_u10);
    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public byte[] STJSrcGen_Ser_U10() => JsonSerializer.SerializeToUtf8Bytes(_u10, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public byte[] Newtonsoft_Ser_U10() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_u10));
    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public byte[] SpanJson_Ser_U10() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_u10);
    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public byte[] Utf8Json_Ser_U10() => Utf8Json.JsonSerializer.Serialize(_u10);

    // ===================== U25 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U25")]
    public Node20<string> STJRefGen_Deser_U25() => JsonSerializer.Deserialize<Node20<string>>(_u25_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U25")]
    public Node20<string> STJSrcGen_Deser_U25() => JsonSerializer.Deserialize(_u25_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-U25")]
    public Node20<string> Newtonsoft_Deser_U25() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_u25_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-U25")]
    public Node20<string> SpanJson_Deser_U25() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_u25_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U25")]
    public Node20<string> Utf8Json_Deser_U25() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_u25_b)!;

    [Benchmark, BenchmarkCategory("Serialize-U25")]
    public byte[] STJRefGen_Ser_U25() => JsonSerializer.SerializeToUtf8Bytes(_u25);
    [Benchmark, BenchmarkCategory("Serialize-U25")]
    public byte[] STJSrcGen_Ser_U25() => JsonSerializer.SerializeToUtf8Bytes(_u25, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-U25")]
    public byte[] Newtonsoft_Ser_U25() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_u25));
    [Benchmark, BenchmarkCategory("Serialize-U25")]
    public byte[] SpanJson_Ser_U25() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_u25);
    [Benchmark, BenchmarkCategory("Serialize-U25")]
    public byte[] Utf8Json_Ser_U25() => Utf8Json.JsonSerializer.Serialize(_u25);

    // ===================== U50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> STJRefGen_Deser_U50() => JsonSerializer.Deserialize<Node20<string>>(_u50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> STJSrcGen_Deser_U50() => JsonSerializer.Deserialize(_u50_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> Newtonsoft_Deser_U50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_u50_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> SpanJson_Deser_U50() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_u50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> Utf8Json_Deser_U50() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_u50_b)!;

    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public byte[] STJRefGen_Ser_U50() => JsonSerializer.SerializeToUtf8Bytes(_u50);
    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public byte[] STJSrcGen_Ser_U50() => JsonSerializer.SerializeToUtf8Bytes(_u50, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public byte[] Newtonsoft_Ser_U50() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_u50));
    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public byte[] SpanJson_Ser_U50() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_u50);
    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public byte[] Utf8Json_Ser_U50() => Utf8Json.JsonSerializer.Serialize(_u50);

    // ===================== U100 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U100")]
    public Node20<string> STJRefGen_Deser_U100() => JsonSerializer.Deserialize<Node20<string>>(_u100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U100")]
    public Node20<string> STJSrcGen_Deser_U100() => JsonSerializer.Deserialize(_u100_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-U100")]
    public Node20<string> Newtonsoft_Deser_U100() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_u100_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-U100")]
    public Node20<string> SpanJson_Deser_U100() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_u100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-U100")]
    public Node20<string> Utf8Json_Deser_U100() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_u100_b)!;

    [Benchmark, BenchmarkCategory("Serialize-U100")]
    public byte[] STJRefGen_Ser_U100() => JsonSerializer.SerializeToUtf8Bytes(_u100);
    [Benchmark, BenchmarkCategory("Serialize-U100")]
    public byte[] STJSrcGen_Ser_U100() => JsonSerializer.SerializeToUtf8Bytes(_u100, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-U100")]
    public byte[] Newtonsoft_Ser_U100() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_u100));
    [Benchmark, BenchmarkCategory("Serialize-U100")]
    public byte[] SpanJson_Ser_U100() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_u100);
    [Benchmark, BenchmarkCategory("Serialize-U100")]
    public byte[] Utf8Json_Ser_U100() => Utf8Json.JsonSerializer.Serialize(_u100);

    // ===================== E5 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E5")]
    public Node20<string> STJRefGen_Deser_E5() => JsonSerializer.Deserialize<Node20<string>>(_e5_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E5")]
    public Node20<string> STJSrcGen_Deser_E5() => JsonSerializer.Deserialize(_e5_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-E5")]
    public Node20<string> Newtonsoft_Deser_E5() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_e5_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-E5")]
    public Node20<string> SpanJson_Deser_E5() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_e5_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E5")]
    public Node20<string> Utf8Json_Deser_E5() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_e5_b)!;

    [Benchmark, BenchmarkCategory("Serialize-E5")]
    public byte[] STJRefGen_Ser_E5() => JsonSerializer.SerializeToUtf8Bytes(_e5);
    [Benchmark, BenchmarkCategory("Serialize-E5")]
    public byte[] STJSrcGen_Ser_E5() => JsonSerializer.SerializeToUtf8Bytes(_e5, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-E5")]
    public byte[] Newtonsoft_Ser_E5() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_e5));
    [Benchmark, BenchmarkCategory("Serialize-E5")]
    public byte[] SpanJson_Ser_E5() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_e5);
    [Benchmark, BenchmarkCategory("Serialize-E5")]
    public byte[] Utf8Json_Ser_E5() => Utf8Json.JsonSerializer.Serialize(_e5);

    // ===================== E10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> STJRefGen_Deser_E10() => JsonSerializer.Deserialize<Node20<string>>(_e10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> STJSrcGen_Deser_E10() => JsonSerializer.Deserialize(_e10_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> Newtonsoft_Deser_E10() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_e10_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> SpanJson_Deser_E10() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_e10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> Utf8Json_Deser_E10() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_e10_b)!;

    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public byte[] STJRefGen_Ser_E10() => JsonSerializer.SerializeToUtf8Bytes(_e10);
    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public byte[] STJSrcGen_Ser_E10() => JsonSerializer.SerializeToUtf8Bytes(_e10, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public byte[] Newtonsoft_Ser_E10() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_e10));
    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public byte[] SpanJson_Ser_E10() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_e10);
    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public byte[] Utf8Json_Ser_E10() => Utf8Json.JsonSerializer.Serialize(_e10);

    // ===================== E25 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E25")]
    public Node20<string> STJRefGen_Deser_E25() => JsonSerializer.Deserialize<Node20<string>>(_e25_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E25")]
    public Node20<string> STJSrcGen_Deser_E25() => JsonSerializer.Deserialize(_e25_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-E25")]
    public Node20<string> Newtonsoft_Deser_E25() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_e25_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-E25")]
    public Node20<string> SpanJson_Deser_E25() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_e25_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E25")]
    public Node20<string> Utf8Json_Deser_E25() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_e25_b)!;

    [Benchmark, BenchmarkCategory("Serialize-E25")]
    public byte[] STJRefGen_Ser_E25() => JsonSerializer.SerializeToUtf8Bytes(_e25);
    [Benchmark, BenchmarkCategory("Serialize-E25")]
    public byte[] STJSrcGen_Ser_E25() => JsonSerializer.SerializeToUtf8Bytes(_e25, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-E25")]
    public byte[] Newtonsoft_Ser_E25() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_e25));
    [Benchmark, BenchmarkCategory("Serialize-E25")]
    public byte[] SpanJson_Ser_E25() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_e25);
    [Benchmark, BenchmarkCategory("Serialize-E25")]
    public byte[] Utf8Json_Ser_E25() => Utf8Json.JsonSerializer.Serialize(_e25);

    // ===================== E50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> STJRefGen_Deser_E50() => JsonSerializer.Deserialize<Node20<string>>(_e50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> STJSrcGen_Deser_E50() => JsonSerializer.Deserialize(_e50_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> Newtonsoft_Deser_E50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_e50_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> SpanJson_Deser_E50() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_e50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> Utf8Json_Deser_E50() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_e50_b)!;

    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public byte[] STJRefGen_Ser_E50() => JsonSerializer.SerializeToUtf8Bytes(_e50);
    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public byte[] STJSrcGen_Ser_E50() => JsonSerializer.SerializeToUtf8Bytes(_e50, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public byte[] Newtonsoft_Ser_E50() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_e50));
    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public byte[] SpanJson_Ser_E50() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_e50);
    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public byte[] Utf8Json_Ser_E50() => Utf8Json.JsonSerializer.Serialize(_e50);

    // ===================== E100 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E100")]
    public Node20<string> STJRefGen_Deser_E100() => JsonSerializer.Deserialize<Node20<string>>(_e100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E100")]
    public Node20<string> STJSrcGen_Deser_E100() => JsonSerializer.Deserialize(_e100_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-E100")]
    public Node20<string> Newtonsoft_Deser_E100() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_e100_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-E100")]
    public Node20<string> SpanJson_Deser_E100() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_e100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-E100")]
    public Node20<string> Utf8Json_Deser_E100() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_e100_b)!;

    [Benchmark, BenchmarkCategory("Serialize-E100")]
    public byte[] STJRefGen_Ser_E100() => JsonSerializer.SerializeToUtf8Bytes(_e100);
    [Benchmark, BenchmarkCategory("Serialize-E100")]
    public byte[] STJSrcGen_Ser_E100() => JsonSerializer.SerializeToUtf8Bytes(_e100, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-E100")]
    public byte[] Newtonsoft_Ser_E100() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_e100));
    [Benchmark, BenchmarkCategory("Serialize-E100")]
    public byte[] SpanJson_Ser_E100() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_e100);
    [Benchmark, BenchmarkCategory("Serialize-E100")]
    public byte[] Utf8Json_Ser_E100() => Utf8Json.JsonSerializer.Serialize(_e100);

    // ===================== UE5 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE5")]
    public Node20<string> STJRefGen_Deser_UE5() => JsonSerializer.Deserialize<Node20<string>>(_ue5_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE5")]
    public Node20<string> STJSrcGen_Deser_UE5() => JsonSerializer.Deserialize(_ue5_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE5")]
    public Node20<string> Newtonsoft_Deser_UE5() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_ue5_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE5")]
    public Node20<string> SpanJson_Deser_UE5() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_ue5_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE5")]
    public Node20<string> Utf8Json_Deser_UE5() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_ue5_b)!;

    [Benchmark, BenchmarkCategory("Serialize-UE5")]
    public byte[] STJRefGen_Ser_UE5() => JsonSerializer.SerializeToUtf8Bytes(_ue5);
    [Benchmark, BenchmarkCategory("Serialize-UE5")]
    public byte[] STJSrcGen_Ser_UE5() => JsonSerializer.SerializeToUtf8Bytes(_ue5, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-UE5")]
    public byte[] Newtonsoft_Ser_UE5() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_ue5));
    [Benchmark, BenchmarkCategory("Serialize-UE5")]
    public byte[] SpanJson_Ser_UE5() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_ue5);
    [Benchmark, BenchmarkCategory("Serialize-UE5")]
    public byte[] Utf8Json_Ser_UE5() => Utf8Json.JsonSerializer.Serialize(_ue5);

    // ===================== UE10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> STJRefGen_Deser_UE10() => JsonSerializer.Deserialize<Node20<string>>(_ue10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> STJSrcGen_Deser_UE10() => JsonSerializer.Deserialize(_ue10_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> Newtonsoft_Deser_UE10() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_ue10_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> SpanJson_Deser_UE10() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_ue10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> Utf8Json_Deser_UE10() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_ue10_b)!;

    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public byte[] STJRefGen_Ser_UE10() => JsonSerializer.SerializeToUtf8Bytes(_ue10);
    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public byte[] STJSrcGen_Ser_UE10() => JsonSerializer.SerializeToUtf8Bytes(_ue10, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public byte[] Newtonsoft_Ser_UE10() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_ue10));
    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public byte[] SpanJson_Ser_UE10() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_ue10);
    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public byte[] Utf8Json_Ser_UE10() => Utf8Json.JsonSerializer.Serialize(_ue10);

    // ===================== UE25 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE25")]
    public Node20<string> STJRefGen_Deser_UE25() => JsonSerializer.Deserialize<Node20<string>>(_ue25_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE25")]
    public Node20<string> STJSrcGen_Deser_UE25() => JsonSerializer.Deserialize(_ue25_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE25")]
    public Node20<string> Newtonsoft_Deser_UE25() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_ue25_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE25")]
    public Node20<string> SpanJson_Deser_UE25() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_ue25_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE25")]
    public Node20<string> Utf8Json_Deser_UE25() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_ue25_b)!;

    [Benchmark, BenchmarkCategory("Serialize-UE25")]
    public byte[] STJRefGen_Ser_UE25() => JsonSerializer.SerializeToUtf8Bytes(_ue25);
    [Benchmark, BenchmarkCategory("Serialize-UE25")]
    public byte[] STJSrcGen_Ser_UE25() => JsonSerializer.SerializeToUtf8Bytes(_ue25, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-UE25")]
    public byte[] Newtonsoft_Ser_UE25() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_ue25));
    [Benchmark, BenchmarkCategory("Serialize-UE25")]
    public byte[] SpanJson_Ser_UE25() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_ue25);
    [Benchmark, BenchmarkCategory("Serialize-UE25")]
    public byte[] Utf8Json_Ser_UE25() => Utf8Json.JsonSerializer.Serialize(_ue25);

    // ===================== UE50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> STJRefGen_Deser_UE50() => JsonSerializer.Deserialize<Node20<string>>(_ue50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> STJSrcGen_Deser_UE50() => JsonSerializer.Deserialize(_ue50_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> Newtonsoft_Deser_UE50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_ue50_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> SpanJson_Deser_UE50() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_ue50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> Utf8Json_Deser_UE50() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_ue50_b)!;

    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public byte[] STJRefGen_Ser_UE50() => JsonSerializer.SerializeToUtf8Bytes(_ue50);
    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public byte[] STJSrcGen_Ser_UE50() => JsonSerializer.SerializeToUtf8Bytes(_ue50, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public byte[] Newtonsoft_Ser_UE50() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_ue50));
    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public byte[] SpanJson_Ser_UE50() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_ue50);
    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public byte[] Utf8Json_Ser_UE50() => Utf8Json.JsonSerializer.Serialize(_ue50);

    // ===================== UE100 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE100")]
    public Node20<string> STJRefGen_Deser_UE100() => JsonSerializer.Deserialize<Node20<string>>(_ue100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE100")]
    public Node20<string> STJSrcGen_Deser_UE100() => JsonSerializer.Deserialize(_ue100_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE100")]
    public Node20<string> Newtonsoft_Deser_UE100() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_ue100_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE100")]
    public Node20<string> SpanJson_Deser_UE100() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_ue100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE100")]
    public Node20<string> Utf8Json_Deser_UE100() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_ue100_b)!;

    [Benchmark, BenchmarkCategory("Serialize-UE100")]
    public byte[] STJRefGen_Ser_UE100() => JsonSerializer.SerializeToUtf8Bytes(_ue100);
    [Benchmark, BenchmarkCategory("Serialize-UE100")]
    public byte[] STJSrcGen_Ser_UE100() => JsonSerializer.SerializeToUtf8Bytes(_ue100, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-UE100")]
    public byte[] Newtonsoft_Ser_UE100() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_ue100));
    [Benchmark, BenchmarkCategory("Serialize-UE100")]
    public byte[] SpanJson_Ser_UE100() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_ue100);
    [Benchmark, BenchmarkCategory("Serialize-UE100")]
    public byte[] Utf8Json_Ser_UE100() => Utf8Json.JsonSerializer.Serialize(_ue100);
}
