using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// String length isolation benchmark: varies string value length (7 levels), byte[] I/O.
/// Baseline: D5, W20, Textual, Object-only, ASCII, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class StringLengthIsolationByteBench
{
    private byte[] _l5_b = null!; private Node20<string> _l5 = null!;
    private byte[] _l20_b = null!; private Node20<string> _l20 = null!;
    private byte[] _l50_b = null!; private Node20<string> _l50 = null!;
    private byte[] _l200_b = null!; private Node20<string> _l200 = null!;
    private byte[] _l500_b = null!; private Node20<string> _l500 = null!;
    private byte[] _l2000_b = null!; private Node20<string> _l2000 = null!;
    private byte[] _l10000_b = null!; private Node20<string> _l10000 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _l5_b = Load("L5"); _l5 = JsonSerializer.Deserialize<Node20<string>>(_l5_b)!;
        _l20_b = Load("L20"); _l20 = JsonSerializer.Deserialize<Node20<string>>(_l20_b)!;
        _l50_b = Load("L50"); _l50 = JsonSerializer.Deserialize<Node20<string>>(_l50_b)!;
        _l200_b = Load("L200"); _l200 = JsonSerializer.Deserialize<Node20<string>>(_l200_b)!;
        _l500_b = Load("L500"); _l500 = JsonSerializer.Deserialize<Node20<string>>(_l500_b)!;
        _l2000_b = Load("L2000"); _l2000 = JsonSerializer.Deserialize<Node20<string>>(_l2000_b)!;
        _l10000_b = Load("L10000"); _l10000 = JsonSerializer.Deserialize<Node20<string>>(_l10000_b)!;
    }

    private static byte[] Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoStringLength", $"{id}.json");
        return File.ReadAllBytes(path);
    }

    // ===================== L5 =====================

    [Benchmark, BenchmarkCategory("Deserialize-L5")]
    public Node20<string> STJRefGen_Deser_L5() => JsonSerializer.Deserialize<Node20<string>>(_l5_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-L5")]
    public Node20<string> STJSrcGen_Deser_L5() => JsonSerializer.Deserialize(_l5_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-L5")]
    public Node20<string> Newtonsoft_Deser_L5() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_l5_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-L5")]
    public Node20<string> SpanJson_Deser_L5() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_l5_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-L5")]
    public Node20<string> Utf8Json_Deser_L5() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_l5_b)!;

    [Benchmark, BenchmarkCategory("Serialize-L5")]
    public byte[] STJRefGen_Ser_L5() => JsonSerializer.SerializeToUtf8Bytes(_l5);
    [Benchmark, BenchmarkCategory("Serialize-L5")]
    public byte[] STJSrcGen_Ser_L5() => JsonSerializer.SerializeToUtf8Bytes(_l5, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-L5")]
    public byte[] Newtonsoft_Ser_L5() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_l5));
    [Benchmark, BenchmarkCategory("Serialize-L5")]
    public byte[] SpanJson_Ser_L5() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_l5);
    [Benchmark, BenchmarkCategory("Serialize-L5")]
    public byte[] Utf8Json_Ser_L5() => Utf8Json.JsonSerializer.Serialize(_l5);

    // ===================== L20 =====================

    [Benchmark, BenchmarkCategory("Deserialize-L20")]
    public Node20<string> STJRefGen_Deser_L20() => JsonSerializer.Deserialize<Node20<string>>(_l20_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-L20")]
    public Node20<string> STJSrcGen_Deser_L20() => JsonSerializer.Deserialize(_l20_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-L20")]
    public Node20<string> Newtonsoft_Deser_L20() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_l20_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-L20")]
    public Node20<string> SpanJson_Deser_L20() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_l20_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-L20")]
    public Node20<string> Utf8Json_Deser_L20() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_l20_b)!;

    [Benchmark, BenchmarkCategory("Serialize-L20")]
    public byte[] STJRefGen_Ser_L20() => JsonSerializer.SerializeToUtf8Bytes(_l20);
    [Benchmark, BenchmarkCategory("Serialize-L20")]
    public byte[] STJSrcGen_Ser_L20() => JsonSerializer.SerializeToUtf8Bytes(_l20, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-L20")]
    public byte[] Newtonsoft_Ser_L20() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_l20));
    [Benchmark, BenchmarkCategory("Serialize-L20")]
    public byte[] SpanJson_Ser_L20() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_l20);
    [Benchmark, BenchmarkCategory("Serialize-L20")]
    public byte[] Utf8Json_Ser_L20() => Utf8Json.JsonSerializer.Serialize(_l20);

    // ===================== L50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-L50")]
    public Node20<string> STJRefGen_Deser_L50() => JsonSerializer.Deserialize<Node20<string>>(_l50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-L50")]
    public Node20<string> STJSrcGen_Deser_L50() => JsonSerializer.Deserialize(_l50_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-L50")]
    public Node20<string> Newtonsoft_Deser_L50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_l50_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-L50")]
    public Node20<string> SpanJson_Deser_L50() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_l50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-L50")]
    public Node20<string> Utf8Json_Deser_L50() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_l50_b)!;

    [Benchmark, BenchmarkCategory("Serialize-L50")]
    public byte[] STJRefGen_Ser_L50() => JsonSerializer.SerializeToUtf8Bytes(_l50);
    [Benchmark, BenchmarkCategory("Serialize-L50")]
    public byte[] STJSrcGen_Ser_L50() => JsonSerializer.SerializeToUtf8Bytes(_l50, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-L50")]
    public byte[] Newtonsoft_Ser_L50() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_l50));
    [Benchmark, BenchmarkCategory("Serialize-L50")]
    public byte[] SpanJson_Ser_L50() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_l50);
    [Benchmark, BenchmarkCategory("Serialize-L50")]
    public byte[] Utf8Json_Ser_L50() => Utf8Json.JsonSerializer.Serialize(_l50);

    // ===================== L200 =====================

    [Benchmark, BenchmarkCategory("Deserialize-L200")]
    public Node20<string> STJRefGen_Deser_L200() => JsonSerializer.Deserialize<Node20<string>>(_l200_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-L200")]
    public Node20<string> STJSrcGen_Deser_L200() => JsonSerializer.Deserialize(_l200_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-L200")]
    public Node20<string> Newtonsoft_Deser_L200() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_l200_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-L200")]
    public Node20<string> SpanJson_Deser_L200() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_l200_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-L200")]
    public Node20<string> Utf8Json_Deser_L200() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_l200_b)!;

    [Benchmark, BenchmarkCategory("Serialize-L200")]
    public byte[] STJRefGen_Ser_L200() => JsonSerializer.SerializeToUtf8Bytes(_l200);
    [Benchmark, BenchmarkCategory("Serialize-L200")]
    public byte[] STJSrcGen_Ser_L200() => JsonSerializer.SerializeToUtf8Bytes(_l200, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-L200")]
    public byte[] Newtonsoft_Ser_L200() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_l200));
    [Benchmark, BenchmarkCategory("Serialize-L200")]
    public byte[] SpanJson_Ser_L200() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_l200);
    [Benchmark, BenchmarkCategory("Serialize-L200")]
    public byte[] Utf8Json_Ser_L200() => Utf8Json.JsonSerializer.Serialize(_l200);

    // ===================== L500 =====================

    [Benchmark, BenchmarkCategory("Deserialize-L500")]
    public Node20<string> STJRefGen_Deser_L500() => JsonSerializer.Deserialize<Node20<string>>(_l500_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-L500")]
    public Node20<string> STJSrcGen_Deser_L500() => JsonSerializer.Deserialize(_l500_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-L500")]
    public Node20<string> Newtonsoft_Deser_L500() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_l500_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-L500")]
    public Node20<string> SpanJson_Deser_L500() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_l500_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-L500")]
    public Node20<string> Utf8Json_Deser_L500() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_l500_b)!;

    [Benchmark, BenchmarkCategory("Serialize-L500")]
    public byte[] STJRefGen_Ser_L500() => JsonSerializer.SerializeToUtf8Bytes(_l500);
    [Benchmark, BenchmarkCategory("Serialize-L500")]
    public byte[] STJSrcGen_Ser_L500() => JsonSerializer.SerializeToUtf8Bytes(_l500, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-L500")]
    public byte[] Newtonsoft_Ser_L500() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_l500));
    [Benchmark, BenchmarkCategory("Serialize-L500")]
    public byte[] SpanJson_Ser_L500() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_l500);
    [Benchmark, BenchmarkCategory("Serialize-L500")]
    public byte[] Utf8Json_Ser_L500() => Utf8Json.JsonSerializer.Serialize(_l500);

    // ===================== L2000 =====================

    [Benchmark, BenchmarkCategory("Deserialize-L2000")]
    public Node20<string> STJRefGen_Deser_L2000() => JsonSerializer.Deserialize<Node20<string>>(_l2000_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-L2000")]
    public Node20<string> STJSrcGen_Deser_L2000() => JsonSerializer.Deserialize(_l2000_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-L2000")]
    public Node20<string> Newtonsoft_Deser_L2000() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_l2000_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-L2000")]
    public Node20<string> SpanJson_Deser_L2000() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_l2000_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-L2000")]
    public Node20<string> Utf8Json_Deser_L2000() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_l2000_b)!;

    [Benchmark, BenchmarkCategory("Serialize-L2000")]
    public byte[] STJRefGen_Ser_L2000() => JsonSerializer.SerializeToUtf8Bytes(_l2000);
    [Benchmark, BenchmarkCategory("Serialize-L2000")]
    public byte[] STJSrcGen_Ser_L2000() => JsonSerializer.SerializeToUtf8Bytes(_l2000, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-L2000")]
    public byte[] Newtonsoft_Ser_L2000() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_l2000));
    [Benchmark, BenchmarkCategory("Serialize-L2000")]
    public byte[] SpanJson_Ser_L2000() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_l2000);
    [Benchmark, BenchmarkCategory("Serialize-L2000")]
    public byte[] Utf8Json_Ser_L2000() => Utf8Json.JsonSerializer.Serialize(_l2000);

    // ===================== L10000 =====================

    [Benchmark, BenchmarkCategory("Deserialize-L10000")]
    public Node20<string> STJRefGen_Deser_L10000() => JsonSerializer.Deserialize<Node20<string>>(_l10000_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-L10000")]
    public Node20<string> STJSrcGen_Deser_L10000() => JsonSerializer.Deserialize(_l10000_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-L10000")]
    public Node20<string> Newtonsoft_Deser_L10000() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_l10000_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-L10000")]
    public Node20<string> SpanJson_Deser_L10000() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_l10000_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-L10000")]
    public Node20<string> Utf8Json_Deser_L10000() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_l10000_b)!;

    [Benchmark, BenchmarkCategory("Serialize-L10000")]
    public byte[] STJRefGen_Ser_L10000() => JsonSerializer.SerializeToUtf8Bytes(_l10000);
    [Benchmark, BenchmarkCategory("Serialize-L10000")]
    public byte[] STJSrcGen_Ser_L10000() => JsonSerializer.SerializeToUtf8Bytes(_l10000, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-L10000")]
    public byte[] Newtonsoft_Ser_L10000() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_l10000));
    [Benchmark, BenchmarkCategory("Serialize-L10000")]
    public byte[] SpanJson_Ser_L10000() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_l10000);
    [Benchmark, BenchmarkCategory("Serialize-L10000")]
    public byte[] Utf8Json_Ser_L10000() => Utf8Json.JsonSerializer.Serialize(_l10000);
}
