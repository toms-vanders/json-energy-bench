using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Unicode escape isolation benchmark: varies \uXXXX escape density (5 levels), string I/O.
/// Baseline: D5, W20, Textual, Object-only, ASCII, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class UnicodeEscapeIsolationStringBench
{
    private string _ue0_s = null!; private Node20<string> _ue0 = null!;
    private string _ue10_s = null!; private Node20<string> _ue10 = null!;
    private string _ue30_s = null!; private Node20<string> _ue30 = null!;
    private string _ue50_s = null!; private Node20<string> _ue50 = null!;
    private string _ue70_s = null!; private Node20<string> _ue70 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _ue0_s = Load("UE0"); _ue0 = JsonSerializer.Deserialize<Node20<string>>(_ue0_s)!;
        _ue10_s = Load("UE10"); _ue10 = JsonSerializer.Deserialize<Node20<string>>(_ue10_s)!;
        _ue30_s = Load("UE30"); _ue30 = JsonSerializer.Deserialize<Node20<string>>(_ue30_s)!;
        _ue50_s = Load("UE50"); _ue50 = JsonSerializer.Deserialize<Node20<string>>(_ue50_s)!;
        _ue70_s = Load("UE70"); _ue70 = JsonSerializer.Deserialize<Node20<string>>(_ue70_s)!;
    }

    private static string Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoUnicodeEscape", $"{id}.json");
        return File.ReadAllText(path);
    }

    // ===================== UE0 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE0")]
    public Node20<string> STJ_Deser_UE0() => JsonSerializer.Deserialize<Node20<string>>(_ue0_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE0")]
    public Node20<string> Newtonsoft_Deser_UE0() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_ue0_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE0")]
    public Node20<string> SpanJson_Deser_UE0() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_ue0_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE0")]
    public Node20<string> Utf8Json_Deser_UE0() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_ue0_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE0")]
    public Node20<string> Jil_Deser_UE0() => Jil.JSON.Deserialize<Node20<string>>(_ue0_s)!;

    [Benchmark, BenchmarkCategory("Serialize-UE0")]
    public string STJ_Ser_UE0() => JsonSerializer.Serialize(_ue0);
    [Benchmark, BenchmarkCategory("Serialize-UE0")]
    public string Newtonsoft_Ser_UE0() => Newtonsoft.Json.JsonConvert.SerializeObject(_ue0);
    [Benchmark, BenchmarkCategory("Serialize-UE0")]
    public string SpanJson_Ser_UE0() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_ue0);
    [Benchmark, BenchmarkCategory("Serialize-UE0")]
    public string Utf8Json_Ser_UE0() => Utf8Json.JsonSerializer.ToJsonString(_ue0);
    [Benchmark, BenchmarkCategory("Serialize-UE0")]
    public string Jil_Ser_UE0() => Jil.JSON.Serialize(_ue0);

    // ===================== UE10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> STJ_Deser_UE10() => JsonSerializer.Deserialize<Node20<string>>(_ue10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> Newtonsoft_Deser_UE10() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_ue10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> SpanJson_Deser_UE10() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_ue10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> Utf8Json_Deser_UE10() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_ue10_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> Jil_Deser_UE10() => Jil.JSON.Deserialize<Node20<string>>(_ue10_s)!;

    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public string STJ_Ser_UE10() => JsonSerializer.Serialize(_ue10);
    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public string Newtonsoft_Ser_UE10() => Newtonsoft.Json.JsonConvert.SerializeObject(_ue10);
    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public string SpanJson_Ser_UE10() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_ue10);
    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public string Utf8Json_Ser_UE10() => Utf8Json.JsonSerializer.ToJsonString(_ue10);
    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public string Jil_Ser_UE10() => Jil.JSON.Serialize(_ue10);

    // ===================== UE30 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE30")]
    public Node20<string> STJ_Deser_UE30() => JsonSerializer.Deserialize<Node20<string>>(_ue30_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE30")]
    public Node20<string> Newtonsoft_Deser_UE30() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_ue30_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE30")]
    public Node20<string> SpanJson_Deser_UE30() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_ue30_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE30")]
    public Node20<string> Utf8Json_Deser_UE30() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_ue30_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE30")]
    public Node20<string> Jil_Deser_UE30() => Jil.JSON.Deserialize<Node20<string>>(_ue30_s)!;

    [Benchmark, BenchmarkCategory("Serialize-UE30")]
    public string STJ_Ser_UE30() => JsonSerializer.Serialize(_ue30);
    [Benchmark, BenchmarkCategory("Serialize-UE30")]
    public string Newtonsoft_Ser_UE30() => Newtonsoft.Json.JsonConvert.SerializeObject(_ue30);
    [Benchmark, BenchmarkCategory("Serialize-UE30")]
    public string SpanJson_Ser_UE30() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_ue30);
    [Benchmark, BenchmarkCategory("Serialize-UE30")]
    public string Utf8Json_Ser_UE30() => Utf8Json.JsonSerializer.ToJsonString(_ue30);
    [Benchmark, BenchmarkCategory("Serialize-UE30")]
    public string Jil_Ser_UE30() => Jil.JSON.Serialize(_ue30);

    // ===================== UE50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> STJ_Deser_UE50() => JsonSerializer.Deserialize<Node20<string>>(_ue50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> Newtonsoft_Deser_UE50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_ue50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> SpanJson_Deser_UE50() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_ue50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> Utf8Json_Deser_UE50() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_ue50_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> Jil_Deser_UE50() => Jil.JSON.Deserialize<Node20<string>>(_ue50_s)!;

    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public string STJ_Ser_UE50() => JsonSerializer.Serialize(_ue50);
    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public string Newtonsoft_Ser_UE50() => Newtonsoft.Json.JsonConvert.SerializeObject(_ue50);
    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public string SpanJson_Ser_UE50() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_ue50);
    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public string Utf8Json_Ser_UE50() => Utf8Json.JsonSerializer.ToJsonString(_ue50);
    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public string Jil_Ser_UE50() => Jil.JSON.Serialize(_ue50);

    // ===================== UE70 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE70")]
    public Node20<string> STJ_Deser_UE70() => JsonSerializer.Deserialize<Node20<string>>(_ue70_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE70")]
    public Node20<string> Newtonsoft_Deser_UE70() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_ue70_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE70")]
    public Node20<string> SpanJson_Deser_UE70() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_ue70_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE70")]
    public Node20<string> Utf8Json_Deser_UE70() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_ue70_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE70")]
    public Node20<string> Jil_Deser_UE70() => Jil.JSON.Deserialize<Node20<string>>(_ue70_s)!;

    [Benchmark, BenchmarkCategory("Serialize-UE70")]
    public string STJ_Ser_UE70() => JsonSerializer.Serialize(_ue70);
    [Benchmark, BenchmarkCategory("Serialize-UE70")]
    public string Newtonsoft_Ser_UE70() => Newtonsoft.Json.JsonConvert.SerializeObject(_ue70);
    [Benchmark, BenchmarkCategory("Serialize-UE70")]
    public string SpanJson_Ser_UE70() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_ue70);
    [Benchmark, BenchmarkCategory("Serialize-UE70")]
    public string Utf8Json_Ser_UE70() => Utf8Json.JsonSerializer.ToJsonString(_ue70);
    [Benchmark, BenchmarkCategory("Serialize-UE70")]
    public string Jil_Ser_UE70() => Jil.JSON.Serialize(_ue70);
}
