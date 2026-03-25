using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Escape isolation benchmark: varies simple escape density (5 levels), string I/O.
/// Baseline: D5, W20, Textual, Object-only, ASCII, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class EscapeIsolationStringBench
{
    private string _e0_s = null!; private Node20<string> _e0 = null!;
    private string _e10_s = null!; private Node20<string> _e10 = null!;
    private string _e30_s = null!; private Node20<string> _e30 = null!;
    private string _e50_s = null!; private Node20<string> _e50 = null!;
    private string _e70_s = null!; private Node20<string> _e70 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _e0_s = Load("E0"); _e0 = JsonSerializer.Deserialize<Node20<string>>(_e0_s)!;
        _e10_s = Load("E10"); _e10 = JsonSerializer.Deserialize<Node20<string>>(_e10_s)!;
        _e30_s = Load("E30"); _e30 = JsonSerializer.Deserialize<Node20<string>>(_e30_s)!;
        _e50_s = Load("E50"); _e50 = JsonSerializer.Deserialize<Node20<string>>(_e50_s)!;
        _e70_s = Load("E70"); _e70 = JsonSerializer.Deserialize<Node20<string>>(_e70_s)!;
    }

    private static string Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoEscape", $"{id}.json");
        return File.ReadAllText(path);
    }

    // ===================== E0 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E0")]
    public Node20<string> STJ_Deser_E0() => JsonSerializer.Deserialize<Node20<string>>(_e0_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E0")]
    public Node20<string> Newtonsoft_Deser_E0() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_e0_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E0")]
    public Node20<string> SpanJson_Deser_E0() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_e0_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E0")]
    public Node20<string> Utf8Json_Deser_E0() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_e0_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-E0")]
    public Node20<string> Jil_Deser_E0() => Jil.JSON.Deserialize<Node20<string>>(_e0_s)!;

    [Benchmark, BenchmarkCategory("Serialize-E0")]
    public string STJ_Ser_E0() => JsonSerializer.Serialize(_e0);
    [Benchmark, BenchmarkCategory("Serialize-E0")]
    public string Newtonsoft_Ser_E0() => Newtonsoft.Json.JsonConvert.SerializeObject(_e0);
    [Benchmark, BenchmarkCategory("Serialize-E0")]
    public string SpanJson_Ser_E0() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_e0);
    [Benchmark, BenchmarkCategory("Serialize-E0")]
    public string Utf8Json_Ser_E0() => Utf8Json.JsonSerializer.ToJsonString(_e0);
    [Benchmark, BenchmarkCategory("Serialize-E0")]
    public string Jil_Ser_E0() => Jil.JSON.Serialize(_e0);

    // ===================== E10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> STJ_Deser_E10() => JsonSerializer.Deserialize<Node20<string>>(_e10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> Newtonsoft_Deser_E10() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_e10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> SpanJson_Deser_E10() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_e10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> Utf8Json_Deser_E10() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_e10_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-E10")]
    public Node20<string> Jil_Deser_E10() => Jil.JSON.Deserialize<Node20<string>>(_e10_s)!;

    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public string STJ_Ser_E10() => JsonSerializer.Serialize(_e10);
    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public string Newtonsoft_Ser_E10() => Newtonsoft.Json.JsonConvert.SerializeObject(_e10);
    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public string SpanJson_Ser_E10() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_e10);
    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public string Utf8Json_Ser_E10() => Utf8Json.JsonSerializer.ToJsonString(_e10);
    [Benchmark, BenchmarkCategory("Serialize-E10")]
    public string Jil_Ser_E10() => Jil.JSON.Serialize(_e10);

    // ===================== E30 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E30")]
    public Node20<string> STJ_Deser_E30() => JsonSerializer.Deserialize<Node20<string>>(_e30_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E30")]
    public Node20<string> Newtonsoft_Deser_E30() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_e30_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E30")]
    public Node20<string> SpanJson_Deser_E30() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_e30_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E30")]
    public Node20<string> Utf8Json_Deser_E30() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_e30_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-E30")]
    public Node20<string> Jil_Deser_E30() => Jil.JSON.Deserialize<Node20<string>>(_e30_s)!;

    [Benchmark, BenchmarkCategory("Serialize-E30")]
    public string STJ_Ser_E30() => JsonSerializer.Serialize(_e30);
    [Benchmark, BenchmarkCategory("Serialize-E30")]
    public string Newtonsoft_Ser_E30() => Newtonsoft.Json.JsonConvert.SerializeObject(_e30);
    [Benchmark, BenchmarkCategory("Serialize-E30")]
    public string SpanJson_Ser_E30() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_e30);
    [Benchmark, BenchmarkCategory("Serialize-E30")]
    public string Utf8Json_Ser_E30() => Utf8Json.JsonSerializer.ToJsonString(_e30);
    [Benchmark, BenchmarkCategory("Serialize-E30")]
    public string Jil_Ser_E30() => Jil.JSON.Serialize(_e30);

    // ===================== E50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> STJ_Deser_E50() => JsonSerializer.Deserialize<Node20<string>>(_e50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> Newtonsoft_Deser_E50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_e50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> SpanJson_Deser_E50() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_e50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> Utf8Json_Deser_E50() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_e50_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-E50")]
    public Node20<string> Jil_Deser_E50() => Jil.JSON.Deserialize<Node20<string>>(_e50_s)!;

    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public string STJ_Ser_E50() => JsonSerializer.Serialize(_e50);
    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public string Newtonsoft_Ser_E50() => Newtonsoft.Json.JsonConvert.SerializeObject(_e50);
    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public string SpanJson_Ser_E50() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_e50);
    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public string Utf8Json_Ser_E50() => Utf8Json.JsonSerializer.ToJsonString(_e50);
    [Benchmark, BenchmarkCategory("Serialize-E50")]
    public string Jil_Ser_E50() => Jil.JSON.Serialize(_e50);

    // ===================== E70 =====================

    [Benchmark, BenchmarkCategory("Deserialize-E70")]
    public Node20<string> STJ_Deser_E70() => JsonSerializer.Deserialize<Node20<string>>(_e70_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E70")]
    public Node20<string> Newtonsoft_Deser_E70() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_e70_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E70")]
    public Node20<string> SpanJson_Deser_E70() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_e70_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-E70")]
    public Node20<string> Utf8Json_Deser_E70() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_e70_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-E70")]
    public Node20<string> Jil_Deser_E70() => Jil.JSON.Deserialize<Node20<string>>(_e70_s)!;

    [Benchmark, BenchmarkCategory("Serialize-E70")]
    public string STJ_Ser_E70() => JsonSerializer.Serialize(_e70);
    [Benchmark, BenchmarkCategory("Serialize-E70")]
    public string Newtonsoft_Ser_E70() => Newtonsoft.Json.JsonConvert.SerializeObject(_e70);
    [Benchmark, BenchmarkCategory("Serialize-E70")]
    public string SpanJson_Ser_E70() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_e70);
    [Benchmark, BenchmarkCategory("Serialize-E70")]
    public string Utf8Json_Ser_E70() => Utf8Json.JsonSerializer.ToJsonString(_e70);
    [Benchmark, BenchmarkCategory("Serialize-E70")]
    public string Jil_Ser_E70() => Jil.JSON.Serialize(_e70);
}
