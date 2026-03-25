using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Depth isolation benchmark: varies depth (7 levels), string I/O.
/// Baseline: W20, Textual, Object-only, ASCII, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class DepthIsolationStringBench
{
    private string _d1_s = null!; private Node20<string> _d1 = null!;
    private string _d2_s = null!; private Node20<string> _d2 = null!;
    private string _d4_s = null!; private Node20<string> _d4 = null!;
    private string _d8_s = null!; private Node20<string> _d8 = null!;
    private string _d15_s = null!; private Node20<string> _d15 = null!;
    private string _d25_s = null!; private Node20<string> _d25 = null!;
    private string _d40_s = null!; private Node20<string> _d40 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _d1_s = Load("D1"); _d1 = JsonSerializer.Deserialize<Node20<string>>(_d1_s)!;
        _d2_s = Load("D2"); _d2 = JsonSerializer.Deserialize<Node20<string>>(_d2_s)!;
        _d4_s = Load("D4"); _d4 = JsonSerializer.Deserialize<Node20<string>>(_d4_s)!;
        _d8_s = Load("D8"); _d8 = JsonSerializer.Deserialize<Node20<string>>(_d8_s)!;
        _d15_s = Load("D15"); _d15 = JsonSerializer.Deserialize<Node20<string>>(_d15_s)!;
        _d25_s = Load("D25"); _d25 = JsonSerializer.Deserialize<Node20<string>>(_d25_s)!;
        _d40_s = Load("D40"); _d40 = JsonSerializer.Deserialize<Node20<string>>(_d40_s)!;
    }

    private static string Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoDepth", $"{id}.json");
        return File.ReadAllText(path);
    }

    // ===================== D1 =====================

    [Benchmark, BenchmarkCategory("Deserialize-D1")]
    public Node20<string> STJ_Deser_D1() => JsonSerializer.Deserialize<Node20<string>>(_d1_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D1")]
    public Node20<string> Newtonsoft_Deser_D1() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_d1_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D1")]
    public Node20<string> SpanJson_Deser_D1() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_d1_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D1")]
    public Node20<string> Utf8Json_Deser_D1() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_d1_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D1")]
    public Node20<string> Jil_Deser_D1() => Jil.JSON.Deserialize<Node20<string>>(_d1_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D1")]
    public string STJ_Ser_D1() => JsonSerializer.Serialize(_d1);
    [Benchmark, BenchmarkCategory("Serialize-D1")]
    public string Newtonsoft_Ser_D1() => Newtonsoft.Json.JsonConvert.SerializeObject(_d1);
    [Benchmark, BenchmarkCategory("Serialize-D1")]
    public string SpanJson_Ser_D1() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d1);
    [Benchmark, BenchmarkCategory("Serialize-D1")]
    public string Utf8Json_Ser_D1() => Utf8Json.JsonSerializer.ToJsonString(_d1);
    [Benchmark, BenchmarkCategory("Serialize-D1")]
    public string Jil_Ser_D1() => Jil.JSON.Serialize(_d1);

    // ===================== D2 =====================

    [Benchmark, BenchmarkCategory("Deserialize-D2")]
    public Node20<string> STJ_Deser_D2() => JsonSerializer.Deserialize<Node20<string>>(_d2_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2")]
    public Node20<string> Newtonsoft_Deser_D2() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_d2_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2")]
    public Node20<string> SpanJson_Deser_D2() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_d2_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2")]
    public Node20<string> Utf8Json_Deser_D2() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_d2_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D2")]
    public Node20<string> Jil_Deser_D2() => Jil.JSON.Deserialize<Node20<string>>(_d2_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2")]
    public string STJ_Ser_D2() => JsonSerializer.Serialize(_d2);
    [Benchmark, BenchmarkCategory("Serialize-D2")]
    public string Newtonsoft_Ser_D2() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2);
    [Benchmark, BenchmarkCategory("Serialize-D2")]
    public string SpanJson_Ser_D2() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2);
    [Benchmark, BenchmarkCategory("Serialize-D2")]
    public string Utf8Json_Ser_D2() => Utf8Json.JsonSerializer.ToJsonString(_d2);
    [Benchmark, BenchmarkCategory("Serialize-D2")]
    public string Jil_Ser_D2() => Jil.JSON.Serialize(_d2);

    // ===================== D4 =====================

    [Benchmark, BenchmarkCategory("Deserialize-D4")]
    public Node20<string> STJ_Deser_D4() => JsonSerializer.Deserialize<Node20<string>>(_d4_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D4")]
    public Node20<string> Newtonsoft_Deser_D4() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_d4_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D4")]
    public Node20<string> SpanJson_Deser_D4() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_d4_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D4")]
    public Node20<string> Utf8Json_Deser_D4() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_d4_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D4")]
    public Node20<string> Jil_Deser_D4() => Jil.JSON.Deserialize<Node20<string>>(_d4_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D4")]
    public string STJ_Ser_D4() => JsonSerializer.Serialize(_d4);
    [Benchmark, BenchmarkCategory("Serialize-D4")]
    public string Newtonsoft_Ser_D4() => Newtonsoft.Json.JsonConvert.SerializeObject(_d4);
    [Benchmark, BenchmarkCategory("Serialize-D4")]
    public string SpanJson_Ser_D4() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d4);
    [Benchmark, BenchmarkCategory("Serialize-D4")]
    public string Utf8Json_Ser_D4() => Utf8Json.JsonSerializer.ToJsonString(_d4);
    [Benchmark, BenchmarkCategory("Serialize-D4")]
    public string Jil_Ser_D4() => Jil.JSON.Serialize(_d4);

    // ===================== D8 =====================

    [Benchmark, BenchmarkCategory("Deserialize-D8")]
    public Node20<string> STJ_Deser_D8() => JsonSerializer.Deserialize<Node20<string>>(_d8_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D8")]
    public Node20<string> Newtonsoft_Deser_D8() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_d8_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D8")]
    public Node20<string> SpanJson_Deser_D8() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_d8_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D8")]
    public Node20<string> Utf8Json_Deser_D8() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_d8_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D8")]
    public Node20<string> Jil_Deser_D8() => Jil.JSON.Deserialize<Node20<string>>(_d8_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D8")]
    public string STJ_Ser_D8() => JsonSerializer.Serialize(_d8);
    [Benchmark, BenchmarkCategory("Serialize-D8")]
    public string Newtonsoft_Ser_D8() => Newtonsoft.Json.JsonConvert.SerializeObject(_d8);
    [Benchmark, BenchmarkCategory("Serialize-D8")]
    public string SpanJson_Ser_D8() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d8);
    [Benchmark, BenchmarkCategory("Serialize-D8")]
    public string Utf8Json_Ser_D8() => Utf8Json.JsonSerializer.ToJsonString(_d8);
    [Benchmark, BenchmarkCategory("Serialize-D8")]
    public string Jil_Ser_D8() => Jil.JSON.Serialize(_d8);

    // ===================== D15 =====================

    [Benchmark, BenchmarkCategory("Deserialize-D15")]
    public Node20<string> STJ_Deser_D15() => JsonSerializer.Deserialize<Node20<string>>(_d15_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D15")]
    public Node20<string> Newtonsoft_Deser_D15() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_d15_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D15")]
    public Node20<string> SpanJson_Deser_D15() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_d15_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D15")]
    public Node20<string> Utf8Json_Deser_D15() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_d15_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D15")]
    public Node20<string> Jil_Deser_D15() => Jil.JSON.Deserialize<Node20<string>>(_d15_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D15")]
    public string STJ_Ser_D15() => JsonSerializer.Serialize(_d15);
    [Benchmark, BenchmarkCategory("Serialize-D15")]
    public string Newtonsoft_Ser_D15() => Newtonsoft.Json.JsonConvert.SerializeObject(_d15);
    [Benchmark, BenchmarkCategory("Serialize-D15")]
    public string SpanJson_Ser_D15() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d15);
    [Benchmark, BenchmarkCategory("Serialize-D15")]
    public string Utf8Json_Ser_D15() => Utf8Json.JsonSerializer.ToJsonString(_d15);
    [Benchmark, BenchmarkCategory("Serialize-D15")]
    public string Jil_Ser_D15() => Jil.JSON.Serialize(_d15);

    // ===================== D25 =====================

    [Benchmark, BenchmarkCategory("Deserialize-D25")]
    public Node20<string> STJ_Deser_D25() => JsonSerializer.Deserialize<Node20<string>>(_d25_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D25")]
    public Node20<string> Newtonsoft_Deser_D25() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_d25_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D25")]
    public Node20<string> SpanJson_Deser_D25() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_d25_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D25")]
    public Node20<string> Utf8Json_Deser_D25() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_d25_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D25")]
    public Node20<string> Jil_Deser_D25() => Jil.JSON.Deserialize<Node20<string>>(_d25_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D25")]
    public string STJ_Ser_D25() => JsonSerializer.Serialize(_d25);
    [Benchmark, BenchmarkCategory("Serialize-D25")]
    public string Newtonsoft_Ser_D25() => Newtonsoft.Json.JsonConvert.SerializeObject(_d25);
    [Benchmark, BenchmarkCategory("Serialize-D25")]
    public string SpanJson_Ser_D25() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d25);
    [Benchmark, BenchmarkCategory("Serialize-D25")]
    public string Utf8Json_Ser_D25() => Utf8Json.JsonSerializer.ToJsonString(_d25);
    [Benchmark, BenchmarkCategory("Serialize-D25")]
    public string Jil_Ser_D25() => Jil.JSON.Serialize(_d25);

    // ===================== D40 =====================

    [Benchmark, BenchmarkCategory("Deserialize-D40")]
    public Node20<string> STJ_Deser_D40() => JsonSerializer.Deserialize<Node20<string>>(_d40_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D40")]
    public Node20<string> Newtonsoft_Deser_D40() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_d40_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D40")]
    public Node20<string> SpanJson_Deser_D40() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_d40_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D40")]
    public Node20<string> Utf8Json_Deser_D40() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_d40_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D40")]
    public Node20<string> Jil_Deser_D40() => Jil.JSON.Deserialize<Node20<string>>(_d40_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D40")]
    public string STJ_Ser_D40() => JsonSerializer.Serialize(_d40);
    [Benchmark, BenchmarkCategory("Serialize-D40")]
    public string Newtonsoft_Ser_D40() => Newtonsoft.Json.JsonConvert.SerializeObject(_d40);
    [Benchmark, BenchmarkCategory("Serialize-D40")]
    public string SpanJson_Ser_D40() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d40);
    [Benchmark, BenchmarkCategory("Serialize-D40")]
    public string Utf8Json_Ser_D40() => Utf8Json.JsonSerializer.ToJsonString(_d40);
    [Benchmark, BenchmarkCategory("Serialize-D40")]
    public string Jil_Ser_D40() => Jil.JSON.Serialize(_d40);
}
