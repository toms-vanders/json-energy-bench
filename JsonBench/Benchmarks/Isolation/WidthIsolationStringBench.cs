using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using Serialization.Bench;
using Serialization.Bench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Width isolation benchmark: varies width (7 levels), string I/O.
/// Baseline: D5, Textual, Object-only, ASCII, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class WidthIsolationStringBench
{
    private string _w2_s = null!; private Node2<string> _w2 = null!;
    private string _w5_s = null!; private Node5<string> _w5 = null!;
    private string _w10_s = null!; private Node10<string> _w10 = null!;
    private string _w20_s = null!; private Node20<string> _w20 = null!;
    private string _w50_s = null!; private Node50<string> _w50 = null!;
    private string _w100_s = null!; private Node100<string> _w100 = null!;
    private string _w200_s = null!; private Node200<string> _w200 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _w2_s = Load("W2"); _w2 = JsonSerializer.Deserialize<Node2<string>>(_w2_s)!;
        _w5_s = Load("W5"); _w5 = JsonSerializer.Deserialize<Node5<string>>(_w5_s)!;
        _w10_s = Load("W10"); _w10 = JsonSerializer.Deserialize<Node10<string>>(_w10_s)!;
        _w20_s = Load("W20"); _w20 = JsonSerializer.Deserialize<Node20<string>>(_w20_s)!;
        _w50_s = Load("W50"); _w50 = JsonSerializer.Deserialize<Node50<string>>(_w50_s)!;
        _w100_s = Load("W100"); _w100 = JsonSerializer.Deserialize<Node100<string>>(_w100_s)!;
        _w200_s = Load("W200"); _w200 = JsonSerializer.Deserialize<Node200<string>>(_w200_s)!;
    }

    private static string Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoWidth", $"{id}.json");
        return File.ReadAllText(path);
    }

    // ===================== W2 =====================

    [Benchmark, BenchmarkCategory("Deserialize-W2")]
    public Node2<string> STJ_Deser_W2() => JsonSerializer.Deserialize<Node2<string>>(_w2_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W2")]
    public Node2<string> Newtonsoft_Deser_W2() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node2<string>>(_w2_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W2")]
    public Node2<string> SpanJson_Deser_W2() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node2<string>>(_w2_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W2")]
    public Node2<string> Utf8Json_Deser_W2() => Utf8Json.JsonSerializer.Deserialize<Node2<string>>(Encoding.UTF8.GetBytes(_w2_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-W2")]
    public Node2<string> Jil_Deser_W2() => Jil.JSON.Deserialize<Node2<string>>(_w2_s)!;

    [Benchmark, BenchmarkCategory("Serialize-W2")]
    public string STJ_Ser_W2() => JsonSerializer.Serialize(_w2);
    [Benchmark, BenchmarkCategory("Serialize-W2")]
    public string Newtonsoft_Ser_W2() => Newtonsoft.Json.JsonConvert.SerializeObject(_w2);
    [Benchmark, BenchmarkCategory("Serialize-W2")]
    public string SpanJson_Ser_W2() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_w2);
    [Benchmark, BenchmarkCategory("Serialize-W2")]
    public string Utf8Json_Ser_W2() => Utf8Json.JsonSerializer.ToJsonString(_w2);
    [Benchmark, BenchmarkCategory("Serialize-W2")]
    public string Jil_Ser_W2() => Jil.JSON.Serialize(_w2);

    // ===================== W5 =====================

    [Benchmark, BenchmarkCategory("Deserialize-W5")]
    public Node5<string> STJ_Deser_W5() => JsonSerializer.Deserialize<Node5<string>>(_w5_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W5")]
    public Node5<string> Newtonsoft_Deser_W5() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node5<string>>(_w5_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W5")]
    public Node5<string> SpanJson_Deser_W5() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node5<string>>(_w5_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W5")]
    public Node5<string> Utf8Json_Deser_W5() => Utf8Json.JsonSerializer.Deserialize<Node5<string>>(Encoding.UTF8.GetBytes(_w5_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-W5")]
    public Node5<string> Jil_Deser_W5() => Jil.JSON.Deserialize<Node5<string>>(_w5_s)!;

    [Benchmark, BenchmarkCategory("Serialize-W5")]
    public string STJ_Ser_W5() => JsonSerializer.Serialize(_w5);
    [Benchmark, BenchmarkCategory("Serialize-W5")]
    public string Newtonsoft_Ser_W5() => Newtonsoft.Json.JsonConvert.SerializeObject(_w5);
    [Benchmark, BenchmarkCategory("Serialize-W5")]
    public string SpanJson_Ser_W5() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_w5);
    [Benchmark, BenchmarkCategory("Serialize-W5")]
    public string Utf8Json_Ser_W5() => Utf8Json.JsonSerializer.ToJsonString(_w5);
    [Benchmark, BenchmarkCategory("Serialize-W5")]
    public string Jil_Ser_W5() => Jil.JSON.Serialize(_w5);

    // ===================== W10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-W10")]
    public Node10<string> STJ_Deser_W10() => JsonSerializer.Deserialize<Node10<string>>(_w10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W10")]
    public Node10<string> Newtonsoft_Deser_W10() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node10<string>>(_w10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W10")]
    public Node10<string> SpanJson_Deser_W10() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node10<string>>(_w10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W10")]
    public Node10<string> Utf8Json_Deser_W10() => Utf8Json.JsonSerializer.Deserialize<Node10<string>>(Encoding.UTF8.GetBytes(_w10_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-W10")]
    public Node10<string> Jil_Deser_W10() => Jil.JSON.Deserialize<Node10<string>>(_w10_s)!;

    [Benchmark, BenchmarkCategory("Serialize-W10")]
    public string STJ_Ser_W10() => JsonSerializer.Serialize(_w10);
    [Benchmark, BenchmarkCategory("Serialize-W10")]
    public string Newtonsoft_Ser_W10() => Newtonsoft.Json.JsonConvert.SerializeObject(_w10);
    [Benchmark, BenchmarkCategory("Serialize-W10")]
    public string SpanJson_Ser_W10() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_w10);
    [Benchmark, BenchmarkCategory("Serialize-W10")]
    public string Utf8Json_Ser_W10() => Utf8Json.JsonSerializer.ToJsonString(_w10);
    [Benchmark, BenchmarkCategory("Serialize-W10")]
    public string Jil_Ser_W10() => Jil.JSON.Serialize(_w10);

    // ===================== W20 =====================

    [Benchmark, BenchmarkCategory("Deserialize-W20")]
    public Node20<string> STJ_Deser_W20() => JsonSerializer.Deserialize<Node20<string>>(_w20_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W20")]
    public Node20<string> Newtonsoft_Deser_W20() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_w20_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W20")]
    public Node20<string> SpanJson_Deser_W20() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_w20_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W20")]
    public Node20<string> Utf8Json_Deser_W20() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(Encoding.UTF8.GetBytes(_w20_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-W20")]
    public Node20<string> Jil_Deser_W20() => Jil.JSON.Deserialize<Node20<string>>(_w20_s)!;

    [Benchmark, BenchmarkCategory("Serialize-W20")]
    public string STJ_Ser_W20() => JsonSerializer.Serialize(_w20);
    [Benchmark, BenchmarkCategory("Serialize-W20")]
    public string Newtonsoft_Ser_W20() => Newtonsoft.Json.JsonConvert.SerializeObject(_w20);
    [Benchmark, BenchmarkCategory("Serialize-W20")]
    public string SpanJson_Ser_W20() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_w20);
    [Benchmark, BenchmarkCategory("Serialize-W20")]
    public string Utf8Json_Ser_W20() => Utf8Json.JsonSerializer.ToJsonString(_w20);
    [Benchmark, BenchmarkCategory("Serialize-W20")]
    public string Jil_Ser_W20() => Jil.JSON.Serialize(_w20);

    // ===================== W50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-W50")]
    public Node50<string> STJ_Deser_W50() => JsonSerializer.Deserialize<Node50<string>>(_w50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W50")]
    public Node50<string> Newtonsoft_Deser_W50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node50<string>>(_w50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W50")]
    public Node50<string> SpanJson_Deser_W50() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node50<string>>(_w50_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W50")]
    public Node50<string> Utf8Json_Deser_W50() => Utf8Json.JsonSerializer.Deserialize<Node50<string>>(Encoding.UTF8.GetBytes(_w50_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-W50")]
    public Node50<string> Jil_Deser_W50() => Jil.JSON.Deserialize<Node50<string>>(_w50_s)!;

    [Benchmark, BenchmarkCategory("Serialize-W50")]
    public string STJ_Ser_W50() => JsonSerializer.Serialize(_w50);
    [Benchmark, BenchmarkCategory("Serialize-W50")]
    public string Newtonsoft_Ser_W50() => Newtonsoft.Json.JsonConvert.SerializeObject(_w50);
    [Benchmark, BenchmarkCategory("Serialize-W50")]
    public string SpanJson_Ser_W50() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_w50);
    [Benchmark, BenchmarkCategory("Serialize-W50")]
    public string Utf8Json_Ser_W50() => Utf8Json.JsonSerializer.ToJsonString(_w50);
    [Benchmark, BenchmarkCategory("Serialize-W50")]
    public string Jil_Ser_W50() => Jil.JSON.Serialize(_w50);

    // ===================== W100 =====================

    [Benchmark, BenchmarkCategory("Deserialize-W100")]
    public Node100<string> STJ_Deser_W100() => JsonSerializer.Deserialize<Node100<string>>(_w100_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W100")]
    public Node100<string> Newtonsoft_Deser_W100() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node100<string>>(_w100_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W100")]
    public Node100<string> SpanJson_Deser_W100() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node100<string>>(_w100_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W100")]
    public Node100<string> Utf8Json_Deser_W100() => Utf8Json.JsonSerializer.Deserialize<Node100<string>>(Encoding.UTF8.GetBytes(_w100_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-W100")]
    public Node100<string> Jil_Deser_W100() => Jil.JSON.Deserialize<Node100<string>>(_w100_s)!;

    [Benchmark, BenchmarkCategory("Serialize-W100")]
    public string STJ_Ser_W100() => JsonSerializer.Serialize(_w100);
    [Benchmark, BenchmarkCategory("Serialize-W100")]
    public string Newtonsoft_Ser_W100() => Newtonsoft.Json.JsonConvert.SerializeObject(_w100);
    [Benchmark, BenchmarkCategory("Serialize-W100")]
    public string SpanJson_Ser_W100() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_w100);
    [Benchmark, BenchmarkCategory("Serialize-W100")]
    public string Utf8Json_Ser_W100() => Utf8Json.JsonSerializer.ToJsonString(_w100);
    [Benchmark, BenchmarkCategory("Serialize-W100")]
    public string Jil_Ser_W100() => Jil.JSON.Serialize(_w100);

    // ===================== W200 =====================

    [Benchmark, BenchmarkCategory("Deserialize-W200")]
    public Node200<string> STJ_Deser_W200() => JsonSerializer.Deserialize<Node200<string>>(_w200_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W200")]
    public Node200<string> Newtonsoft_Deser_W200() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node200<string>>(_w200_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W200")]
    public Node200<string> SpanJson_Deser_W200() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node200<string>>(_w200_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-W200")]
    public Node200<string> Utf8Json_Deser_W200() => Utf8Json.JsonSerializer.Deserialize<Node200<string>>(Encoding.UTF8.GetBytes(_w200_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-W200")]
    public Node200<string> Jil_Deser_W200() => Jil.JSON.Deserialize<Node200<string>>(_w200_s)!;

    [Benchmark, BenchmarkCategory("Serialize-W200")]
    public string STJ_Ser_W200() => JsonSerializer.Serialize(_w200);
    [Benchmark, BenchmarkCategory("Serialize-W200")]
    public string Newtonsoft_Ser_W200() => Newtonsoft.Json.JsonConvert.SerializeObject(_w200);
    [Benchmark, BenchmarkCategory("Serialize-W200")]
    public string SpanJson_Ser_W200() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_w200);
    [Benchmark, BenchmarkCategory("Serialize-W200")]
    public string Utf8Json_Ser_W200() => Utf8Json.JsonSerializer.ToJsonString(_w200);
    [Benchmark, BenchmarkCategory("Serialize-W200")]
    public string Jil_Ser_W200() => Jil.JSON.Serialize(_w200);
}
