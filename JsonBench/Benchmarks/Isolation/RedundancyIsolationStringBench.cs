using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Redundancy isolation benchmark: varies duplicate value ratio (5 levels), string I/O.
/// Baseline: D5, W20, Textual, Object-only, ASCII
/// </summary>
[Config(typeof(BenchConfig))]
public class RedundancyIsolationStringBench
{
    private string _r0_s = null!; private Node20<string> _r0 = null!;
    private string _r25_s = null!; private Node20<string> _r25 = null!;
    private string _r50_s = null!; private Node20<string> _r50 = null!;
    private string _r75_s = null!; private Node20<string> _r75 = null!;
    private string _r95_s = null!; private Node20<string> _r95 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _r0_s = Load("R0"); _r0 = JsonSerializer.Deserialize<Node20<string>>(_r0_s)!;
        _r25_s = Load("R25"); _r25 = JsonSerializer.Deserialize<Node20<string>>(_r25_s)!;
        _r50_s = Load("R50"); _r50 = JsonSerializer.Deserialize<Node20<string>>(_r50_s)!;
        _r75_s = Load("R75"); _r75 = JsonSerializer.Deserialize<Node20<string>>(_r75_s)!;
        _r95_s = Load("R95"); _r95 = JsonSerializer.Deserialize<Node20<string>>(_r95_s)!;
    }

    private static string Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoRedundancy", $"{id}.json");
        return File.ReadAllText(path);
    }

    // ===================== R0 =====================

    [Benchmark, BenchmarkCategory("Deserialize-R0")]
    public Node20<string> STJ_Deser_R0() => JsonSerializer.Deserialize<Node20<string>>(_r0_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R0")]
    public Node20<string> Newtonsoft_Deser_R0() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_r0_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R0")]
    public Node20<string> SpanJson_Deser_R0() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_r0_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R0")]
    public Node20<string> Utf8Json_Deser_R0() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_r0_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-R0")]
    public Node20<string> Jil_Deser_R0() => Jil.JSON.Deserialize<Node20<string>>(_r0_s)!;

    [Benchmark, BenchmarkCategory("Serialize-R0")]
    public string STJ_Ser_R0() => JsonSerializer.Serialize(_r0);
    [Benchmark, BenchmarkCategory("Serialize-R0")]
    public string Newtonsoft_Ser_R0() => Newtonsoft.Json.JsonConvert.SerializeObject(_r0);
    [Benchmark, BenchmarkCategory("Serialize-R0")]
    public string SpanJson_Ser_R0() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_r0);
    [Benchmark, BenchmarkCategory("Serialize-R0")]
    public string Utf8Json_Ser_R0() => Utf8Json.JsonSerializer.ToJsonString(_r0);
    [Benchmark, BenchmarkCategory("Serialize-R0")]
    public string Jil_Ser_R0() => Jil.JSON.Serialize(_r0);

    // ===================== R25 =====================

    [Benchmark, BenchmarkCategory("Deserialize-R25")]
    public Node20<string> STJ_Deser_R25() => JsonSerializer.Deserialize<Node20<string>>(_r25_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R25")]
    public Node20<string> Newtonsoft_Deser_R25() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_r25_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R25")]
    public Node20<string> SpanJson_Deser_R25() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_r25_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R25")]
    public Node20<string> Utf8Json_Deser_R25() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_r25_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-R25")]
    public Node20<string> Jil_Deser_R25() => Jil.JSON.Deserialize<Node20<string>>(_r25_s)!;

    [Benchmark, BenchmarkCategory("Serialize-R25")]
    public string STJ_Ser_R25() => JsonSerializer.Serialize(_r25);
    [Benchmark, BenchmarkCategory("Serialize-R25")]
    public string Newtonsoft_Ser_R25() => Newtonsoft.Json.JsonConvert.SerializeObject(_r25);
    [Benchmark, BenchmarkCategory("Serialize-R25")]
    public string SpanJson_Ser_R25() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_r25);
    [Benchmark, BenchmarkCategory("Serialize-R25")]
    public string Utf8Json_Ser_R25() => Utf8Json.JsonSerializer.ToJsonString(_r25);
    [Benchmark, BenchmarkCategory("Serialize-R25")]
    public string Jil_Ser_R25() => Jil.JSON.Serialize(_r25);

    // ===================== R50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-R50")]
    public Node20<string> STJ_Deser_R50() => JsonSerializer.Deserialize<Node20<string>>(_r50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R50")]
    public Node20<string> Newtonsoft_Deser_R50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_r50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R50")]
    public Node20<string> SpanJson_Deser_R50() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_r50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R50")]
    public Node20<string> Utf8Json_Deser_R50() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_r50_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-R50")]
    public Node20<string> Jil_Deser_R50() => Jil.JSON.Deserialize<Node20<string>>(_r50_s)!;

    [Benchmark, BenchmarkCategory("Serialize-R50")]
    public string STJ_Ser_R50() => JsonSerializer.Serialize(_r50);
    [Benchmark, BenchmarkCategory("Serialize-R50")]
    public string Newtonsoft_Ser_R50() => Newtonsoft.Json.JsonConvert.SerializeObject(_r50);
    [Benchmark, BenchmarkCategory("Serialize-R50")]
    public string SpanJson_Ser_R50() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_r50);
    [Benchmark, BenchmarkCategory("Serialize-R50")]
    public string Utf8Json_Ser_R50() => Utf8Json.JsonSerializer.ToJsonString(_r50);
    [Benchmark, BenchmarkCategory("Serialize-R50")]
    public string Jil_Ser_R50() => Jil.JSON.Serialize(_r50);

    // ===================== R75 =====================

    [Benchmark, BenchmarkCategory("Deserialize-R75")]
    public Node20<string> STJ_Deser_R75() => JsonSerializer.Deserialize<Node20<string>>(_r75_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R75")]
    public Node20<string> Newtonsoft_Deser_R75() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_r75_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R75")]
    public Node20<string> SpanJson_Deser_R75() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_r75_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R75")]
    public Node20<string> Utf8Json_Deser_R75() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_r75_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-R75")]
    public Node20<string> Jil_Deser_R75() => Jil.JSON.Deserialize<Node20<string>>(_r75_s)!;

    [Benchmark, BenchmarkCategory("Serialize-R75")]
    public string STJ_Ser_R75() => JsonSerializer.Serialize(_r75);
    [Benchmark, BenchmarkCategory("Serialize-R75")]
    public string Newtonsoft_Ser_R75() => Newtonsoft.Json.JsonConvert.SerializeObject(_r75);
    [Benchmark, BenchmarkCategory("Serialize-R75")]
    public string SpanJson_Ser_R75() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_r75);
    [Benchmark, BenchmarkCategory("Serialize-R75")]
    public string Utf8Json_Ser_R75() => Utf8Json.JsonSerializer.ToJsonString(_r75);
    [Benchmark, BenchmarkCategory("Serialize-R75")]
    public string Jil_Ser_R75() => Jil.JSON.Serialize(_r75);

    // ===================== R95 =====================

    [Benchmark, BenchmarkCategory("Deserialize-R95")]
    public Node20<string> STJ_Deser_R95() => JsonSerializer.Deserialize<Node20<string>>(_r95_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R95")]
    public Node20<string> Newtonsoft_Deser_R95() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_r95_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R95")]
    public Node20<string> SpanJson_Deser_R95() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_r95_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-R95")]
    public Node20<string> Utf8Json_Deser_R95() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_r95_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-R95")]
    public Node20<string> Jil_Deser_R95() => Jil.JSON.Deserialize<Node20<string>>(_r95_s)!;

    [Benchmark, BenchmarkCategory("Serialize-R95")]
    public string STJ_Ser_R95() => JsonSerializer.Serialize(_r95);
    [Benchmark, BenchmarkCategory("Serialize-R95")]
    public string Newtonsoft_Ser_R95() => Newtonsoft.Json.JsonConvert.SerializeObject(_r95);
    [Benchmark, BenchmarkCategory("Serialize-R95")]
    public string SpanJson_Ser_R95() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_r95);
    [Benchmark, BenchmarkCategory("Serialize-R95")]
    public string Utf8Json_Ser_R95() => Utf8Json.JsonSerializer.ToJsonString(_r95);
    [Benchmark, BenchmarkCategory("Serialize-R95")]
    public string Jil_Ser_R95() => Jil.JSON.Serialize(_r95);
}
