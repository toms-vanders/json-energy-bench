using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Unicode isolation benchmark: varies literal Unicode density (5 levels), string I/O.
/// Baseline: D5, W20, Textual, Object-only, ASCII, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class UnicodeIsolationStringBench
{
    private string _u0_s = null!; private Node20<string> _u0 = null!;
    private string _u10_s = null!; private Node20<string> _u10 = null!;
    private string _u30_s = null!; private Node20<string> _u30 = null!;
    private string _u50_s = null!; private Node20<string> _u50 = null!;
    private string _u70_s = null!; private Node20<string> _u70 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _u0_s = Load("U0"); _u0 = JsonSerializer.Deserialize<Node20<string>>(_u0_s)!;
        _u10_s = Load("U10"); _u10 = JsonSerializer.Deserialize<Node20<string>>(_u10_s)!;
        _u30_s = Load("U30"); _u30 = JsonSerializer.Deserialize<Node20<string>>(_u30_s)!;
        _u50_s = Load("U50"); _u50 = JsonSerializer.Deserialize<Node20<string>>(_u50_s)!;
        _u70_s = Load("U70"); _u70 = JsonSerializer.Deserialize<Node20<string>>(_u70_s)!;
    }

    private static string Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoUnicode", $"{id}.json");
        return File.ReadAllText(path);
    }

    // ===================== U0 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U0")]
    public Node20<string> STJ_Deser_U0() => JsonSerializer.Deserialize<Node20<string>>(_u0_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U0")]
    public Node20<string> Newtonsoft_Deser_U0() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_u0_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U0")]
    public Node20<string> SpanJson_Deser_U0() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_u0_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U0")]
    public Node20<string> Utf8Json_Deser_U0() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_u0_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-U0")]
    public Node20<string> Jil_Deser_U0() => Jil.JSON.Deserialize<Node20<string>>(_u0_s)!;

    [Benchmark, BenchmarkCategory("Serialize-U0")]
    public string STJ_Ser_U0() => JsonSerializer.Serialize(_u0);
    [Benchmark, BenchmarkCategory("Serialize-U0")]
    public string Newtonsoft_Ser_U0() => Newtonsoft.Json.JsonConvert.SerializeObject(_u0);
    [Benchmark, BenchmarkCategory("Serialize-U0")]
    public string SpanJson_Ser_U0() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_u0);
    [Benchmark, BenchmarkCategory("Serialize-U0")]
    public string Utf8Json_Ser_U0() => Utf8Json.JsonSerializer.ToJsonString(_u0);
    [Benchmark, BenchmarkCategory("Serialize-U0")]
    public string Jil_Ser_U0() => Jil.JSON.Serialize(_u0);

    // ===================== U10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> STJ_Deser_U10() => JsonSerializer.Deserialize<Node20<string>>(_u10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> Newtonsoft_Deser_U10() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_u10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> SpanJson_Deser_U10() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_u10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> Utf8Json_Deser_U10() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_u10_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-U10")]
    public Node20<string> Jil_Deser_U10() => Jil.JSON.Deserialize<Node20<string>>(_u10_s)!;

    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public string STJ_Ser_U10() => JsonSerializer.Serialize(_u10);
    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public string Newtonsoft_Ser_U10() => Newtonsoft.Json.JsonConvert.SerializeObject(_u10);
    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public string SpanJson_Ser_U10() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_u10);
    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public string Utf8Json_Ser_U10() => Utf8Json.JsonSerializer.ToJsonString(_u10);
    [Benchmark, BenchmarkCategory("Serialize-U10")]
    public string Jil_Ser_U10() => Jil.JSON.Serialize(_u10);

    // ===================== U30 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U30")]
    public Node20<string> STJ_Deser_U30() => JsonSerializer.Deserialize<Node20<string>>(_u30_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U30")]
    public Node20<string> Newtonsoft_Deser_U30() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_u30_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U30")]
    public Node20<string> SpanJson_Deser_U30() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_u30_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U30")]
    public Node20<string> Utf8Json_Deser_U30() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_u30_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-U30")]
    public Node20<string> Jil_Deser_U30() => Jil.JSON.Deserialize<Node20<string>>(_u30_s)!;

    [Benchmark, BenchmarkCategory("Serialize-U30")]
    public string STJ_Ser_U30() => JsonSerializer.Serialize(_u30);
    [Benchmark, BenchmarkCategory("Serialize-U30")]
    public string Newtonsoft_Ser_U30() => Newtonsoft.Json.JsonConvert.SerializeObject(_u30);
    [Benchmark, BenchmarkCategory("Serialize-U30")]
    public string SpanJson_Ser_U30() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_u30);
    [Benchmark, BenchmarkCategory("Serialize-U30")]
    public string Utf8Json_Ser_U30() => Utf8Json.JsonSerializer.ToJsonString(_u30);
    [Benchmark, BenchmarkCategory("Serialize-U30")]
    public string Jil_Ser_U30() => Jil.JSON.Serialize(_u30);

    // ===================== U50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> STJ_Deser_U50() => JsonSerializer.Deserialize<Node20<string>>(_u50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> Newtonsoft_Deser_U50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_u50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> SpanJson_Deser_U50() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_u50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> Utf8Json_Deser_U50() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_u50_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-U50")]
    public Node20<string> Jil_Deser_U50() => Jil.JSON.Deserialize<Node20<string>>(_u50_s)!;

    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public string STJ_Ser_U50() => JsonSerializer.Serialize(_u50);
    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public string Newtonsoft_Ser_U50() => Newtonsoft.Json.JsonConvert.SerializeObject(_u50);
    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public string SpanJson_Ser_U50() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_u50);
    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public string Utf8Json_Ser_U50() => Utf8Json.JsonSerializer.ToJsonString(_u50);
    [Benchmark, BenchmarkCategory("Serialize-U50")]
    public string Jil_Ser_U50() => Jil.JSON.Serialize(_u50);

    // ===================== U70 =====================

    [Benchmark, BenchmarkCategory("Deserialize-U70")]
    public Node20<string> STJ_Deser_U70() => JsonSerializer.Deserialize<Node20<string>>(_u70_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U70")]
    public Node20<string> Newtonsoft_Deser_U70() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_u70_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U70")]
    public Node20<string> SpanJson_Deser_U70() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_u70_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-U70")]
    public Node20<string> Utf8Json_Deser_U70() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_u70_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-U70")]
    public Node20<string> Jil_Deser_U70() => Jil.JSON.Deserialize<Node20<string>>(_u70_s)!;

    [Benchmark, BenchmarkCategory("Serialize-U70")]
    public string STJ_Ser_U70() => JsonSerializer.Serialize(_u70);
    [Benchmark, BenchmarkCategory("Serialize-U70")]
    public string Newtonsoft_Ser_U70() => Newtonsoft.Json.JsonConvert.SerializeObject(_u70);
    [Benchmark, BenchmarkCategory("Serialize-U70")]
    public string SpanJson_Ser_U70() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_u70);
    [Benchmark, BenchmarkCategory("Serialize-U70")]
    public string Utf8Json_Ser_U70() => Utf8Json.JsonSerializer.ToJsonString(_u70);
    [Benchmark, BenchmarkCategory("Serialize-U70")]
    public string Jil_Ser_U70() => Jil.JSON.Serialize(_u70);
}
