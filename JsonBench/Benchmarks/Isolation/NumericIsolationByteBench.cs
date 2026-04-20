using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Numeric isolation benchmark: varies integer/float ratio (5 levels), byte[] I/O.
/// Content is 100% numeric. Baseline: D5, W20, Object-only, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class NumericIsolationByteBench
{
    private byte[] _i100_b = null!; private Node20<double> _i100 = null!;
    private byte[] _i70_b = null!; private Node20<double> _i70 = null!;
    private byte[] _i50_b = null!; private Node20<double> _i50 = null!;
    private byte[] _i30_b = null!; private Node20<double> _i30 = null!;
    private byte[] _f100_b = null!; private Node20<double> _f100 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _i100_b = Load("I100"); _i100 = JsonSerializer.Deserialize<Node20<double>>(_i100_b)!;
        _i70_b = Load("I70"); _i70 = JsonSerializer.Deserialize<Node20<double>>(_i70_b)!;
        _i50_b = Load("I50"); _i50 = JsonSerializer.Deserialize<Node20<double>>(_i50_b)!;
        _i30_b = Load("I30"); _i30 = JsonSerializer.Deserialize<Node20<double>>(_i30_b)!;
        _f100_b = Load("F100"); _f100 = JsonSerializer.Deserialize<Node20<double>>(_f100_b)!;
    }

    private static byte[] Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoNumeric", $"{id}.json");
        return File.ReadAllBytes(path);
    }

    // ===================== I100 =====================

    [Benchmark, BenchmarkCategory("Deserialize-I100")]
    public Node20<double> STJRefGen_Deser_I100() => JsonSerializer.Deserialize<Node20<double>>(_i100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I100")]
    public Node20<double> STJSrcGen_Deser_I100() => JsonSerializer.Deserialize(_i100_b, IsolationJsonContext.Default.Node20Double)!;
    [Benchmark, BenchmarkCategory("Deserialize-I100")]
    public Node20<double> Newtonsoft_Deser_I100() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(Encoding.UTF8.GetString(_i100_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-I100")]
    public Node20<double> SpanJson_Deser_I100() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<double>>(_i100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I100")]
    public Node20<double> Utf8Json_Deser_I100() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(_i100_b)!;

    [Benchmark, BenchmarkCategory("Serialize-I100")]
    public byte[] STJRefGen_Ser_I100() => JsonSerializer.SerializeToUtf8Bytes(_i100);
    [Benchmark, BenchmarkCategory("Serialize-I100")]
    public byte[] STJSrcGen_Ser_I100() => JsonSerializer.SerializeToUtf8Bytes(_i100, IsolationJsonContext.Default.Node20Double);
    [Benchmark, BenchmarkCategory("Serialize-I100")]
    public byte[] Newtonsoft_Ser_I100() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_i100));
    [Benchmark, BenchmarkCategory("Serialize-I100")]
    public byte[] SpanJson_Ser_I100() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_i100);
    [Benchmark, BenchmarkCategory("Serialize-I100")]
    public byte[] Utf8Json_Ser_I100() => Utf8Json.JsonSerializer.Serialize(_i100);

    // ===================== I70 =====================

    [Benchmark, BenchmarkCategory("Deserialize-I70")]
    public Node20<double> STJRefGen_Deser_I70() => JsonSerializer.Deserialize<Node20<double>>(_i70_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I70")]
    public Node20<double> STJSrcGen_Deser_I70() => JsonSerializer.Deserialize(_i70_b, IsolationJsonContext.Default.Node20Double)!;
    [Benchmark, BenchmarkCategory("Deserialize-I70")]
    public Node20<double> Newtonsoft_Deser_I70() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(Encoding.UTF8.GetString(_i70_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-I70")]
    public Node20<double> SpanJson_Deser_I70() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<double>>(_i70_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I70")]
    public Node20<double> Utf8Json_Deser_I70() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(_i70_b)!;

    [Benchmark, BenchmarkCategory("Serialize-I70")]
    public byte[] STJRefGen_Ser_I70() => JsonSerializer.SerializeToUtf8Bytes(_i70);
    [Benchmark, BenchmarkCategory("Serialize-I70")]
    public byte[] STJSrcGen_Ser_I70() => JsonSerializer.SerializeToUtf8Bytes(_i70, IsolationJsonContext.Default.Node20Double);
    [Benchmark, BenchmarkCategory("Serialize-I70")]
    public byte[] Newtonsoft_Ser_I70() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_i70));
    [Benchmark, BenchmarkCategory("Serialize-I70")]
    public byte[] SpanJson_Ser_I70() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_i70);
    [Benchmark, BenchmarkCategory("Serialize-I70")]
    public byte[] Utf8Json_Ser_I70() => Utf8Json.JsonSerializer.Serialize(_i70);

    // ===================== I50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-I50")]
    public Node20<double> STJRefGen_Deser_I50() => JsonSerializer.Deserialize<Node20<double>>(_i50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I50")]
    public Node20<double> STJSrcGen_Deser_I50() => JsonSerializer.Deserialize(_i50_b, IsolationJsonContext.Default.Node20Double)!;
    [Benchmark, BenchmarkCategory("Deserialize-I50")]
    public Node20<double> Newtonsoft_Deser_I50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(Encoding.UTF8.GetString(_i50_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-I50")]
    public Node20<double> SpanJson_Deser_I50() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<double>>(_i50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I50")]
    public Node20<double> Utf8Json_Deser_I50() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(_i50_b)!;

    [Benchmark, BenchmarkCategory("Serialize-I50")]
    public byte[] STJRefGen_Ser_I50() => JsonSerializer.SerializeToUtf8Bytes(_i50);
    [Benchmark, BenchmarkCategory("Serialize-I50")]
    public byte[] STJSrcGen_Ser_I50() => JsonSerializer.SerializeToUtf8Bytes(_i50, IsolationJsonContext.Default.Node20Double);
    [Benchmark, BenchmarkCategory("Serialize-I50")]
    public byte[] Newtonsoft_Ser_I50() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_i50));
    [Benchmark, BenchmarkCategory("Serialize-I50")]
    public byte[] SpanJson_Ser_I50() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_i50);
    [Benchmark, BenchmarkCategory("Serialize-I50")]
    public byte[] Utf8Json_Ser_I50() => Utf8Json.JsonSerializer.Serialize(_i50);

    // ===================== I30 =====================

    [Benchmark, BenchmarkCategory("Deserialize-I30")]
    public Node20<double> STJRefGen_Deser_I30() => JsonSerializer.Deserialize<Node20<double>>(_i30_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I30")]
    public Node20<double> STJSrcGen_Deser_I30() => JsonSerializer.Deserialize(_i30_b, IsolationJsonContext.Default.Node20Double)!;
    [Benchmark, BenchmarkCategory("Deserialize-I30")]
    public Node20<double> Newtonsoft_Deser_I30() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(Encoding.UTF8.GetString(_i30_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-I30")]
    public Node20<double> SpanJson_Deser_I30() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<double>>(_i30_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I30")]
    public Node20<double> Utf8Json_Deser_I30() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(_i30_b)!;

    [Benchmark, BenchmarkCategory("Serialize-I30")]
    public byte[] STJRefGen_Ser_I30() => JsonSerializer.SerializeToUtf8Bytes(_i30);
    [Benchmark, BenchmarkCategory("Serialize-I30")]
    public byte[] STJSrcGen_Ser_I30() => JsonSerializer.SerializeToUtf8Bytes(_i30, IsolationJsonContext.Default.Node20Double);
    [Benchmark, BenchmarkCategory("Serialize-I30")]
    public byte[] Newtonsoft_Ser_I30() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_i30));
    [Benchmark, BenchmarkCategory("Serialize-I30")]
    public byte[] SpanJson_Ser_I30() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_i30);
    [Benchmark, BenchmarkCategory("Serialize-I30")]
    public byte[] Utf8Json_Ser_I30() => Utf8Json.JsonSerializer.Serialize(_i30);

    // ===================== F100 =====================

    [Benchmark, BenchmarkCategory("Deserialize-F100")]
    public Node20<double> STJRefGen_Deser_F100() => JsonSerializer.Deserialize<Node20<double>>(_f100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-F100")]
    public Node20<double> STJSrcGen_Deser_F100() => JsonSerializer.Deserialize(_f100_b, IsolationJsonContext.Default.Node20Double)!;
    [Benchmark, BenchmarkCategory("Deserialize-F100")]
    public Node20<double> Newtonsoft_Deser_F100() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(Encoding.UTF8.GetString(_f100_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-F100")]
    public Node20<double> SpanJson_Deser_F100() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<double>>(_f100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-F100")]
    public Node20<double> Utf8Json_Deser_F100() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(_f100_b)!;

    [Benchmark, BenchmarkCategory("Serialize-F100")]
    public byte[] STJRefGen_Ser_F100() => JsonSerializer.SerializeToUtf8Bytes(_f100);
    [Benchmark, BenchmarkCategory("Serialize-F100")]
    public byte[] STJSrcGen_Ser_F100() => JsonSerializer.SerializeToUtf8Bytes(_f100, IsolationJsonContext.Default.Node20Double);
    [Benchmark, BenchmarkCategory("Serialize-F100")]
    public byte[] Newtonsoft_Ser_F100() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_f100));
    [Benchmark, BenchmarkCategory("Serialize-F100")]
    public byte[] SpanJson_Ser_F100() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_f100);
    [Benchmark, BenchmarkCategory("Serialize-F100")]
    public byte[] Utf8Json_Ser_F100() => Utf8Json.JsonSerializer.Serialize(_f100);
}
