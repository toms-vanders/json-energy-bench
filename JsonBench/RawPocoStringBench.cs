using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.MinimalFactorial;
using Serialization.Bench;
using Serialization.Bench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench;

/// <summary>
/// Raw POCO benchmarks using string input/output for all libraries.
/// Libraries that are natively UTF-8 (Utf8Json) include the encoding conversion cost.
/// </summary>
[Config(typeof(BenchConfig))]public class RawPocoStringBench
{
    private string _d2w10t_s = null!; private D2_W10_T _d2w10t = null!;
    private string _d2w10n_s = null!; private D2_W10_N _d2w10n = null!;
    private string _d2w10b_s = null!; private D2_W10_B _d2w10b = null!;
    private string _d2w100t_s = null!; private D2_W100_T _d2w100t = null!;
    private string _d2w100n_s = null!; private D2_W100_N _d2w100n = null!;
    private string _d2w100b_s = null!; private D2_W100_B _d2w100b = null!;
    private string _d10w10t_s = null!; private D10_W10_T _d10w10t = null!;
    private string _d10w10n_s = null!; private D10_W10_N _d10w10n = null!;
    private string _d10w10b_s = null!; private D10_W10_B _d10w10b = null!;
    private string _d10w100t_s = null!; private D10_W100_T _d10w100t = null!;
    private string _d10w100n_s = null!; private D10_W100_N _d10w100n = null!;
    private string _d10w100b_s = null!; private D10_W100_B _d10w100b = null!;

    [GlobalSetup]
    public void Setup()
    {
        _d2w10t_s = Load("D2-W10-T"); _d2w10t = JsonSerializer.Deserialize<D2_W10_T>(_d2w10t_s)!;
        _d2w10n_s = Load("D2-W10-N"); _d2w10n = JsonSerializer.Deserialize<D2_W10_N>(_d2w10n_s)!;
        _d2w10b_s = Load("D2-W10-B"); _d2w10b = JsonSerializer.Deserialize<D2_W10_B>(_d2w10b_s)!;
        _d2w100t_s = Load("D2-W100-T"); _d2w100t = JsonSerializer.Deserialize<D2_W100_T>(_d2w100t_s)!;
        _d2w100n_s = Load("D2-W100-N"); _d2w100n = JsonSerializer.Deserialize<D2_W100_N>(_d2w100n_s)!;
        _d2w100b_s = Load("D2-W100-B"); _d2w100b = JsonSerializer.Deserialize<D2_W100_B>(_d2w100b_s)!;
        _d10w10t_s = Load("D10-W10-T"); _d10w10t = JsonSerializer.Deserialize<D10_W10_T>(_d10w10t_s)!;
        _d10w10n_s = Load("D10-W10-N"); _d10w10n = JsonSerializer.Deserialize<D10_W10_N>(_d10w10n_s)!;
        _d10w10b_s = Load("D10-W10-B"); _d10w10b = JsonSerializer.Deserialize<D10_W10_B>(_d10w10b_s)!;
        _d10w100t_s = Load("D10-W100-T"); _d10w100t = JsonSerializer.Deserialize<D10_W100_T>(_d10w100t_s)!;
        _d10w100n_s = Load("D10-W100-N"); _d10w100n = JsonSerializer.Deserialize<D10_W100_N>(_d10w100n_s)!;
        _d10w100b_s = Load("D10-W100-B"); _d10w100b = JsonSerializer.Deserialize<D10_W100_B>(_d10w100b_s)!;
    }

    private static string Load(string id)
    {
        var path = SerializationHelper.TestDataFile("MinimalFactorial", $"{id}.json");
        return File.ReadAllText(path);
    }

    // ===================== D2-W10-T =====================

    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-T")]
    public D2_W10_T STJ_Deser_D2_W10_T() => JsonSerializer.Deserialize<D2_W10_T>(_d2w10t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-T")]
    public D2_W10_T Newtonsoft_Deser_D2_W10_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<D2_W10_T>(_d2w10t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-T")]
    public D2_W10_T SpanJson_Deser_D2_W10_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<D2_W10_T>(_d2w10t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-T")]
    public D2_W10_T Utf8Json_Deser_D2_W10_T() => Utf8Json.JsonSerializer.Deserialize<D2_W10_T>(Encoding.UTF8.GetBytes(_d2w10t_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-T")]
    public D2_W10_T Jil_Deser_D2_W10_T() => Jil.JSON.Deserialize<D2_W10_T>(_d2w10t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W10-T")]
    public string STJ_Ser_D2_W10_T() => JsonSerializer.Serialize(_d2w10t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W10-T")]
    public string Newtonsoft_Ser_D2_W10_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w10t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W10-T")]
    public string SpanJson_Ser_D2_W10_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w10t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W10-T")]
    public string Utf8Json_Ser_D2_W10_T() => Utf8Json.JsonSerializer.ToJsonString(_d2w10t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W10-T")]
    public string Jil_Ser_D2_W10_T() => Jil.JSON.Serialize(_d2w10t);

    // ===================== D2-W10-N =====================

    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-N")]
    public D2_W10_N STJ_Deser_D2_W10_N() => JsonSerializer.Deserialize<D2_W10_N>(_d2w10n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-N")]
    public D2_W10_N Newtonsoft_Deser_D2_W10_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<D2_W10_N>(_d2w10n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-N")]
    public D2_W10_N SpanJson_Deser_D2_W10_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<D2_W10_N>(_d2w10n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-N")]
    public D2_W10_N Utf8Json_Deser_D2_W10_N() => Utf8Json.JsonSerializer.Deserialize<D2_W10_N>(Encoding.UTF8.GetBytes(_d2w10n_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-N")]
    public D2_W10_N Jil_Deser_D2_W10_N() => Jil.JSON.Deserialize<D2_W10_N>(_d2w10n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W10-N")]
    public string STJ_Ser_D2_W10_N() => JsonSerializer.Serialize(_d2w10n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W10-N")]
    public string Newtonsoft_Ser_D2_W10_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w10n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W10-N")]
    public string SpanJson_Ser_D2_W10_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w10n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W10-N")]
    public string Utf8Json_Ser_D2_W10_N() => Utf8Json.JsonSerializer.ToJsonString(_d2w10n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W10-N")]
    public string Jil_Ser_D2_W10_N() => Jil.JSON.Serialize(_d2w10n);

    // ===================== D2-W10-B =====================

    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-B")]
    public D2_W10_B STJ_Deser_D2_W10_B() => JsonSerializer.Deserialize<D2_W10_B>(_d2w10b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-B")]
    public D2_W10_B Newtonsoft_Deser_D2_W10_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<D2_W10_B>(_d2w10b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-B")]
    public D2_W10_B SpanJson_Deser_D2_W10_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<D2_W10_B>(_d2w10b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-B")]
    public D2_W10_B Utf8Json_Deser_D2_W10_B() => Utf8Json.JsonSerializer.Deserialize<D2_W10_B>(Encoding.UTF8.GetBytes(_d2w10b_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W10-B")]
    public D2_W10_B Jil_Deser_D2_W10_B() => Jil.JSON.Deserialize<D2_W10_B>(_d2w10b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W10-B")]
    public string STJ_Ser_D2_W10_B() => JsonSerializer.Serialize(_d2w10b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W10-B")]
    public string Newtonsoft_Ser_D2_W10_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w10b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W10-B")]
    public string SpanJson_Ser_D2_W10_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w10b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W10-B")]
    public string Utf8Json_Ser_D2_W10_B() => Utf8Json.JsonSerializer.ToJsonString(_d2w10b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W10-B")]
    public string Jil_Ser_D2_W10_B() => Jil.JSON.Serialize(_d2w10b);

    // ===================== D2-W100-T =====================

    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-T")]
    public D2_W100_T STJ_Deser_D2_W100_T() => JsonSerializer.Deserialize<D2_W100_T>(_d2w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-T")]
    public D2_W100_T Newtonsoft_Deser_D2_W100_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<D2_W100_T>(_d2w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-T")]
    public D2_W100_T SpanJson_Deser_D2_W100_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<D2_W100_T>(_d2w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-T")]
    public D2_W100_T Utf8Json_Deser_D2_W100_T() => Utf8Json.JsonSerializer.Deserialize<D2_W100_T>(Encoding.UTF8.GetBytes(_d2w100t_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-T")]
    public D2_W100_T Jil_Deser_D2_W100_T() => Jil.JSON.Deserialize<D2_W100_T>(_d2w100t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W100-T")]
    public string STJ_Ser_D2_W100_T() => JsonSerializer.Serialize(_d2w100t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-T")]
    public string Newtonsoft_Ser_D2_W100_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w100t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-T")]
    public string SpanJson_Ser_D2_W100_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w100t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-T")]
    public string Utf8Json_Ser_D2_W100_T() => Utf8Json.JsonSerializer.ToJsonString(_d2w100t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-T")]
    public string Jil_Ser_D2_W100_T() => Jil.JSON.Serialize(_d2w100t);

    // ===================== D2-W100-N =====================

    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-N")]
    public D2_W100_N STJ_Deser_D2_W100_N() => JsonSerializer.Deserialize<D2_W100_N>(_d2w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-N")]
    public D2_W100_N Newtonsoft_Deser_D2_W100_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<D2_W100_N>(_d2w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-N")]
    public D2_W100_N SpanJson_Deser_D2_W100_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<D2_W100_N>(_d2w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-N")]
    public D2_W100_N Utf8Json_Deser_D2_W100_N() => Utf8Json.JsonSerializer.Deserialize<D2_W100_N>(Encoding.UTF8.GetBytes(_d2w100n_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-N")]
    public D2_W100_N Jil_Deser_D2_W100_N() => Jil.JSON.Deserialize<D2_W100_N>(_d2w100n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W100-N")]
    public string STJ_Ser_D2_W100_N() => JsonSerializer.Serialize(_d2w100n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-N")]
    public string Newtonsoft_Ser_D2_W100_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w100n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-N")]
    public string SpanJson_Ser_D2_W100_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w100n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-N")]
    public string Utf8Json_Ser_D2_W100_N() => Utf8Json.JsonSerializer.ToJsonString(_d2w100n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-N")]
    public string Jil_Ser_D2_W100_N() => Jil.JSON.Serialize(_d2w100n);

    // ===================== D2-W100-B =====================

    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-B")]
    public D2_W100_B STJ_Deser_D2_W100_B() => JsonSerializer.Deserialize<D2_W100_B>(_d2w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-B")]
    public D2_W100_B Newtonsoft_Deser_D2_W100_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<D2_W100_B>(_d2w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-B")]
    public D2_W100_B SpanJson_Deser_D2_W100_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<D2_W100_B>(_d2w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-B")]
    public D2_W100_B Utf8Json_Deser_D2_W100_B() => Utf8Json.JsonSerializer.Deserialize<D2_W100_B>(Encoding.UTF8.GetBytes(_d2w100b_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-B")]
    public D2_W100_B Jil_Deser_D2_W100_B() => Jil.JSON.Deserialize<D2_W100_B>(_d2w100b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W100-B")]
    public string STJ_Ser_D2_W100_B() => JsonSerializer.Serialize(_d2w100b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-B")]
    public string Newtonsoft_Ser_D2_W100_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w100b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-B")]
    public string SpanJson_Ser_D2_W100_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w100b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-B")]
    public string Utf8Json_Ser_D2_W100_B() => Utf8Json.JsonSerializer.ToJsonString(_d2w100b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-B")]
    public string Jil_Ser_D2_W100_B() => Jil.JSON.Serialize(_d2w100b);

    // ===================== D10-W10-T =====================

    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-T")]
    public D10_W10_T STJ_Deser_D10_W10_T() => JsonSerializer.Deserialize<D10_W10_T>(_d10w10t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-T")]
    public D10_W10_T Newtonsoft_Deser_D10_W10_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<D10_W10_T>(_d10w10t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-T")]
    public D10_W10_T SpanJson_Deser_D10_W10_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<D10_W10_T>(_d10w10t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-T")]
    public D10_W10_T Utf8Json_Deser_D10_W10_T() => Utf8Json.JsonSerializer.Deserialize<D10_W10_T>(Encoding.UTF8.GetBytes(_d10w10t_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-T")]
    public D10_W10_T Jil_Deser_D10_W10_T() => Jil.JSON.Deserialize<D10_W10_T>(_d10w10t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W10-T")]
    public string STJ_Ser_D10_W10_T() => JsonSerializer.Serialize(_d10w10t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W10-T")]
    public string Newtonsoft_Ser_D10_W10_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w10t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W10-T")]
    public string SpanJson_Ser_D10_W10_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w10t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W10-T")]
    public string Utf8Json_Ser_D10_W10_T() => Utf8Json.JsonSerializer.ToJsonString(_d10w10t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W10-T")]
    public string Jil_Ser_D10_W10_T() => Jil.JSON.Serialize(_d10w10t);

    // ===================== D10-W10-N =====================

    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-N")]
    public D10_W10_N STJ_Deser_D10_W10_N() => JsonSerializer.Deserialize<D10_W10_N>(_d10w10n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-N")]
    public D10_W10_N Newtonsoft_Deser_D10_W10_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<D10_W10_N>(_d10w10n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-N")]
    public D10_W10_N SpanJson_Deser_D10_W10_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<D10_W10_N>(_d10w10n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-N")]
    public D10_W10_N Utf8Json_Deser_D10_W10_N() => Utf8Json.JsonSerializer.Deserialize<D10_W10_N>(Encoding.UTF8.GetBytes(_d10w10n_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-N")]
    public D10_W10_N Jil_Deser_D10_W10_N() => Jil.JSON.Deserialize<D10_W10_N>(_d10w10n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W10-N")]
    public string STJ_Ser_D10_W10_N() => JsonSerializer.Serialize(_d10w10n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W10-N")]
    public string Newtonsoft_Ser_D10_W10_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w10n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W10-N")]
    public string SpanJson_Ser_D10_W10_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w10n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W10-N")]
    public string Utf8Json_Ser_D10_W10_N() => Utf8Json.JsonSerializer.ToJsonString(_d10w10n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W10-N")]
    public string Jil_Ser_D10_W10_N() => Jil.JSON.Serialize(_d10w10n);

    // ===================== D10-W10-B =====================

    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-B")]
    public D10_W10_B STJ_Deser_D10_W10_B() => JsonSerializer.Deserialize<D10_W10_B>(_d10w10b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-B")]
    public D10_W10_B Newtonsoft_Deser_D10_W10_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<D10_W10_B>(_d10w10b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-B")]
    public D10_W10_B SpanJson_Deser_D10_W10_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<D10_W10_B>(_d10w10b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-B")]
    public D10_W10_B Utf8Json_Deser_D10_W10_B() => Utf8Json.JsonSerializer.Deserialize<D10_W10_B>(Encoding.UTF8.GetBytes(_d10w10b_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W10-B")]
    public D10_W10_B Jil_Deser_D10_W10_B() => Jil.JSON.Deserialize<D10_W10_B>(_d10w10b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W10-B")]
    public string STJ_Ser_D10_W10_B() => JsonSerializer.Serialize(_d10w10b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W10-B")]
    public string Newtonsoft_Ser_D10_W10_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w10b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W10-B")]
    public string SpanJson_Ser_D10_W10_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w10b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W10-B")]
    public string Utf8Json_Ser_D10_W10_B() => Utf8Json.JsonSerializer.ToJsonString(_d10w10b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W10-B")]
    public string Jil_Ser_D10_W10_B() => Jil.JSON.Serialize(_d10w10b);

    // ===================== D10-W100-T =====================

    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-T")]
    public D10_W100_T STJ_Deser_D10_W100_T() => JsonSerializer.Deserialize<D10_W100_T>(_d10w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-T")]
    public D10_W100_T Newtonsoft_Deser_D10_W100_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<D10_W100_T>(_d10w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-T")]
    public D10_W100_T SpanJson_Deser_D10_W100_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<D10_W100_T>(_d10w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-T")]
    public D10_W100_T Utf8Json_Deser_D10_W100_T() => Utf8Json.JsonSerializer.Deserialize<D10_W100_T>(Encoding.UTF8.GetBytes(_d10w100t_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-T")]
    public D10_W100_T Jil_Deser_D10_W100_T() => Jil.JSON.Deserialize<D10_W100_T>(_d10w100t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W100-T")]
    public string STJ_Ser_D10_W100_T() => JsonSerializer.Serialize(_d10w100t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-T")]
    public string Newtonsoft_Ser_D10_W100_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w100t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-T")]
    public string SpanJson_Ser_D10_W100_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w100t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-T")]
    public string Utf8Json_Ser_D10_W100_T() => Utf8Json.JsonSerializer.ToJsonString(_d10w100t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-T")]
    public string Jil_Ser_D10_W100_T() => Jil.JSON.Serialize(_d10w100t);

    // ===================== D10-W100-N =====================

    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-N")]
    public D10_W100_N STJ_Deser_D10_W100_N() => JsonSerializer.Deserialize<D10_W100_N>(_d10w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-N")]
    public D10_W100_N Newtonsoft_Deser_D10_W100_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<D10_W100_N>(_d10w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-N")]
    public D10_W100_N SpanJson_Deser_D10_W100_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<D10_W100_N>(_d10w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-N")]
    public D10_W100_N Utf8Json_Deser_D10_W100_N() => Utf8Json.JsonSerializer.Deserialize<D10_W100_N>(Encoding.UTF8.GetBytes(_d10w100n_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-N")]
    public D10_W100_N Jil_Deser_D10_W100_N() => Jil.JSON.Deserialize<D10_W100_N>(_d10w100n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W100-N")]
    public string STJ_Ser_D10_W100_N() => JsonSerializer.Serialize(_d10w100n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-N")]
    public string Newtonsoft_Ser_D10_W100_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w100n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-N")]
    public string SpanJson_Ser_D10_W100_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w100n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-N")]
    public string Utf8Json_Ser_D10_W100_N() => Utf8Json.JsonSerializer.ToJsonString(_d10w100n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-N")]
    public string Jil_Ser_D10_W100_N() => Jil.JSON.Serialize(_d10w100n);

    // ===================== D10-W100-B =====================

    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-B")]
    public D10_W100_B STJ_Deser_D10_W100_B() => JsonSerializer.Deserialize<D10_W100_B>(_d10w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-B")]
    public D10_W100_B Newtonsoft_Deser_D10_W100_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<D10_W100_B>(_d10w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-B")]
    public D10_W100_B SpanJson_Deser_D10_W100_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<D10_W100_B>(_d10w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-B")]
    public D10_W100_B Utf8Json_Deser_D10_W100_B() => Utf8Json.JsonSerializer.Deserialize<D10_W100_B>(Encoding.UTF8.GetBytes(_d10w100b_s))!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-B")]
    public D10_W100_B Jil_Deser_D10_W100_B() => Jil.JSON.Deserialize<D10_W100_B>(_d10w100b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W100-B")]
    public string STJ_Ser_D10_W100_B() => JsonSerializer.Serialize(_d10w100b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-B")]
    public string Newtonsoft_Ser_D10_W100_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w100b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-B")]
    public string SpanJson_Ser_D10_W100_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w100b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-B")]
    public string Utf8Json_Ser_D10_W100_B() => Utf8Json.JsonSerializer.ToJsonString(_d10w100b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-B")]
    public string Jil_Ser_D10_W100_B() => Jil.JSON.Serialize(_d10w100b);
}
