using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Width isolation benchmark: varies width (7 levels), byte[] I/O.
/// Baseline: D5, Textual, Object-only, ASCII, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class WidthIsolationByteBench
{
    private byte[] _w2_b = null!; private Node2<string> _w2 = null!;
    private byte[] _w5_b = null!; private Node5<string> _w5 = null!;
    private byte[] _w10_b = null!; private Node10<string> _w10 = null!;
    private byte[] _w20_b = null!; private Node20<string> _w20 = null!;
    private byte[] _w50_b = null!; private Node50<string> _w50 = null!;
    private byte[] _w100_b = null!; private Node100<string> _w100 = null!;
    private byte[] _w200_b = null!; private Node200<string> _w200 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _w2_b = Load("W2"); _w2 = JsonSerializer.Deserialize<Node2<string>>(_w2_b)!;
        _w5_b = Load("W5"); _w5 = JsonSerializer.Deserialize<Node5<string>>(_w5_b)!;
        _w10_b = Load("W10"); _w10 = JsonSerializer.Deserialize<Node10<string>>(_w10_b)!;
        _w20_b = Load("W20"); _w20 = JsonSerializer.Deserialize<Node20<string>>(_w20_b)!;
        _w50_b = Load("W50"); _w50 = JsonSerializer.Deserialize<Node50<string>>(_w50_b)!;
        _w100_b = Load("W100"); _w100 = JsonSerializer.Deserialize<Node100<string>>(_w100_b)!;
        _w200_b = Load("W200"); _w200 = JsonSerializer.Deserialize<Node200<string>>(_w200_b)!;
    }

    private static byte[] Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoWidth", $"{id}.json");
        return File.ReadAllBytes(path);
    }

    // ===================== W2 =====================

    [Benchmark, BenchmarkCategory("Deserialize-W2")]
    public Node2<string> STJRefGen_Deser_W2() => JsonSerializer.Deserialize<Node2<string>>(_w2_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-W2")]
    public Node2<string> STJSrcGen_Deser_W2() => JsonSerializer.Deserialize(_w2_b, IsolationJsonContext.Default.Node2String)!;
    [Benchmark, BenchmarkCategory("Deserialize-W2")]
    public Node2<string> Newtonsoft_Deser_W2() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node2<string>>(Encoding.UTF8.GetString(_w2_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-W2")]
    public Node2<string> SpanJson_Deser_W2() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node2<string>>(_w2_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-W2")]
    public Node2<string> Utf8Json_Deser_W2() => Utf8Json.JsonSerializer.Deserialize<Node2<string>>(_w2_b)!;

    [Benchmark, BenchmarkCategory("Serialize-W2")]
    public byte[] STJRefGen_Ser_W2() => JsonSerializer.SerializeToUtf8Bytes(_w2);
    [Benchmark, BenchmarkCategory("Serialize-W2")]
    public byte[] STJSrcGen_Ser_W2() => JsonSerializer.SerializeToUtf8Bytes(_w2, IsolationJsonContext.Default.Node2String);
    [Benchmark, BenchmarkCategory("Serialize-W2")]
    public byte[] Newtonsoft_Ser_W2() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_w2));
    [Benchmark, BenchmarkCategory("Serialize-W2")]
    public byte[] SpanJson_Ser_W2() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_w2);
    [Benchmark, BenchmarkCategory("Serialize-W2")]
    public byte[] Utf8Json_Ser_W2() => Utf8Json.JsonSerializer.Serialize(_w2);

    // ===================== W5 =====================

    [Benchmark, BenchmarkCategory("Deserialize-W5")]
    public Node5<string> STJRefGen_Deser_W5() => JsonSerializer.Deserialize<Node5<string>>(_w5_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-W5")]
    public Node5<string> STJSrcGen_Deser_W5() => JsonSerializer.Deserialize(_w5_b, IsolationJsonContext.Default.Node5String)!;
    [Benchmark, BenchmarkCategory("Deserialize-W5")]
    public Node5<string> Newtonsoft_Deser_W5() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node5<string>>(Encoding.UTF8.GetString(_w5_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-W5")]
    public Node5<string> SpanJson_Deser_W5() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node5<string>>(_w5_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-W5")]
    public Node5<string> Utf8Json_Deser_W5() => Utf8Json.JsonSerializer.Deserialize<Node5<string>>(_w5_b)!;

    [Benchmark, BenchmarkCategory("Serialize-W5")]
    public byte[] STJRefGen_Ser_W5() => JsonSerializer.SerializeToUtf8Bytes(_w5);
    [Benchmark, BenchmarkCategory("Serialize-W5")]
    public byte[] STJSrcGen_Ser_W5() => JsonSerializer.SerializeToUtf8Bytes(_w5, IsolationJsonContext.Default.Node5String);
    [Benchmark, BenchmarkCategory("Serialize-W5")]
    public byte[] Newtonsoft_Ser_W5() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_w5));
    [Benchmark, BenchmarkCategory("Serialize-W5")]
    public byte[] SpanJson_Ser_W5() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_w5);
    [Benchmark, BenchmarkCategory("Serialize-W5")]
    public byte[] Utf8Json_Ser_W5() => Utf8Json.JsonSerializer.Serialize(_w5);

    // ===================== W10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-W10")]
    public Node10<string> STJRefGen_Deser_W10() => JsonSerializer.Deserialize<Node10<string>>(_w10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-W10")]
    public Node10<string> STJSrcGen_Deser_W10() => JsonSerializer.Deserialize(_w10_b, IsolationJsonContext.Default.Node10String)!;
    [Benchmark, BenchmarkCategory("Deserialize-W10")]
    public Node10<string> Newtonsoft_Deser_W10() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node10<string>>(Encoding.UTF8.GetString(_w10_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-W10")]
    public Node10<string> SpanJson_Deser_W10() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node10<string>>(_w10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-W10")]
    public Node10<string> Utf8Json_Deser_W10() => Utf8Json.JsonSerializer.Deserialize<Node10<string>>(_w10_b)!;

    [Benchmark, BenchmarkCategory("Serialize-W10")]
    public byte[] STJRefGen_Ser_W10() => JsonSerializer.SerializeToUtf8Bytes(_w10);
    [Benchmark, BenchmarkCategory("Serialize-W10")]
    public byte[] STJSrcGen_Ser_W10() => JsonSerializer.SerializeToUtf8Bytes(_w10, IsolationJsonContext.Default.Node10String);
    [Benchmark, BenchmarkCategory("Serialize-W10")]
    public byte[] Newtonsoft_Ser_W10() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_w10));
    [Benchmark, BenchmarkCategory("Serialize-W10")]
    public byte[] SpanJson_Ser_W10() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_w10);
    [Benchmark, BenchmarkCategory("Serialize-W10")]
    public byte[] Utf8Json_Ser_W10() => Utf8Json.JsonSerializer.Serialize(_w10);

    // ===================== W20 =====================

    [Benchmark, BenchmarkCategory("Deserialize-W20")]
    public Node20<string> STJRefGen_Deser_W20() => JsonSerializer.Deserialize<Node20<string>>(_w20_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-W20")]
    public Node20<string> STJSrcGen_Deser_W20() => JsonSerializer.Deserialize(_w20_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-W20")]
    public Node20<string> Newtonsoft_Deser_W20() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_w20_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-W20")]
    public Node20<string> SpanJson_Deser_W20() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_w20_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-W20")]
    public Node20<string> Utf8Json_Deser_W20() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_w20_b)!;

    [Benchmark, BenchmarkCategory("Serialize-W20")]
    public byte[] STJRefGen_Ser_W20() => JsonSerializer.SerializeToUtf8Bytes(_w20);
    [Benchmark, BenchmarkCategory("Serialize-W20")]
    public byte[] STJSrcGen_Ser_W20() => JsonSerializer.SerializeToUtf8Bytes(_w20, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-W20")]
    public byte[] Newtonsoft_Ser_W20() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_w20));
    [Benchmark, BenchmarkCategory("Serialize-W20")]
    public byte[] SpanJson_Ser_W20() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_w20);
    [Benchmark, BenchmarkCategory("Serialize-W20")]
    public byte[] Utf8Json_Ser_W20() => Utf8Json.JsonSerializer.Serialize(_w20);

    // ===================== W50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-W50")]
    public Node50<string> STJRefGen_Deser_W50() => JsonSerializer.Deserialize<Node50<string>>(_w50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-W50")]
    public Node50<string> STJSrcGen_Deser_W50() => JsonSerializer.Deserialize(_w50_b, IsolationJsonContext.Default.Node50String)!;
    [Benchmark, BenchmarkCategory("Deserialize-W50")]
    public Node50<string> Newtonsoft_Deser_W50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node50<string>>(Encoding.UTF8.GetString(_w50_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-W50")]
    public Node50<string> SpanJson_Deser_W50() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node50<string>>(_w50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-W50")]
    public Node50<string> Utf8Json_Deser_W50() => Utf8Json.JsonSerializer.Deserialize<Node50<string>>(_w50_b)!;

    [Benchmark, BenchmarkCategory("Serialize-W50")]
    public byte[] STJRefGen_Ser_W50() => JsonSerializer.SerializeToUtf8Bytes(_w50);
    [Benchmark, BenchmarkCategory("Serialize-W50")]
    public byte[] STJSrcGen_Ser_W50() => JsonSerializer.SerializeToUtf8Bytes(_w50, IsolationJsonContext.Default.Node50String);
    [Benchmark, BenchmarkCategory("Serialize-W50")]
    public byte[] Newtonsoft_Ser_W50() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_w50));
    [Benchmark, BenchmarkCategory("Serialize-W50")]
    public byte[] SpanJson_Ser_W50() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_w50);
    [Benchmark, BenchmarkCategory("Serialize-W50")]
    public byte[] Utf8Json_Ser_W50() => Utf8Json.JsonSerializer.Serialize(_w50);

    // ===================== W100 =====================

    [Benchmark, BenchmarkCategory("Deserialize-W100")]
    public Node100<string> STJRefGen_Deser_W100() => JsonSerializer.Deserialize<Node100<string>>(_w100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-W100")]
    public Node100<string> STJSrcGen_Deser_W100() => JsonSerializer.Deserialize(_w100_b, IsolationJsonContext.Default.Node100String)!;
    [Benchmark, BenchmarkCategory("Deserialize-W100")]
    public Node100<string> Newtonsoft_Deser_W100() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node100<string>>(Encoding.UTF8.GetString(_w100_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-W100")]
    public Node100<string> SpanJson_Deser_W100() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node100<string>>(_w100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-W100")]
    public Node100<string> Utf8Json_Deser_W100() => Utf8Json.JsonSerializer.Deserialize<Node100<string>>(_w100_b)!;

    [Benchmark, BenchmarkCategory("Serialize-W100")]
    public byte[] STJRefGen_Ser_W100() => JsonSerializer.SerializeToUtf8Bytes(_w100);
    [Benchmark, BenchmarkCategory("Serialize-W100")]
    public byte[] STJSrcGen_Ser_W100() => JsonSerializer.SerializeToUtf8Bytes(_w100, IsolationJsonContext.Default.Node100String);
    [Benchmark, BenchmarkCategory("Serialize-W100")]
    public byte[] Newtonsoft_Ser_W100() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_w100));
    [Benchmark, BenchmarkCategory("Serialize-W100")]
    public byte[] SpanJson_Ser_W100() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_w100);
    [Benchmark, BenchmarkCategory("Serialize-W100")]
    public byte[] Utf8Json_Ser_W100() => Utf8Json.JsonSerializer.Serialize(_w100);

    // ===================== W200 =====================

    [Benchmark, BenchmarkCategory("Deserialize-W200")]
    public Node200<string> STJRefGen_Deser_W200() => JsonSerializer.Deserialize<Node200<string>>(_w200_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-W200")]
    public Node200<string> STJSrcGen_Deser_W200() => JsonSerializer.Deserialize(_w200_b, IsolationJsonContext.Default.Node200String)!;
    [Benchmark, BenchmarkCategory("Deserialize-W200")]
    public Node200<string> Newtonsoft_Deser_W200() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node200<string>>(Encoding.UTF8.GetString(_w200_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-W200")]
    public Node200<string> SpanJson_Deser_W200() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node200<string>>(_w200_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-W200")]
    public Node200<string> Utf8Json_Deser_W200() => Utf8Json.JsonSerializer.Deserialize<Node200<string>>(_w200_b)!;

    [Benchmark, BenchmarkCategory("Serialize-W200")]
    public byte[] STJRefGen_Ser_W200() => JsonSerializer.SerializeToUtf8Bytes(_w200);
    [Benchmark, BenchmarkCategory("Serialize-W200")]
    public byte[] STJSrcGen_Ser_W200() => JsonSerializer.SerializeToUtf8Bytes(_w200, IsolationJsonContext.Default.Node200String);
    [Benchmark, BenchmarkCategory("Serialize-W200")]
    public byte[] Newtonsoft_Ser_W200() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_w200));
    [Benchmark, BenchmarkCategory("Serialize-W200")]
    public byte[] SpanJson_Ser_W200() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_w200);
    [Benchmark, BenchmarkCategory("Serialize-W200")]
    public byte[] Utf8Json_Ser_W200() => Utf8Json.JsonSerializer.Serialize(_w200);
}
