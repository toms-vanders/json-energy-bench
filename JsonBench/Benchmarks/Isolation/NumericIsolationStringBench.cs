using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Numeric isolation benchmark: varies integer/float ratio (5 levels), string I/O.
/// Content is 100% numeric. Baseline: D5, W20, Object-only, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class NumericIsolationStringBench
{
    private string _i100_s = null!; private Node20<double> _i100 = null!;
    private string _i70_s = null!; private Node20<double> _i70 = null!;
    private string _i50_s = null!; private Node20<double> _i50 = null!;
    private string _i30_s = null!; private Node20<double> _i30 = null!;
    private string _f100_s = null!; private Node20<double> _f100 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _i100_s = Load("I100"); _i100 = JsonSerializer.Deserialize<Node20<double>>(_i100_s)!;
        _i70_s = Load("I70"); _i70 = JsonSerializer.Deserialize<Node20<double>>(_i70_s)!;
        _i50_s = Load("I50"); _i50 = JsonSerializer.Deserialize<Node20<double>>(_i50_s)!;
        _i30_s = Load("I30"); _i30 = JsonSerializer.Deserialize<Node20<double>>(_i30_s)!;
        _f100_s = Load("F100"); _f100 = JsonSerializer.Deserialize<Node20<double>>(_f100_s)!;
    }

    private static string Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoNumeric", $"{id}.json");
        return File.ReadAllText(path);
    }

    // ===================== I100 =====================

    [Benchmark, BenchmarkCategory("Deserialize-I100")]
    public Node20<double> STJ_Deser_I100() => JsonSerializer.Deserialize<Node20<double>>(_i100_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-I100")]
    public Node20<double> Newtonsoft_Deser_I100() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(_i100_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-I100")]
    public Node20<double> SpanJson_Deser_I100() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<double>>(_i100_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-I100")]
    public Node20<double> Utf8Json_Deser_I100() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(Encoding.UTF8.GetBytes(_i100_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-I100")]
    public Node20<double> Jil_Deser_I100() => Jil.JSON.Deserialize<Node20<double>>(_i100_s)!;

    [Benchmark, BenchmarkCategory("Serialize-I100")]
    public string STJ_Ser_I100() => JsonSerializer.Serialize(_i100);
    [Benchmark, BenchmarkCategory("Serialize-I100")]
    public string Newtonsoft_Ser_I100() => Newtonsoft.Json.JsonConvert.SerializeObject(_i100);
    [Benchmark, BenchmarkCategory("Serialize-I100")]
    public string SpanJson_Ser_I100() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_i100);
    [Benchmark, BenchmarkCategory("Serialize-I100")]
    public string Utf8Json_Ser_I100() => Utf8Json.JsonSerializer.ToJsonString(_i100);
    [Benchmark, BenchmarkCategory("Serialize-I100")]
    public string Jil_Ser_I100() => Jil.JSON.Serialize(_i100);

    // ===================== I70 =====================

    [Benchmark, BenchmarkCategory("Deserialize-I70")]
    public Node20<double> STJ_Deser_I70() => JsonSerializer.Deserialize<Node20<double>>(_i70_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-I70")]
    public Node20<double> Newtonsoft_Deser_I70() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(_i70_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-I70")]
    public Node20<double> SpanJson_Deser_I70() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<double>>(_i70_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-I70")]
    public Node20<double> Utf8Json_Deser_I70() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(Encoding.UTF8.GetBytes(_i70_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-I70")]
    public Node20<double> Jil_Deser_I70() => Jil.JSON.Deserialize<Node20<double>>(_i70_s)!;

    [Benchmark, BenchmarkCategory("Serialize-I70")]
    public string STJ_Ser_I70() => JsonSerializer.Serialize(_i70);
    [Benchmark, BenchmarkCategory("Serialize-I70")]
    public string Newtonsoft_Ser_I70() => Newtonsoft.Json.JsonConvert.SerializeObject(_i70);
    [Benchmark, BenchmarkCategory("Serialize-I70")]
    public string SpanJson_Ser_I70() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_i70);
    [Benchmark, BenchmarkCategory("Serialize-I70")]
    public string Utf8Json_Ser_I70() => Utf8Json.JsonSerializer.ToJsonString(_i70);
    [Benchmark, BenchmarkCategory("Serialize-I70")]
    public string Jil_Ser_I70() => Jil.JSON.Serialize(_i70);

    // ===================== I50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-I50")]
    public Node20<double> STJ_Deser_I50() => JsonSerializer.Deserialize<Node20<double>>(_i50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-I50")]
    public Node20<double> Newtonsoft_Deser_I50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(_i50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-I50")]
    public Node20<double> SpanJson_Deser_I50() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<double>>(_i50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-I50")]
    public Node20<double> Utf8Json_Deser_I50() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(Encoding.UTF8.GetBytes(_i50_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-I50")]
    public Node20<double> Jil_Deser_I50() => Jil.JSON.Deserialize<Node20<double>>(_i50_s)!;

    [Benchmark, BenchmarkCategory("Serialize-I50")]
    public string STJ_Ser_I50() => JsonSerializer.Serialize(_i50);
    [Benchmark, BenchmarkCategory("Serialize-I50")]
    public string Newtonsoft_Ser_I50() => Newtonsoft.Json.JsonConvert.SerializeObject(_i50);
    [Benchmark, BenchmarkCategory("Serialize-I50")]
    public string SpanJson_Ser_I50() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_i50);
    [Benchmark, BenchmarkCategory("Serialize-I50")]
    public string Utf8Json_Ser_I50() => Utf8Json.JsonSerializer.ToJsonString(_i50);
    [Benchmark, BenchmarkCategory("Serialize-I50")]
    public string Jil_Ser_I50() => Jil.JSON.Serialize(_i50);

    // ===================== I30 =====================

    [Benchmark, BenchmarkCategory("Deserialize-I30")]
    public Node20<double> STJ_Deser_I30() => JsonSerializer.Deserialize<Node20<double>>(_i30_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-I30")]
    public Node20<double> Newtonsoft_Deser_I30() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(_i30_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-I30")]
    public Node20<double> SpanJson_Deser_I30() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<double>>(_i30_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-I30")]
    public Node20<double> Utf8Json_Deser_I30() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(Encoding.UTF8.GetBytes(_i30_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-I30")]
    public Node20<double> Jil_Deser_I30() => Jil.JSON.Deserialize<Node20<double>>(_i30_s)!;

    [Benchmark, BenchmarkCategory("Serialize-I30")]
    public string STJ_Ser_I30() => JsonSerializer.Serialize(_i30);
    [Benchmark, BenchmarkCategory("Serialize-I30")]
    public string Newtonsoft_Ser_I30() => Newtonsoft.Json.JsonConvert.SerializeObject(_i30);
    [Benchmark, BenchmarkCategory("Serialize-I30")]
    public string SpanJson_Ser_I30() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_i30);
    [Benchmark, BenchmarkCategory("Serialize-I30")]
    public string Utf8Json_Ser_I30() => Utf8Json.JsonSerializer.ToJsonString(_i30);
    [Benchmark, BenchmarkCategory("Serialize-I30")]
    public string Jil_Ser_I30() => Jil.JSON.Serialize(_i30);

    // ===================== F100 =====================

    [Benchmark, BenchmarkCategory("Deserialize-F100")]
    public Node20<double> STJ_Deser_F100() => JsonSerializer.Deserialize<Node20<double>>(_f100_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-F100")]
    public Node20<double> Newtonsoft_Deser_F100() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(_f100_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-F100")]
    public Node20<double> SpanJson_Deser_F100() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<double>>(_f100_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-F100")]
    public Node20<double> Utf8Json_Deser_F100() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(Encoding.UTF8.GetBytes(_f100_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-F100")]
    public Node20<double> Jil_Deser_F100() => Jil.JSON.Deserialize<Node20<double>>(_f100_s)!;

    [Benchmark, BenchmarkCategory("Serialize-F100")]
    public string STJ_Ser_F100() => JsonSerializer.Serialize(_f100);
    [Benchmark, BenchmarkCategory("Serialize-F100")]
    public string Newtonsoft_Ser_F100() => Newtonsoft.Json.JsonConvert.SerializeObject(_f100);
    [Benchmark, BenchmarkCategory("Serialize-F100")]
    public string SpanJson_Ser_F100() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_f100);
    [Benchmark, BenchmarkCategory("Serialize-F100")]
    public string Utf8Json_Ser_F100() => Utf8Json.JsonSerializer.ToJsonString(_f100);
    [Benchmark, BenchmarkCategory("Serialize-F100")]
    public string Jil_Ser_F100() => Jil.JSON.Serialize(_f100);
}
