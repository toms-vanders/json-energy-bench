using BenchmarkDotNet.Attributes;
using JsonBench.Models.Factorial;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Factorial;

/// <summary>
/// Factorial benchmark: Depth(2,5,10,20) x Width(5,20,50,100) x Content(T,N,B) = 48 configs.
/// String input/output. UTF-8 native libraries (Utf8Json) include encoding conversion cost.
/// </summary>
[Config(typeof(BenchConfig))]
public class FactorialStringBench
{
    // --- Width 5 fields ---
    private string _d2w5t_s = null!; private Node5<string> _d2w5t = null!;
    private string _d2w5n_s = null!; private Node5<int> _d2w5n = null!;
    private string _d2w5b_s = null!; private Node5<bool> _d2w5b = null!;
    private string _d5w5t_s = null!; private Node5<string> _d5w5t = null!;
    private string _d5w5n_s = null!; private Node5<int> _d5w5n = null!;
    private string _d5w5b_s = null!; private Node5<bool> _d5w5b = null!;
    private string _d10w5t_s = null!; private Node5<string> _d10w5t = null!;
    private string _d10w5n_s = null!; private Node5<int> _d10w5n = null!;
    private string _d10w5b_s = null!; private Node5<bool> _d10w5b = null!;
    private string _d20w5t_s = null!; private Node5<string> _d20w5t = null!;
    private string _d20w5n_s = null!; private Node5<int> _d20w5n = null!;
    private string _d20w5b_s = null!; private Node5<bool> _d20w5b = null!;

    // --- Width 20 fields ---
    private string _d2w20t_s = null!; private Node20<string> _d2w20t = null!;
    private string _d2w20n_s = null!; private Node20<int> _d2w20n = null!;
    private string _d2w20b_s = null!; private Node20<bool> _d2w20b = null!;
    private string _d5w20t_s = null!; private Node20<string> _d5w20t = null!;
    private string _d5w20n_s = null!; private Node20<int> _d5w20n = null!;
    private string _d5w20b_s = null!; private Node20<bool> _d5w20b = null!;
    private string _d10w20t_s = null!; private Node20<string> _d10w20t = null!;
    private string _d10w20n_s = null!; private Node20<int> _d10w20n = null!;
    private string _d10w20b_s = null!; private Node20<bool> _d10w20b = null!;
    private string _d20w20t_s = null!; private Node20<string> _d20w20t = null!;
    private string _d20w20n_s = null!; private Node20<int> _d20w20n = null!;
    private string _d20w20b_s = null!; private Node20<bool> _d20w20b = null!;

    // --- Width 50 fields ---
    private string _d2w50t_s = null!; private Node50<string> _d2w50t = null!;
    private string _d2w50n_s = null!; private Node50<int> _d2w50n = null!;
    private string _d2w50b_s = null!; private Node50<bool> _d2w50b = null!;
    private string _d5w50t_s = null!; private Node50<string> _d5w50t = null!;
    private string _d5w50n_s = null!; private Node50<int> _d5w50n = null!;
    private string _d5w50b_s = null!; private Node50<bool> _d5w50b = null!;
    private string _d10w50t_s = null!; private Node50<string> _d10w50t = null!;
    private string _d10w50n_s = null!; private Node50<int> _d10w50n = null!;
    private string _d10w50b_s = null!; private Node50<bool> _d10w50b = null!;
    private string _d20w50t_s = null!; private Node50<string> _d20w50t = null!;
    private string _d20w50n_s = null!; private Node50<int> _d20w50n = null!;
    private string _d20w50b_s = null!; private Node50<bool> _d20w50b = null!;

    // --- Width 100 fields ---
    private string _d2w100t_s = null!; private Node100<string> _d2w100t = null!;
    private string _d2w100n_s = null!; private Node100<int> _d2w100n = null!;
    private string _d2w100b_s = null!; private Node100<bool> _d2w100b = null!;
    private string _d5w100t_s = null!; private Node100<string> _d5w100t = null!;
    private string _d5w100n_s = null!; private Node100<int> _d5w100n = null!;
    private string _d5w100b_s = null!; private Node100<bool> _d5w100b = null!;
    private string _d10w100t_s = null!; private Node100<string> _d10w100t = null!;
    private string _d10w100n_s = null!; private Node100<int> _d10w100n = null!;
    private string _d10w100b_s = null!; private Node100<bool> _d10w100b = null!;
    private string _d20w100t_s = null!; private Node100<string> _d20w100t = null!;
    private string _d20w100n_s = null!; private Node100<int> _d20w100n = null!;
    private string _d20w100b_s = null!; private Node100<bool> _d20w100b = null!;

    [GlobalSetup]
    public void Setup()
    {
        _d2w5t_s = Load("D2-W5-T"); _d2w5t = JsonSerializer.Deserialize<Node5<string>>(_d2w5t_s)!;
        _d2w5n_s = Load("D2-W5-N"); _d2w5n = JsonSerializer.Deserialize<Node5<int>>(_d2w5n_s)!;
        _d2w5b_s = Load("D2-W5-B"); _d2w5b = JsonSerializer.Deserialize<Node5<bool>>(_d2w5b_s)!;
        _d5w5t_s = Load("D5-W5-T"); _d5w5t = JsonSerializer.Deserialize<Node5<string>>(_d5w5t_s)!;
        _d5w5n_s = Load("D5-W5-N"); _d5w5n = JsonSerializer.Deserialize<Node5<int>>(_d5w5n_s)!;
        _d5w5b_s = Load("D5-W5-B"); _d5w5b = JsonSerializer.Deserialize<Node5<bool>>(_d5w5b_s)!;
        _d10w5t_s = Load("D10-W5-T"); _d10w5t = JsonSerializer.Deserialize<Node5<string>>(_d10w5t_s)!;
        _d10w5n_s = Load("D10-W5-N"); _d10w5n = JsonSerializer.Deserialize<Node5<int>>(_d10w5n_s)!;
        _d10w5b_s = Load("D10-W5-B"); _d10w5b = JsonSerializer.Deserialize<Node5<bool>>(_d10w5b_s)!;
        _d20w5t_s = Load("D20-W5-T"); _d20w5t = JsonSerializer.Deserialize<Node5<string>>(_d20w5t_s)!;
        _d20w5n_s = Load("D20-W5-N"); _d20w5n = JsonSerializer.Deserialize<Node5<int>>(_d20w5n_s)!;
        _d20w5b_s = Load("D20-W5-B"); _d20w5b = JsonSerializer.Deserialize<Node5<bool>>(_d20w5b_s)!;
        _d2w20t_s = Load("D2-W20-T"); _d2w20t = JsonSerializer.Deserialize<Node20<string>>(_d2w20t_s)!;
        _d2w20n_s = Load("D2-W20-N"); _d2w20n = JsonSerializer.Deserialize<Node20<int>>(_d2w20n_s)!;
        _d2w20b_s = Load("D2-W20-B"); _d2w20b = JsonSerializer.Deserialize<Node20<bool>>(_d2w20b_s)!;
        _d5w20t_s = Load("D5-W20-T"); _d5w20t = JsonSerializer.Deserialize<Node20<string>>(_d5w20t_s)!;
        _d5w20n_s = Load("D5-W20-N"); _d5w20n = JsonSerializer.Deserialize<Node20<int>>(_d5w20n_s)!;
        _d5w20b_s = Load("D5-W20-B"); _d5w20b = JsonSerializer.Deserialize<Node20<bool>>(_d5w20b_s)!;
        _d10w20t_s = Load("D10-W20-T"); _d10w20t = JsonSerializer.Deserialize<Node20<string>>(_d10w20t_s)!;
        _d10w20n_s = Load("D10-W20-N"); _d10w20n = JsonSerializer.Deserialize<Node20<int>>(_d10w20n_s)!;
        _d10w20b_s = Load("D10-W20-B"); _d10w20b = JsonSerializer.Deserialize<Node20<bool>>(_d10w20b_s)!;
        _d20w20t_s = Load("D20-W20-T"); _d20w20t = JsonSerializer.Deserialize<Node20<string>>(_d20w20t_s)!;
        _d20w20n_s = Load("D20-W20-N"); _d20w20n = JsonSerializer.Deserialize<Node20<int>>(_d20w20n_s)!;
        _d20w20b_s = Load("D20-W20-B"); _d20w20b = JsonSerializer.Deserialize<Node20<bool>>(_d20w20b_s)!;
        _d2w50t_s = Load("D2-W50-T"); _d2w50t = JsonSerializer.Deserialize<Node50<string>>(_d2w50t_s)!;
        _d2w50n_s = Load("D2-W50-N"); _d2w50n = JsonSerializer.Deserialize<Node50<int>>(_d2w50n_s)!;
        _d2w50b_s = Load("D2-W50-B"); _d2w50b = JsonSerializer.Deserialize<Node50<bool>>(_d2w50b_s)!;
        _d5w50t_s = Load("D5-W50-T"); _d5w50t = JsonSerializer.Deserialize<Node50<string>>(_d5w50t_s)!;
        _d5w50n_s = Load("D5-W50-N"); _d5w50n = JsonSerializer.Deserialize<Node50<int>>(_d5w50n_s)!;
        _d5w50b_s = Load("D5-W50-B"); _d5w50b = JsonSerializer.Deserialize<Node50<bool>>(_d5w50b_s)!;
        _d10w50t_s = Load("D10-W50-T"); _d10w50t = JsonSerializer.Deserialize<Node50<string>>(_d10w50t_s)!;
        _d10w50n_s = Load("D10-W50-N"); _d10w50n = JsonSerializer.Deserialize<Node50<int>>(_d10w50n_s)!;
        _d10w50b_s = Load("D10-W50-B"); _d10w50b = JsonSerializer.Deserialize<Node50<bool>>(_d10w50b_s)!;
        _d20w50t_s = Load("D20-W50-T"); _d20w50t = JsonSerializer.Deserialize<Node50<string>>(_d20w50t_s)!;
        _d20w50n_s = Load("D20-W50-N"); _d20w50n = JsonSerializer.Deserialize<Node50<int>>(_d20w50n_s)!;
        _d20w50b_s = Load("D20-W50-B"); _d20w50b = JsonSerializer.Deserialize<Node50<bool>>(_d20w50b_s)!;
        _d2w100t_s = Load("D2-W100-T"); _d2w100t = JsonSerializer.Deserialize<Node100<string>>(_d2w100t_s)!;
        _d2w100n_s = Load("D2-W100-N"); _d2w100n = JsonSerializer.Deserialize<Node100<int>>(_d2w100n_s)!;
        _d2w100b_s = Load("D2-W100-B"); _d2w100b = JsonSerializer.Deserialize<Node100<bool>>(_d2w100b_s)!;
        _d5w100t_s = Load("D5-W100-T"); _d5w100t = JsonSerializer.Deserialize<Node100<string>>(_d5w100t_s)!;
        _d5w100n_s = Load("D5-W100-N"); _d5w100n = JsonSerializer.Deserialize<Node100<int>>(_d5w100n_s)!;
        _d5w100b_s = Load("D5-W100-B"); _d5w100b = JsonSerializer.Deserialize<Node100<bool>>(_d5w100b_s)!;
        _d10w100t_s = Load("D10-W100-T"); _d10w100t = JsonSerializer.Deserialize<Node100<string>>(_d10w100t_s)!;
        _d10w100n_s = Load("D10-W100-N"); _d10w100n = JsonSerializer.Deserialize<Node100<int>>(_d10w100n_s)!;
        _d10w100b_s = Load("D10-W100-B"); _d10w100b = JsonSerializer.Deserialize<Node100<bool>>(_d10w100b_s)!;
        _d20w100t_s = Load("D20-W100-T"); _d20w100t = JsonSerializer.Deserialize<Node100<string>>(_d20w100t_s)!;
        _d20w100n_s = Load("D20-W100-N"); _d20w100n = JsonSerializer.Deserialize<Node100<int>>(_d20w100n_s)!;
        _d20w100b_s = Load("D20-W100-B"); _d20w100b = JsonSerializer.Deserialize<Node100<bool>>(_d20w100b_s)!;
    }

    private static string Load(string id)
    {
        var path = SerializationHelper.TestDataFile("Factorial", $"{id}.json");
        return File.ReadAllText(path);
    }

    // ==================== Depth 2 ====================

    // ----- D2-W5-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-T")]
    public Node5<string> STJRefGen_Deser_D2_W5_T() => JsonSerializer.Deserialize<Node5<string>>(_d2w5t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-T")]
    public Node5<string> STJSrcGen_Deser_D2_W5_T() => JsonSerializer.Deserialize(_d2w5t_s, FactorialJsonContext.Default.Node5String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-T")]
    public Node5<string> Newtonsoft_Deser_D2_W5_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node5<string>>(_d2w5t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-T")]
    public Node5<string> SpanJson_Deser_D2_W5_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node5<string>>(_d2w5t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-T")]
    public Node5<string> Utf8Json_Deser_D2_W5_T() => Utf8Json.JsonSerializer.Deserialize<Node5<string>>(_d2w5t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W5-T")]
    public string STJRefGen_Ser_D2_W5_T() => JsonSerializer.Serialize(_d2w5t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W5-T")]
    public string STJSrcGen_Ser_D2_W5_T() => JsonSerializer.Serialize(_d2w5t, FactorialJsonContext.Default.Node5String);
    [Benchmark, BenchmarkCategory("Serialize-D2-W5-T")]
    public string Newtonsoft_Ser_D2_W5_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w5t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W5-T")]
    public string SpanJson_Ser_D2_W5_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w5t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W5-T")]
    public string Utf8Json_Ser_D2_W5_T() => Utf8Json.JsonSerializer.ToJsonString(_d2w5t);

    // ----- D2-W5-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-N")]
    public Node5<int> STJRefGen_Deser_D2_W5_N() => JsonSerializer.Deserialize<Node5<int>>(_d2w5n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-N")]
    public Node5<int> STJSrcGen_Deser_D2_W5_N() => JsonSerializer.Deserialize(_d2w5n_s, FactorialJsonContext.Default.Node5Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-N")]
    public Node5<int> Newtonsoft_Deser_D2_W5_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node5<int>>(_d2w5n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-N")]
    public Node5<int> SpanJson_Deser_D2_W5_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node5<int>>(_d2w5n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-N")]
    public Node5<int> Utf8Json_Deser_D2_W5_N() => Utf8Json.JsonSerializer.Deserialize<Node5<int>>(_d2w5n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W5-N")]
    public string STJRefGen_Ser_D2_W5_N() => JsonSerializer.Serialize(_d2w5n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W5-N")]
    public string STJSrcGen_Ser_D2_W5_N() => JsonSerializer.Serialize(_d2w5n, FactorialJsonContext.Default.Node5Int32);
    [Benchmark, BenchmarkCategory("Serialize-D2-W5-N")]
    public string Newtonsoft_Ser_D2_W5_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w5n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W5-N")]
    public string SpanJson_Ser_D2_W5_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w5n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W5-N")]
    public string Utf8Json_Ser_D2_W5_N() => Utf8Json.JsonSerializer.ToJsonString(_d2w5n);

    // ----- D2-W5-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-B")]
    public Node5<bool> STJRefGen_Deser_D2_W5_B() => JsonSerializer.Deserialize<Node5<bool>>(_d2w5b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-B")]
    public Node5<bool> STJSrcGen_Deser_D2_W5_B() => JsonSerializer.Deserialize(_d2w5b_s, FactorialJsonContext.Default.Node5Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-B")]
    public Node5<bool> Newtonsoft_Deser_D2_W5_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node5<bool>>(_d2w5b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-B")]
    public Node5<bool> SpanJson_Deser_D2_W5_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node5<bool>>(_d2w5b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W5-B")]
    public Node5<bool> Utf8Json_Deser_D2_W5_B() => Utf8Json.JsonSerializer.Deserialize<Node5<bool>>(_d2w5b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W5-B")]
    public string STJRefGen_Ser_D2_W5_B() => JsonSerializer.Serialize(_d2w5b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W5-B")]
    public string STJSrcGen_Ser_D2_W5_B() => JsonSerializer.Serialize(_d2w5b, FactorialJsonContext.Default.Node5Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D2-W5-B")]
    public string Newtonsoft_Ser_D2_W5_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w5b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W5-B")]
    public string SpanJson_Ser_D2_W5_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w5b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W5-B")]
    public string Utf8Json_Ser_D2_W5_B() => Utf8Json.JsonSerializer.ToJsonString(_d2w5b);

    // ----- D2-W20-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-T")]
    public Node20<string> STJRefGen_Deser_D2_W20_T() => JsonSerializer.Deserialize<Node20<string>>(_d2w20t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-T")]
    public Node20<string> STJSrcGen_Deser_D2_W20_T() => JsonSerializer.Deserialize(_d2w20t_s, FactorialJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-T")]
    public Node20<string> Newtonsoft_Deser_D2_W20_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_d2w20t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-T")]
    public Node20<string> SpanJson_Deser_D2_W20_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_d2w20t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-T")]
    public Node20<string> Utf8Json_Deser_D2_W20_T() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_d2w20t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W20-T")]
    public string STJRefGen_Ser_D2_W20_T() => JsonSerializer.Serialize(_d2w20t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W20-T")]
    public string STJSrcGen_Ser_D2_W20_T() => JsonSerializer.Serialize(_d2w20t, FactorialJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-D2-W20-T")]
    public string Newtonsoft_Ser_D2_W20_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w20t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W20-T")]
    public string SpanJson_Ser_D2_W20_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w20t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W20-T")]
    public string Utf8Json_Ser_D2_W20_T() => Utf8Json.JsonSerializer.ToJsonString(_d2w20t);

    // ----- D2-W20-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-N")]
    public Node20<int> STJRefGen_Deser_D2_W20_N() => JsonSerializer.Deserialize<Node20<int>>(_d2w20n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-N")]
    public Node20<int> STJSrcGen_Deser_D2_W20_N() => JsonSerializer.Deserialize(_d2w20n_s, FactorialJsonContext.Default.Node20Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-N")]
    public Node20<int> Newtonsoft_Deser_D2_W20_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<int>>(_d2w20n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-N")]
    public Node20<int> SpanJson_Deser_D2_W20_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<int>>(_d2w20n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-N")]
    public Node20<int> Utf8Json_Deser_D2_W20_N() => Utf8Json.JsonSerializer.Deserialize<Node20<int>>(_d2w20n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W20-N")]
    public string STJRefGen_Ser_D2_W20_N() => JsonSerializer.Serialize(_d2w20n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W20-N")]
    public string STJSrcGen_Ser_D2_W20_N() => JsonSerializer.Serialize(_d2w20n, FactorialJsonContext.Default.Node20Int32);
    [Benchmark, BenchmarkCategory("Serialize-D2-W20-N")]
    public string Newtonsoft_Ser_D2_W20_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w20n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W20-N")]
    public string SpanJson_Ser_D2_W20_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w20n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W20-N")]
    public string Utf8Json_Ser_D2_W20_N() => Utf8Json.JsonSerializer.ToJsonString(_d2w20n);

    // ----- D2-W20-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-B")]
    public Node20<bool> STJRefGen_Deser_D2_W20_B() => JsonSerializer.Deserialize<Node20<bool>>(_d2w20b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-B")]
    public Node20<bool> STJSrcGen_Deser_D2_W20_B() => JsonSerializer.Deserialize(_d2w20b_s, FactorialJsonContext.Default.Node20Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-B")]
    public Node20<bool> Newtonsoft_Deser_D2_W20_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<bool>>(_d2w20b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-B")]
    public Node20<bool> SpanJson_Deser_D2_W20_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<bool>>(_d2w20b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W20-B")]
    public Node20<bool> Utf8Json_Deser_D2_W20_B() => Utf8Json.JsonSerializer.Deserialize<Node20<bool>>(_d2w20b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W20-B")]
    public string STJRefGen_Ser_D2_W20_B() => JsonSerializer.Serialize(_d2w20b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W20-B")]
    public string STJSrcGen_Ser_D2_W20_B() => JsonSerializer.Serialize(_d2w20b, FactorialJsonContext.Default.Node20Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D2-W20-B")]
    public string Newtonsoft_Ser_D2_W20_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w20b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W20-B")]
    public string SpanJson_Ser_D2_W20_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w20b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W20-B")]
    public string Utf8Json_Ser_D2_W20_B() => Utf8Json.JsonSerializer.ToJsonString(_d2w20b);

    // ----- D2-W50-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-T")]
    public Node50<string> STJRefGen_Deser_D2_W50_T() => JsonSerializer.Deserialize<Node50<string>>(_d2w50t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-T")]
    public Node50<string> STJSrcGen_Deser_D2_W50_T() => JsonSerializer.Deserialize(_d2w50t_s, FactorialJsonContext.Default.Node50String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-T")]
    public Node50<string> Newtonsoft_Deser_D2_W50_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node50<string>>(_d2w50t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-T")]
    public Node50<string> SpanJson_Deser_D2_W50_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node50<string>>(_d2w50t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-T")]
    public Node50<string> Utf8Json_Deser_D2_W50_T() => Utf8Json.JsonSerializer.Deserialize<Node50<string>>(_d2w50t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W50-T")]
    public string STJRefGen_Ser_D2_W50_T() => JsonSerializer.Serialize(_d2w50t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W50-T")]
    public string STJSrcGen_Ser_D2_W50_T() => JsonSerializer.Serialize(_d2w50t, FactorialJsonContext.Default.Node50String);
    [Benchmark, BenchmarkCategory("Serialize-D2-W50-T")]
    public string Newtonsoft_Ser_D2_W50_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w50t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W50-T")]
    public string SpanJson_Ser_D2_W50_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w50t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W50-T")]
    public string Utf8Json_Ser_D2_W50_T() => Utf8Json.JsonSerializer.ToJsonString(_d2w50t);

    // ----- D2-W50-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-N")]
    public Node50<int> STJRefGen_Deser_D2_W50_N() => JsonSerializer.Deserialize<Node50<int>>(_d2w50n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-N")]
    public Node50<int> STJSrcGen_Deser_D2_W50_N() => JsonSerializer.Deserialize(_d2w50n_s, FactorialJsonContext.Default.Node50Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-N")]
    public Node50<int> Newtonsoft_Deser_D2_W50_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node50<int>>(_d2w50n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-N")]
    public Node50<int> SpanJson_Deser_D2_W50_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node50<int>>(_d2w50n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-N")]
    public Node50<int> Utf8Json_Deser_D2_W50_N() => Utf8Json.JsonSerializer.Deserialize<Node50<int>>(_d2w50n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W50-N")]
    public string STJRefGen_Ser_D2_W50_N() => JsonSerializer.Serialize(_d2w50n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W50-N")]
    public string STJSrcGen_Ser_D2_W50_N() => JsonSerializer.Serialize(_d2w50n, FactorialJsonContext.Default.Node50Int32);
    [Benchmark, BenchmarkCategory("Serialize-D2-W50-N")]
    public string Newtonsoft_Ser_D2_W50_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w50n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W50-N")]
    public string SpanJson_Ser_D2_W50_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w50n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W50-N")]
    public string Utf8Json_Ser_D2_W50_N() => Utf8Json.JsonSerializer.ToJsonString(_d2w50n);

    // ----- D2-W50-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-B")]
    public Node50<bool> STJRefGen_Deser_D2_W50_B() => JsonSerializer.Deserialize<Node50<bool>>(_d2w50b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-B")]
    public Node50<bool> STJSrcGen_Deser_D2_W50_B() => JsonSerializer.Deserialize(_d2w50b_s, FactorialJsonContext.Default.Node50Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-B")]
    public Node50<bool> Newtonsoft_Deser_D2_W50_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node50<bool>>(_d2w50b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-B")]
    public Node50<bool> SpanJson_Deser_D2_W50_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node50<bool>>(_d2w50b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W50-B")]
    public Node50<bool> Utf8Json_Deser_D2_W50_B() => Utf8Json.JsonSerializer.Deserialize<Node50<bool>>(_d2w50b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W50-B")]
    public string STJRefGen_Ser_D2_W50_B() => JsonSerializer.Serialize(_d2w50b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W50-B")]
    public string STJSrcGen_Ser_D2_W50_B() => JsonSerializer.Serialize(_d2w50b, FactorialJsonContext.Default.Node50Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D2-W50-B")]
    public string Newtonsoft_Ser_D2_W50_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w50b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W50-B")]
    public string SpanJson_Ser_D2_W50_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w50b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W50-B")]
    public string Utf8Json_Ser_D2_W50_B() => Utf8Json.JsonSerializer.ToJsonString(_d2w50b);

    // ----- D2-W100-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-T")]
    public Node100<string> STJRefGen_Deser_D2_W100_T() => JsonSerializer.Deserialize<Node100<string>>(_d2w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-T")]
    public Node100<string> STJSrcGen_Deser_D2_W100_T() => JsonSerializer.Deserialize(_d2w100t_s, FactorialJsonContext.Default.Node100String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-T")]
    public Node100<string> Newtonsoft_Deser_D2_W100_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node100<string>>(_d2w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-T")]
    public Node100<string> SpanJson_Deser_D2_W100_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node100<string>>(_d2w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-T")]
    public Node100<string> Utf8Json_Deser_D2_W100_T() => Utf8Json.JsonSerializer.Deserialize<Node100<string>>(_d2w100t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W100-T")]
    public string STJRefGen_Ser_D2_W100_T() => JsonSerializer.Serialize(_d2w100t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-T")]
    public string STJSrcGen_Ser_D2_W100_T() => JsonSerializer.Serialize(_d2w100t, FactorialJsonContext.Default.Node100String);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-T")]
    public string Newtonsoft_Ser_D2_W100_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w100t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-T")]
    public string SpanJson_Ser_D2_W100_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w100t);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-T")]
    public string Utf8Json_Ser_D2_W100_T() => Utf8Json.JsonSerializer.ToJsonString(_d2w100t);

    // ----- D2-W100-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-N")]
    public Node100<int> STJRefGen_Deser_D2_W100_N() => JsonSerializer.Deserialize<Node100<int>>(_d2w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-N")]
    public Node100<int> STJSrcGen_Deser_D2_W100_N() => JsonSerializer.Deserialize(_d2w100n_s, FactorialJsonContext.Default.Node100Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-N")]
    public Node100<int> Newtonsoft_Deser_D2_W100_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node100<int>>(_d2w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-N")]
    public Node100<int> SpanJson_Deser_D2_W100_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node100<int>>(_d2w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-N")]
    public Node100<int> Utf8Json_Deser_D2_W100_N() => Utf8Json.JsonSerializer.Deserialize<Node100<int>>(_d2w100n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W100-N")]
    public string STJRefGen_Ser_D2_W100_N() => JsonSerializer.Serialize(_d2w100n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-N")]
    public string STJSrcGen_Ser_D2_W100_N() => JsonSerializer.Serialize(_d2w100n, FactorialJsonContext.Default.Node100Int32);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-N")]
    public string Newtonsoft_Ser_D2_W100_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w100n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-N")]
    public string SpanJson_Ser_D2_W100_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w100n);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-N")]
    public string Utf8Json_Ser_D2_W100_N() => Utf8Json.JsonSerializer.ToJsonString(_d2w100n);

    // ----- D2-W100-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-B")]
    public Node100<bool> STJRefGen_Deser_D2_W100_B() => JsonSerializer.Deserialize<Node100<bool>>(_d2w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-B")]
    public Node100<bool> STJSrcGen_Deser_D2_W100_B() => JsonSerializer.Deserialize(_d2w100b_s, FactorialJsonContext.Default.Node100Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-B")]
    public Node100<bool> Newtonsoft_Deser_D2_W100_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node100<bool>>(_d2w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-B")]
    public Node100<bool> SpanJson_Deser_D2_W100_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node100<bool>>(_d2w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D2-W100-B")]
    public Node100<bool> Utf8Json_Deser_D2_W100_B() => Utf8Json.JsonSerializer.Deserialize<Node100<bool>>(_d2w100b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D2-W100-B")]
    public string STJRefGen_Ser_D2_W100_B() => JsonSerializer.Serialize(_d2w100b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-B")]
    public string STJSrcGen_Ser_D2_W100_B() => JsonSerializer.Serialize(_d2w100b, FactorialJsonContext.Default.Node100Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-B")]
    public string Newtonsoft_Ser_D2_W100_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d2w100b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-B")]
    public string SpanJson_Ser_D2_W100_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d2w100b);
    [Benchmark, BenchmarkCategory("Serialize-D2-W100-B")]
    public string Utf8Json_Ser_D2_W100_B() => Utf8Json.JsonSerializer.ToJsonString(_d2w100b);

    // ==================== Depth 5 ====================

    // ----- D5-W5-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-T")]
    public Node5<string> STJRefGen_Deser_D5_W5_T() => JsonSerializer.Deserialize<Node5<string>>(_d5w5t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-T")]
    public Node5<string> STJSrcGen_Deser_D5_W5_T() => JsonSerializer.Deserialize(_d5w5t_s, FactorialJsonContext.Default.Node5String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-T")]
    public Node5<string> Newtonsoft_Deser_D5_W5_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node5<string>>(_d5w5t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-T")]
    public Node5<string> SpanJson_Deser_D5_W5_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node5<string>>(_d5w5t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-T")]
    public Node5<string> Utf8Json_Deser_D5_W5_T() => Utf8Json.JsonSerializer.Deserialize<Node5<string>>(_d5w5t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D5-W5-T")]
    public string STJRefGen_Ser_D5_W5_T() => JsonSerializer.Serialize(_d5w5t);
    [Benchmark, BenchmarkCategory("Serialize-D5-W5-T")]
    public string STJSrcGen_Ser_D5_W5_T() => JsonSerializer.Serialize(_d5w5t, FactorialJsonContext.Default.Node5String);
    [Benchmark, BenchmarkCategory("Serialize-D5-W5-T")]
    public string Newtonsoft_Ser_D5_W5_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d5w5t);
    [Benchmark, BenchmarkCategory("Serialize-D5-W5-T")]
    public string SpanJson_Ser_D5_W5_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d5w5t);
    [Benchmark, BenchmarkCategory("Serialize-D5-W5-T")]
    public string Utf8Json_Ser_D5_W5_T() => Utf8Json.JsonSerializer.ToJsonString(_d5w5t);

    // ----- D5-W5-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-N")]
    public Node5<int> STJRefGen_Deser_D5_W5_N() => JsonSerializer.Deserialize<Node5<int>>(_d5w5n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-N")]
    public Node5<int> STJSrcGen_Deser_D5_W5_N() => JsonSerializer.Deserialize(_d5w5n_s, FactorialJsonContext.Default.Node5Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-N")]
    public Node5<int> Newtonsoft_Deser_D5_W5_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node5<int>>(_d5w5n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-N")]
    public Node5<int> SpanJson_Deser_D5_W5_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node5<int>>(_d5w5n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-N")]
    public Node5<int> Utf8Json_Deser_D5_W5_N() => Utf8Json.JsonSerializer.Deserialize<Node5<int>>(_d5w5n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D5-W5-N")]
    public string STJRefGen_Ser_D5_W5_N() => JsonSerializer.Serialize(_d5w5n);
    [Benchmark, BenchmarkCategory("Serialize-D5-W5-N")]
    public string STJSrcGen_Ser_D5_W5_N() => JsonSerializer.Serialize(_d5w5n, FactorialJsonContext.Default.Node5Int32);
    [Benchmark, BenchmarkCategory("Serialize-D5-W5-N")]
    public string Newtonsoft_Ser_D5_W5_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d5w5n);
    [Benchmark, BenchmarkCategory("Serialize-D5-W5-N")]
    public string SpanJson_Ser_D5_W5_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d5w5n);
    [Benchmark, BenchmarkCategory("Serialize-D5-W5-N")]
    public string Utf8Json_Ser_D5_W5_N() => Utf8Json.JsonSerializer.ToJsonString(_d5w5n);

    // ----- D5-W5-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-B")]
    public Node5<bool> STJRefGen_Deser_D5_W5_B() => JsonSerializer.Deserialize<Node5<bool>>(_d5w5b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-B")]
    public Node5<bool> STJSrcGen_Deser_D5_W5_B() => JsonSerializer.Deserialize(_d5w5b_s, FactorialJsonContext.Default.Node5Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-B")]
    public Node5<bool> Newtonsoft_Deser_D5_W5_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node5<bool>>(_d5w5b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-B")]
    public Node5<bool> SpanJson_Deser_D5_W5_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node5<bool>>(_d5w5b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W5-B")]
    public Node5<bool> Utf8Json_Deser_D5_W5_B() => Utf8Json.JsonSerializer.Deserialize<Node5<bool>>(_d5w5b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D5-W5-B")]
    public string STJRefGen_Ser_D5_W5_B() => JsonSerializer.Serialize(_d5w5b);
    [Benchmark, BenchmarkCategory("Serialize-D5-W5-B")]
    public string STJSrcGen_Ser_D5_W5_B() => JsonSerializer.Serialize(_d5w5b, FactorialJsonContext.Default.Node5Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D5-W5-B")]
    public string Newtonsoft_Ser_D5_W5_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d5w5b);
    [Benchmark, BenchmarkCategory("Serialize-D5-W5-B")]
    public string SpanJson_Ser_D5_W5_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d5w5b);
    [Benchmark, BenchmarkCategory("Serialize-D5-W5-B")]
    public string Utf8Json_Ser_D5_W5_B() => Utf8Json.JsonSerializer.ToJsonString(_d5w5b);

    // ----- D5-W20-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-T")]
    public Node20<string> STJRefGen_Deser_D5_W20_T() => JsonSerializer.Deserialize<Node20<string>>(_d5w20t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-T")]
    public Node20<string> STJSrcGen_Deser_D5_W20_T() => JsonSerializer.Deserialize(_d5w20t_s, FactorialJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-T")]
    public Node20<string> Newtonsoft_Deser_D5_W20_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_d5w20t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-T")]
    public Node20<string> SpanJson_Deser_D5_W20_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_d5w20t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-T")]
    public Node20<string> Utf8Json_Deser_D5_W20_T() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_d5w20t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D5-W20-T")]
    public string STJRefGen_Ser_D5_W20_T() => JsonSerializer.Serialize(_d5w20t);
    [Benchmark, BenchmarkCategory("Serialize-D5-W20-T")]
    public string STJSrcGen_Ser_D5_W20_T() => JsonSerializer.Serialize(_d5w20t, FactorialJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-D5-W20-T")]
    public string Newtonsoft_Ser_D5_W20_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d5w20t);
    [Benchmark, BenchmarkCategory("Serialize-D5-W20-T")]
    public string SpanJson_Ser_D5_W20_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d5w20t);
    [Benchmark, BenchmarkCategory("Serialize-D5-W20-T")]
    public string Utf8Json_Ser_D5_W20_T() => Utf8Json.JsonSerializer.ToJsonString(_d5w20t);

    // ----- D5-W20-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-N")]
    public Node20<int> STJRefGen_Deser_D5_W20_N() => JsonSerializer.Deserialize<Node20<int>>(_d5w20n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-N")]
    public Node20<int> STJSrcGen_Deser_D5_W20_N() => JsonSerializer.Deserialize(_d5w20n_s, FactorialJsonContext.Default.Node20Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-N")]
    public Node20<int> Newtonsoft_Deser_D5_W20_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<int>>(_d5w20n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-N")]
    public Node20<int> SpanJson_Deser_D5_W20_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<int>>(_d5w20n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-N")]
    public Node20<int> Utf8Json_Deser_D5_W20_N() => Utf8Json.JsonSerializer.Deserialize<Node20<int>>(_d5w20n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D5-W20-N")]
    public string STJRefGen_Ser_D5_W20_N() => JsonSerializer.Serialize(_d5w20n);
    [Benchmark, BenchmarkCategory("Serialize-D5-W20-N")]
    public string STJSrcGen_Ser_D5_W20_N() => JsonSerializer.Serialize(_d5w20n, FactorialJsonContext.Default.Node20Int32);
    [Benchmark, BenchmarkCategory("Serialize-D5-W20-N")]
    public string Newtonsoft_Ser_D5_W20_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d5w20n);
    [Benchmark, BenchmarkCategory("Serialize-D5-W20-N")]
    public string SpanJson_Ser_D5_W20_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d5w20n);
    [Benchmark, BenchmarkCategory("Serialize-D5-W20-N")]
    public string Utf8Json_Ser_D5_W20_N() => Utf8Json.JsonSerializer.ToJsonString(_d5w20n);

    // ----- D5-W20-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-B")]
    public Node20<bool> STJRefGen_Deser_D5_W20_B() => JsonSerializer.Deserialize<Node20<bool>>(_d5w20b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-B")]
    public Node20<bool> STJSrcGen_Deser_D5_W20_B() => JsonSerializer.Deserialize(_d5w20b_s, FactorialJsonContext.Default.Node20Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-B")]
    public Node20<bool> Newtonsoft_Deser_D5_W20_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<bool>>(_d5w20b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-B")]
    public Node20<bool> SpanJson_Deser_D5_W20_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<bool>>(_d5w20b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W20-B")]
    public Node20<bool> Utf8Json_Deser_D5_W20_B() => Utf8Json.JsonSerializer.Deserialize<Node20<bool>>(_d5w20b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D5-W20-B")]
    public string STJRefGen_Ser_D5_W20_B() => JsonSerializer.Serialize(_d5w20b);
    [Benchmark, BenchmarkCategory("Serialize-D5-W20-B")]
    public string STJSrcGen_Ser_D5_W20_B() => JsonSerializer.Serialize(_d5w20b, FactorialJsonContext.Default.Node20Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D5-W20-B")]
    public string Newtonsoft_Ser_D5_W20_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d5w20b);
    [Benchmark, BenchmarkCategory("Serialize-D5-W20-B")]
    public string SpanJson_Ser_D5_W20_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d5w20b);
    [Benchmark, BenchmarkCategory("Serialize-D5-W20-B")]
    public string Utf8Json_Ser_D5_W20_B() => Utf8Json.JsonSerializer.ToJsonString(_d5w20b);

    // ----- D5-W50-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-T")]
    public Node50<string> STJRefGen_Deser_D5_W50_T() => JsonSerializer.Deserialize<Node50<string>>(_d5w50t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-T")]
    public Node50<string> STJSrcGen_Deser_D5_W50_T() => JsonSerializer.Deserialize(_d5w50t_s, FactorialJsonContext.Default.Node50String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-T")]
    public Node50<string> Newtonsoft_Deser_D5_W50_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node50<string>>(_d5w50t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-T")]
    public Node50<string> SpanJson_Deser_D5_W50_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node50<string>>(_d5w50t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-T")]
    public Node50<string> Utf8Json_Deser_D5_W50_T() => Utf8Json.JsonSerializer.Deserialize<Node50<string>>(_d5w50t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D5-W50-T")]
    public string STJRefGen_Ser_D5_W50_T() => JsonSerializer.Serialize(_d5w50t);
    [Benchmark, BenchmarkCategory("Serialize-D5-W50-T")]
    public string STJSrcGen_Ser_D5_W50_T() => JsonSerializer.Serialize(_d5w50t, FactorialJsonContext.Default.Node50String);
    [Benchmark, BenchmarkCategory("Serialize-D5-W50-T")]
    public string Newtonsoft_Ser_D5_W50_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d5w50t);
    [Benchmark, BenchmarkCategory("Serialize-D5-W50-T")]
    public string SpanJson_Ser_D5_W50_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d5w50t);
    [Benchmark, BenchmarkCategory("Serialize-D5-W50-T")]
    public string Utf8Json_Ser_D5_W50_T() => Utf8Json.JsonSerializer.ToJsonString(_d5w50t);

    // ----- D5-W50-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-N")]
    public Node50<int> STJRefGen_Deser_D5_W50_N() => JsonSerializer.Deserialize<Node50<int>>(_d5w50n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-N")]
    public Node50<int> STJSrcGen_Deser_D5_W50_N() => JsonSerializer.Deserialize(_d5w50n_s, FactorialJsonContext.Default.Node50Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-N")]
    public Node50<int> Newtonsoft_Deser_D5_W50_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node50<int>>(_d5w50n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-N")]
    public Node50<int> SpanJson_Deser_D5_W50_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node50<int>>(_d5w50n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-N")]
    public Node50<int> Utf8Json_Deser_D5_W50_N() => Utf8Json.JsonSerializer.Deserialize<Node50<int>>(_d5w50n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D5-W50-N")]
    public string STJRefGen_Ser_D5_W50_N() => JsonSerializer.Serialize(_d5w50n);
    [Benchmark, BenchmarkCategory("Serialize-D5-W50-N")]
    public string STJSrcGen_Ser_D5_W50_N() => JsonSerializer.Serialize(_d5w50n, FactorialJsonContext.Default.Node50Int32);
    [Benchmark, BenchmarkCategory("Serialize-D5-W50-N")]
    public string Newtonsoft_Ser_D5_W50_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d5w50n);
    [Benchmark, BenchmarkCategory("Serialize-D5-W50-N")]
    public string SpanJson_Ser_D5_W50_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d5w50n);
    [Benchmark, BenchmarkCategory("Serialize-D5-W50-N")]
    public string Utf8Json_Ser_D5_W50_N() => Utf8Json.JsonSerializer.ToJsonString(_d5w50n);

    // ----- D5-W50-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-B")]
    public Node50<bool> STJRefGen_Deser_D5_W50_B() => JsonSerializer.Deserialize<Node50<bool>>(_d5w50b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-B")]
    public Node50<bool> STJSrcGen_Deser_D5_W50_B() => JsonSerializer.Deserialize(_d5w50b_s, FactorialJsonContext.Default.Node50Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-B")]
    public Node50<bool> Newtonsoft_Deser_D5_W50_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node50<bool>>(_d5w50b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-B")]
    public Node50<bool> SpanJson_Deser_D5_W50_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node50<bool>>(_d5w50b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W50-B")]
    public Node50<bool> Utf8Json_Deser_D5_W50_B() => Utf8Json.JsonSerializer.Deserialize<Node50<bool>>(_d5w50b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D5-W50-B")]
    public string STJRefGen_Ser_D5_W50_B() => JsonSerializer.Serialize(_d5w50b);
    [Benchmark, BenchmarkCategory("Serialize-D5-W50-B")]
    public string STJSrcGen_Ser_D5_W50_B() => JsonSerializer.Serialize(_d5w50b, FactorialJsonContext.Default.Node50Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D5-W50-B")]
    public string Newtonsoft_Ser_D5_W50_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d5w50b);
    [Benchmark, BenchmarkCategory("Serialize-D5-W50-B")]
    public string SpanJson_Ser_D5_W50_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d5w50b);
    [Benchmark, BenchmarkCategory("Serialize-D5-W50-B")]
    public string Utf8Json_Ser_D5_W50_B() => Utf8Json.JsonSerializer.ToJsonString(_d5w50b);

    // ----- D5-W100-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-T")]
    public Node100<string> STJRefGen_Deser_D5_W100_T() => JsonSerializer.Deserialize<Node100<string>>(_d5w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-T")]
    public Node100<string> STJSrcGen_Deser_D5_W100_T() => JsonSerializer.Deserialize(_d5w100t_s, FactorialJsonContext.Default.Node100String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-T")]
    public Node100<string> Newtonsoft_Deser_D5_W100_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node100<string>>(_d5w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-T")]
    public Node100<string> SpanJson_Deser_D5_W100_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node100<string>>(_d5w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-T")]
    public Node100<string> Utf8Json_Deser_D5_W100_T() => Utf8Json.JsonSerializer.Deserialize<Node100<string>>(_d5w100t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D5-W100-T")]
    public string STJRefGen_Ser_D5_W100_T() => JsonSerializer.Serialize(_d5w100t);
    [Benchmark, BenchmarkCategory("Serialize-D5-W100-T")]
    public string STJSrcGen_Ser_D5_W100_T() => JsonSerializer.Serialize(_d5w100t, FactorialJsonContext.Default.Node100String);
    [Benchmark, BenchmarkCategory("Serialize-D5-W100-T")]
    public string Newtonsoft_Ser_D5_W100_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d5w100t);
    [Benchmark, BenchmarkCategory("Serialize-D5-W100-T")]
    public string SpanJson_Ser_D5_W100_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d5w100t);
    [Benchmark, BenchmarkCategory("Serialize-D5-W100-T")]
    public string Utf8Json_Ser_D5_W100_T() => Utf8Json.JsonSerializer.ToJsonString(_d5w100t);

    // ----- D5-W100-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-N")]
    public Node100<int> STJRefGen_Deser_D5_W100_N() => JsonSerializer.Deserialize<Node100<int>>(_d5w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-N")]
    public Node100<int> STJSrcGen_Deser_D5_W100_N() => JsonSerializer.Deserialize(_d5w100n_s, FactorialJsonContext.Default.Node100Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-N")]
    public Node100<int> Newtonsoft_Deser_D5_W100_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node100<int>>(_d5w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-N")]
    public Node100<int> SpanJson_Deser_D5_W100_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node100<int>>(_d5w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-N")]
    public Node100<int> Utf8Json_Deser_D5_W100_N() => Utf8Json.JsonSerializer.Deserialize<Node100<int>>(_d5w100n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D5-W100-N")]
    public string STJRefGen_Ser_D5_W100_N() => JsonSerializer.Serialize(_d5w100n);
    [Benchmark, BenchmarkCategory("Serialize-D5-W100-N")]
    public string STJSrcGen_Ser_D5_W100_N() => JsonSerializer.Serialize(_d5w100n, FactorialJsonContext.Default.Node100Int32);
    [Benchmark, BenchmarkCategory("Serialize-D5-W100-N")]
    public string Newtonsoft_Ser_D5_W100_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d5w100n);
    [Benchmark, BenchmarkCategory("Serialize-D5-W100-N")]
    public string SpanJson_Ser_D5_W100_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d5w100n);
    [Benchmark, BenchmarkCategory("Serialize-D5-W100-N")]
    public string Utf8Json_Ser_D5_W100_N() => Utf8Json.JsonSerializer.ToJsonString(_d5w100n);

    // ----- D5-W100-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-B")]
    public Node100<bool> STJRefGen_Deser_D5_W100_B() => JsonSerializer.Deserialize<Node100<bool>>(_d5w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-B")]
    public Node100<bool> STJSrcGen_Deser_D5_W100_B() => JsonSerializer.Deserialize(_d5w100b_s, FactorialJsonContext.Default.Node100Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-B")]
    public Node100<bool> Newtonsoft_Deser_D5_W100_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node100<bool>>(_d5w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-B")]
    public Node100<bool> SpanJson_Deser_D5_W100_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node100<bool>>(_d5w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D5-W100-B")]
    public Node100<bool> Utf8Json_Deser_D5_W100_B() => Utf8Json.JsonSerializer.Deserialize<Node100<bool>>(_d5w100b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D5-W100-B")]
    public string STJRefGen_Ser_D5_W100_B() => JsonSerializer.Serialize(_d5w100b);
    [Benchmark, BenchmarkCategory("Serialize-D5-W100-B")]
    public string STJSrcGen_Ser_D5_W100_B() => JsonSerializer.Serialize(_d5w100b, FactorialJsonContext.Default.Node100Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D5-W100-B")]
    public string Newtonsoft_Ser_D5_W100_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d5w100b);
    [Benchmark, BenchmarkCategory("Serialize-D5-W100-B")]
    public string SpanJson_Ser_D5_W100_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d5w100b);
    [Benchmark, BenchmarkCategory("Serialize-D5-W100-B")]
    public string Utf8Json_Ser_D5_W100_B() => Utf8Json.JsonSerializer.ToJsonString(_d5w100b);

    // ==================== Depth 10 ====================

    // ----- D10-W5-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-T")]
    public Node5<string> STJRefGen_Deser_D10_W5_T() => JsonSerializer.Deserialize<Node5<string>>(_d10w5t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-T")]
    public Node5<string> STJSrcGen_Deser_D10_W5_T() => JsonSerializer.Deserialize(_d10w5t_s, FactorialJsonContext.Default.Node5String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-T")]
    public Node5<string> Newtonsoft_Deser_D10_W5_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node5<string>>(_d10w5t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-T")]
    public Node5<string> SpanJson_Deser_D10_W5_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node5<string>>(_d10w5t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-T")]
    public Node5<string> Utf8Json_Deser_D10_W5_T() => Utf8Json.JsonSerializer.Deserialize<Node5<string>>(_d10w5t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W5-T")]
    public string STJRefGen_Ser_D10_W5_T() => JsonSerializer.Serialize(_d10w5t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W5-T")]
    public string STJSrcGen_Ser_D10_W5_T() => JsonSerializer.Serialize(_d10w5t, FactorialJsonContext.Default.Node5String);
    [Benchmark, BenchmarkCategory("Serialize-D10-W5-T")]
    public string Newtonsoft_Ser_D10_W5_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w5t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W5-T")]
    public string SpanJson_Ser_D10_W5_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w5t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W5-T")]
    public string Utf8Json_Ser_D10_W5_T() => Utf8Json.JsonSerializer.ToJsonString(_d10w5t);

    // ----- D10-W5-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-N")]
    public Node5<int> STJRefGen_Deser_D10_W5_N() => JsonSerializer.Deserialize<Node5<int>>(_d10w5n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-N")]
    public Node5<int> STJSrcGen_Deser_D10_W5_N() => JsonSerializer.Deserialize(_d10w5n_s, FactorialJsonContext.Default.Node5Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-N")]
    public Node5<int> Newtonsoft_Deser_D10_W5_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node5<int>>(_d10w5n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-N")]
    public Node5<int> SpanJson_Deser_D10_W5_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node5<int>>(_d10w5n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-N")]
    public Node5<int> Utf8Json_Deser_D10_W5_N() => Utf8Json.JsonSerializer.Deserialize<Node5<int>>(_d10w5n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W5-N")]
    public string STJRefGen_Ser_D10_W5_N() => JsonSerializer.Serialize(_d10w5n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W5-N")]
    public string STJSrcGen_Ser_D10_W5_N() => JsonSerializer.Serialize(_d10w5n, FactorialJsonContext.Default.Node5Int32);
    [Benchmark, BenchmarkCategory("Serialize-D10-W5-N")]
    public string Newtonsoft_Ser_D10_W5_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w5n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W5-N")]
    public string SpanJson_Ser_D10_W5_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w5n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W5-N")]
    public string Utf8Json_Ser_D10_W5_N() => Utf8Json.JsonSerializer.ToJsonString(_d10w5n);

    // ----- D10-W5-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-B")]
    public Node5<bool> STJRefGen_Deser_D10_W5_B() => JsonSerializer.Deserialize<Node5<bool>>(_d10w5b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-B")]
    public Node5<bool> STJSrcGen_Deser_D10_W5_B() => JsonSerializer.Deserialize(_d10w5b_s, FactorialJsonContext.Default.Node5Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-B")]
    public Node5<bool> Newtonsoft_Deser_D10_W5_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node5<bool>>(_d10w5b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-B")]
    public Node5<bool> SpanJson_Deser_D10_W5_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node5<bool>>(_d10w5b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W5-B")]
    public Node5<bool> Utf8Json_Deser_D10_W5_B() => Utf8Json.JsonSerializer.Deserialize<Node5<bool>>(_d10w5b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W5-B")]
    public string STJRefGen_Ser_D10_W5_B() => JsonSerializer.Serialize(_d10w5b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W5-B")]
    public string STJSrcGen_Ser_D10_W5_B() => JsonSerializer.Serialize(_d10w5b, FactorialJsonContext.Default.Node5Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D10-W5-B")]
    public string Newtonsoft_Ser_D10_W5_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w5b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W5-B")]
    public string SpanJson_Ser_D10_W5_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w5b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W5-B")]
    public string Utf8Json_Ser_D10_W5_B() => Utf8Json.JsonSerializer.ToJsonString(_d10w5b);

    // ----- D10-W20-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-T")]
    public Node20<string> STJRefGen_Deser_D10_W20_T() => JsonSerializer.Deserialize<Node20<string>>(_d10w20t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-T")]
    public Node20<string> STJSrcGen_Deser_D10_W20_T() => JsonSerializer.Deserialize(_d10w20t_s, FactorialJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-T")]
    public Node20<string> Newtonsoft_Deser_D10_W20_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_d10w20t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-T")]
    public Node20<string> SpanJson_Deser_D10_W20_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_d10w20t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-T")]
    public Node20<string> Utf8Json_Deser_D10_W20_T() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_d10w20t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W20-T")]
    public string STJRefGen_Ser_D10_W20_T() => JsonSerializer.Serialize(_d10w20t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W20-T")]
    public string STJSrcGen_Ser_D10_W20_T() => JsonSerializer.Serialize(_d10w20t, FactorialJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-D10-W20-T")]
    public string Newtonsoft_Ser_D10_W20_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w20t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W20-T")]
    public string SpanJson_Ser_D10_W20_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w20t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W20-T")]
    public string Utf8Json_Ser_D10_W20_T() => Utf8Json.JsonSerializer.ToJsonString(_d10w20t);

    // ----- D10-W20-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-N")]
    public Node20<int> STJRefGen_Deser_D10_W20_N() => JsonSerializer.Deserialize<Node20<int>>(_d10w20n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-N")]
    public Node20<int> STJSrcGen_Deser_D10_W20_N() => JsonSerializer.Deserialize(_d10w20n_s, FactorialJsonContext.Default.Node20Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-N")]
    public Node20<int> Newtonsoft_Deser_D10_W20_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<int>>(_d10w20n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-N")]
    public Node20<int> SpanJson_Deser_D10_W20_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<int>>(_d10w20n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-N")]
    public Node20<int> Utf8Json_Deser_D10_W20_N() => Utf8Json.JsonSerializer.Deserialize<Node20<int>>(_d10w20n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W20-N")]
    public string STJRefGen_Ser_D10_W20_N() => JsonSerializer.Serialize(_d10w20n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W20-N")]
    public string STJSrcGen_Ser_D10_W20_N() => JsonSerializer.Serialize(_d10w20n, FactorialJsonContext.Default.Node20Int32);
    [Benchmark, BenchmarkCategory("Serialize-D10-W20-N")]
    public string Newtonsoft_Ser_D10_W20_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w20n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W20-N")]
    public string SpanJson_Ser_D10_W20_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w20n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W20-N")]
    public string Utf8Json_Ser_D10_W20_N() => Utf8Json.JsonSerializer.ToJsonString(_d10w20n);

    // ----- D10-W20-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-B")]
    public Node20<bool> STJRefGen_Deser_D10_W20_B() => JsonSerializer.Deserialize<Node20<bool>>(_d10w20b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-B")]
    public Node20<bool> STJSrcGen_Deser_D10_W20_B() => JsonSerializer.Deserialize(_d10w20b_s, FactorialJsonContext.Default.Node20Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-B")]
    public Node20<bool> Newtonsoft_Deser_D10_W20_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<bool>>(_d10w20b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-B")]
    public Node20<bool> SpanJson_Deser_D10_W20_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<bool>>(_d10w20b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W20-B")]
    public Node20<bool> Utf8Json_Deser_D10_W20_B() => Utf8Json.JsonSerializer.Deserialize<Node20<bool>>(_d10w20b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W20-B")]
    public string STJRefGen_Ser_D10_W20_B() => JsonSerializer.Serialize(_d10w20b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W20-B")]
    public string STJSrcGen_Ser_D10_W20_B() => JsonSerializer.Serialize(_d10w20b, FactorialJsonContext.Default.Node20Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D10-W20-B")]
    public string Newtonsoft_Ser_D10_W20_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w20b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W20-B")]
    public string SpanJson_Ser_D10_W20_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w20b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W20-B")]
    public string Utf8Json_Ser_D10_W20_B() => Utf8Json.JsonSerializer.ToJsonString(_d10w20b);

    // ----- D10-W50-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-T")]
    public Node50<string> STJRefGen_Deser_D10_W50_T() => JsonSerializer.Deserialize<Node50<string>>(_d10w50t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-T")]
    public Node50<string> STJSrcGen_Deser_D10_W50_T() => JsonSerializer.Deserialize(_d10w50t_s, FactorialJsonContext.Default.Node50String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-T")]
    public Node50<string> Newtonsoft_Deser_D10_W50_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node50<string>>(_d10w50t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-T")]
    public Node50<string> SpanJson_Deser_D10_W50_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node50<string>>(_d10w50t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-T")]
    public Node50<string> Utf8Json_Deser_D10_W50_T() => Utf8Json.JsonSerializer.Deserialize<Node50<string>>(_d10w50t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W50-T")]
    public string STJRefGen_Ser_D10_W50_T() => JsonSerializer.Serialize(_d10w50t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W50-T")]
    public string STJSrcGen_Ser_D10_W50_T() => JsonSerializer.Serialize(_d10w50t, FactorialJsonContext.Default.Node50String);
    [Benchmark, BenchmarkCategory("Serialize-D10-W50-T")]
    public string Newtonsoft_Ser_D10_W50_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w50t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W50-T")]
    public string SpanJson_Ser_D10_W50_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w50t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W50-T")]
    public string Utf8Json_Ser_D10_W50_T() => Utf8Json.JsonSerializer.ToJsonString(_d10w50t);

    // ----- D10-W50-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-N")]
    public Node50<int> STJRefGen_Deser_D10_W50_N() => JsonSerializer.Deserialize<Node50<int>>(_d10w50n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-N")]
    public Node50<int> STJSrcGen_Deser_D10_W50_N() => JsonSerializer.Deserialize(_d10w50n_s, FactorialJsonContext.Default.Node50Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-N")]
    public Node50<int> Newtonsoft_Deser_D10_W50_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node50<int>>(_d10w50n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-N")]
    public Node50<int> SpanJson_Deser_D10_W50_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node50<int>>(_d10w50n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-N")]
    public Node50<int> Utf8Json_Deser_D10_W50_N() => Utf8Json.JsonSerializer.Deserialize<Node50<int>>(_d10w50n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W50-N")]
    public string STJRefGen_Ser_D10_W50_N() => JsonSerializer.Serialize(_d10w50n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W50-N")]
    public string STJSrcGen_Ser_D10_W50_N() => JsonSerializer.Serialize(_d10w50n, FactorialJsonContext.Default.Node50Int32);
    [Benchmark, BenchmarkCategory("Serialize-D10-W50-N")]
    public string Newtonsoft_Ser_D10_W50_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w50n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W50-N")]
    public string SpanJson_Ser_D10_W50_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w50n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W50-N")]
    public string Utf8Json_Ser_D10_W50_N() => Utf8Json.JsonSerializer.ToJsonString(_d10w50n);

    // ----- D10-W50-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-B")]
    public Node50<bool> STJRefGen_Deser_D10_W50_B() => JsonSerializer.Deserialize<Node50<bool>>(_d10w50b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-B")]
    public Node50<bool> STJSrcGen_Deser_D10_W50_B() => JsonSerializer.Deserialize(_d10w50b_s, FactorialJsonContext.Default.Node50Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-B")]
    public Node50<bool> Newtonsoft_Deser_D10_W50_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node50<bool>>(_d10w50b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-B")]
    public Node50<bool> SpanJson_Deser_D10_W50_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node50<bool>>(_d10w50b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W50-B")]
    public Node50<bool> Utf8Json_Deser_D10_W50_B() => Utf8Json.JsonSerializer.Deserialize<Node50<bool>>(_d10w50b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W50-B")]
    public string STJRefGen_Ser_D10_W50_B() => JsonSerializer.Serialize(_d10w50b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W50-B")]
    public string STJSrcGen_Ser_D10_W50_B() => JsonSerializer.Serialize(_d10w50b, FactorialJsonContext.Default.Node50Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D10-W50-B")]
    public string Newtonsoft_Ser_D10_W50_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w50b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W50-B")]
    public string SpanJson_Ser_D10_W50_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w50b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W50-B")]
    public string Utf8Json_Ser_D10_W50_B() => Utf8Json.JsonSerializer.ToJsonString(_d10w50b);

    // ----- D10-W100-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-T")]
    public Node100<string> STJRefGen_Deser_D10_W100_T() => JsonSerializer.Deserialize<Node100<string>>(_d10w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-T")]
    public Node100<string> STJSrcGen_Deser_D10_W100_T() => JsonSerializer.Deserialize(_d10w100t_s, FactorialJsonContext.Default.Node100String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-T")]
    public Node100<string> Newtonsoft_Deser_D10_W100_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node100<string>>(_d10w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-T")]
    public Node100<string> SpanJson_Deser_D10_W100_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node100<string>>(_d10w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-T")]
    public Node100<string> Utf8Json_Deser_D10_W100_T() => Utf8Json.JsonSerializer.Deserialize<Node100<string>>(_d10w100t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W100-T")]
    public string STJRefGen_Ser_D10_W100_T() => JsonSerializer.Serialize(_d10w100t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-T")]
    public string STJSrcGen_Ser_D10_W100_T() => JsonSerializer.Serialize(_d10w100t, FactorialJsonContext.Default.Node100String);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-T")]
    public string Newtonsoft_Ser_D10_W100_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w100t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-T")]
    public string SpanJson_Ser_D10_W100_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w100t);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-T")]
    public string Utf8Json_Ser_D10_W100_T() => Utf8Json.JsonSerializer.ToJsonString(_d10w100t);

    // ----- D10-W100-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-N")]
    public Node100<int> STJRefGen_Deser_D10_W100_N() => JsonSerializer.Deserialize<Node100<int>>(_d10w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-N")]
    public Node100<int> STJSrcGen_Deser_D10_W100_N() => JsonSerializer.Deserialize(_d10w100n_s, FactorialJsonContext.Default.Node100Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-N")]
    public Node100<int> Newtonsoft_Deser_D10_W100_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node100<int>>(_d10w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-N")]
    public Node100<int> SpanJson_Deser_D10_W100_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node100<int>>(_d10w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-N")]
    public Node100<int> Utf8Json_Deser_D10_W100_N() => Utf8Json.JsonSerializer.Deserialize<Node100<int>>(_d10w100n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W100-N")]
    public string STJRefGen_Ser_D10_W100_N() => JsonSerializer.Serialize(_d10w100n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-N")]
    public string STJSrcGen_Ser_D10_W100_N() => JsonSerializer.Serialize(_d10w100n, FactorialJsonContext.Default.Node100Int32);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-N")]
    public string Newtonsoft_Ser_D10_W100_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w100n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-N")]
    public string SpanJson_Ser_D10_W100_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w100n);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-N")]
    public string Utf8Json_Ser_D10_W100_N() => Utf8Json.JsonSerializer.ToJsonString(_d10w100n);

    // ----- D10-W100-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-B")]
    public Node100<bool> STJRefGen_Deser_D10_W100_B() => JsonSerializer.Deserialize<Node100<bool>>(_d10w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-B")]
    public Node100<bool> STJSrcGen_Deser_D10_W100_B() => JsonSerializer.Deserialize(_d10w100b_s, FactorialJsonContext.Default.Node100Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-B")]
    public Node100<bool> Newtonsoft_Deser_D10_W100_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node100<bool>>(_d10w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-B")]
    public Node100<bool> SpanJson_Deser_D10_W100_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node100<bool>>(_d10w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D10-W100-B")]
    public Node100<bool> Utf8Json_Deser_D10_W100_B() => Utf8Json.JsonSerializer.Deserialize<Node100<bool>>(_d10w100b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D10-W100-B")]
    public string STJRefGen_Ser_D10_W100_B() => JsonSerializer.Serialize(_d10w100b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-B")]
    public string STJSrcGen_Ser_D10_W100_B() => JsonSerializer.Serialize(_d10w100b, FactorialJsonContext.Default.Node100Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-B")]
    public string Newtonsoft_Ser_D10_W100_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d10w100b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-B")]
    public string SpanJson_Ser_D10_W100_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d10w100b);
    [Benchmark, BenchmarkCategory("Serialize-D10-W100-B")]
    public string Utf8Json_Ser_D10_W100_B() => Utf8Json.JsonSerializer.ToJsonString(_d10w100b);

    // ==================== Depth 20 ====================

    // ----- D20-W5-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-T")]
    public Node5<string> STJRefGen_Deser_D20_W5_T() => JsonSerializer.Deserialize<Node5<string>>(_d20w5t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-T")]
    public Node5<string> STJSrcGen_Deser_D20_W5_T() => JsonSerializer.Deserialize(_d20w5t_s, FactorialJsonContext.Default.Node5String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-T")]
    public Node5<string> Newtonsoft_Deser_D20_W5_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node5<string>>(_d20w5t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-T")]
    public Node5<string> SpanJson_Deser_D20_W5_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node5<string>>(_d20w5t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-T")]
    public Node5<string> Utf8Json_Deser_D20_W5_T() => Utf8Json.JsonSerializer.Deserialize<Node5<string>>(_d20w5t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D20-W5-T")]
    public string STJRefGen_Ser_D20_W5_T() => JsonSerializer.Serialize(_d20w5t);
    [Benchmark, BenchmarkCategory("Serialize-D20-W5-T")]
    public string STJSrcGen_Ser_D20_W5_T() => JsonSerializer.Serialize(_d20w5t, FactorialJsonContext.Default.Node5String);
    [Benchmark, BenchmarkCategory("Serialize-D20-W5-T")]
    public string Newtonsoft_Ser_D20_W5_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d20w5t);
    [Benchmark, BenchmarkCategory("Serialize-D20-W5-T")]
    public string SpanJson_Ser_D20_W5_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d20w5t);
    [Benchmark, BenchmarkCategory("Serialize-D20-W5-T")]
    public string Utf8Json_Ser_D20_W5_T() => Utf8Json.JsonSerializer.ToJsonString(_d20w5t);

    // ----- D20-W5-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-N")]
    public Node5<int> STJRefGen_Deser_D20_W5_N() => JsonSerializer.Deserialize<Node5<int>>(_d20w5n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-N")]
    public Node5<int> STJSrcGen_Deser_D20_W5_N() => JsonSerializer.Deserialize(_d20w5n_s, FactorialJsonContext.Default.Node5Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-N")]
    public Node5<int> Newtonsoft_Deser_D20_W5_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node5<int>>(_d20w5n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-N")]
    public Node5<int> SpanJson_Deser_D20_W5_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node5<int>>(_d20w5n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-N")]
    public Node5<int> Utf8Json_Deser_D20_W5_N() => Utf8Json.JsonSerializer.Deserialize<Node5<int>>(_d20w5n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D20-W5-N")]
    public string STJRefGen_Ser_D20_W5_N() => JsonSerializer.Serialize(_d20w5n);
    [Benchmark, BenchmarkCategory("Serialize-D20-W5-N")]
    public string STJSrcGen_Ser_D20_W5_N() => JsonSerializer.Serialize(_d20w5n, FactorialJsonContext.Default.Node5Int32);
    [Benchmark, BenchmarkCategory("Serialize-D20-W5-N")]
    public string Newtonsoft_Ser_D20_W5_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d20w5n);
    [Benchmark, BenchmarkCategory("Serialize-D20-W5-N")]
    public string SpanJson_Ser_D20_W5_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d20w5n);
    [Benchmark, BenchmarkCategory("Serialize-D20-W5-N")]
    public string Utf8Json_Ser_D20_W5_N() => Utf8Json.JsonSerializer.ToJsonString(_d20w5n);

    // ----- D20-W5-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-B")]
    public Node5<bool> STJRefGen_Deser_D20_W5_B() => JsonSerializer.Deserialize<Node5<bool>>(_d20w5b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-B")]
    public Node5<bool> STJSrcGen_Deser_D20_W5_B() => JsonSerializer.Deserialize(_d20w5b_s, FactorialJsonContext.Default.Node5Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-B")]
    public Node5<bool> Newtonsoft_Deser_D20_W5_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node5<bool>>(_d20w5b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-B")]
    public Node5<bool> SpanJson_Deser_D20_W5_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node5<bool>>(_d20w5b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W5-B")]
    public Node5<bool> Utf8Json_Deser_D20_W5_B() => Utf8Json.JsonSerializer.Deserialize<Node5<bool>>(_d20w5b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D20-W5-B")]
    public string STJRefGen_Ser_D20_W5_B() => JsonSerializer.Serialize(_d20w5b);
    [Benchmark, BenchmarkCategory("Serialize-D20-W5-B")]
    public string STJSrcGen_Ser_D20_W5_B() => JsonSerializer.Serialize(_d20w5b, FactorialJsonContext.Default.Node5Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D20-W5-B")]
    public string Newtonsoft_Ser_D20_W5_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d20w5b);
    [Benchmark, BenchmarkCategory("Serialize-D20-W5-B")]
    public string SpanJson_Ser_D20_W5_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d20w5b);
    [Benchmark, BenchmarkCategory("Serialize-D20-W5-B")]
    public string Utf8Json_Ser_D20_W5_B() => Utf8Json.JsonSerializer.ToJsonString(_d20w5b);

    // ----- D20-W20-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-T")]
    public Node20<string> STJRefGen_Deser_D20_W20_T() => JsonSerializer.Deserialize<Node20<string>>(_d20w20t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-T")]
    public Node20<string> STJSrcGen_Deser_D20_W20_T() => JsonSerializer.Deserialize(_d20w20t_s, FactorialJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-T")]
    public Node20<string> Newtonsoft_Deser_D20_W20_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(_d20w20t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-T")]
    public Node20<string> SpanJson_Deser_D20_W20_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<string>>(_d20w20t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-T")]
    public Node20<string> Utf8Json_Deser_D20_W20_T() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_d20w20t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D20-W20-T")]
    public string STJRefGen_Ser_D20_W20_T() => JsonSerializer.Serialize(_d20w20t);
    [Benchmark, BenchmarkCategory("Serialize-D20-W20-T")]
    public string STJSrcGen_Ser_D20_W20_T() => JsonSerializer.Serialize(_d20w20t, FactorialJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-D20-W20-T")]
    public string Newtonsoft_Ser_D20_W20_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d20w20t);
    [Benchmark, BenchmarkCategory("Serialize-D20-W20-T")]
    public string SpanJson_Ser_D20_W20_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d20w20t);
    [Benchmark, BenchmarkCategory("Serialize-D20-W20-T")]
    public string Utf8Json_Ser_D20_W20_T() => Utf8Json.JsonSerializer.ToJsonString(_d20w20t);

    // ----- D20-W20-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-N")]
    public Node20<int> STJRefGen_Deser_D20_W20_N() => JsonSerializer.Deserialize<Node20<int>>(_d20w20n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-N")]
    public Node20<int> STJSrcGen_Deser_D20_W20_N() => JsonSerializer.Deserialize(_d20w20n_s, FactorialJsonContext.Default.Node20Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-N")]
    public Node20<int> Newtonsoft_Deser_D20_W20_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<int>>(_d20w20n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-N")]
    public Node20<int> SpanJson_Deser_D20_W20_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<int>>(_d20w20n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-N")]
    public Node20<int> Utf8Json_Deser_D20_W20_N() => Utf8Json.JsonSerializer.Deserialize<Node20<int>>(_d20w20n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D20-W20-N")]
    public string STJRefGen_Ser_D20_W20_N() => JsonSerializer.Serialize(_d20w20n);
    [Benchmark, BenchmarkCategory("Serialize-D20-W20-N")]
    public string STJSrcGen_Ser_D20_W20_N() => JsonSerializer.Serialize(_d20w20n, FactorialJsonContext.Default.Node20Int32);
    [Benchmark, BenchmarkCategory("Serialize-D20-W20-N")]
    public string Newtonsoft_Ser_D20_W20_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d20w20n);
    [Benchmark, BenchmarkCategory("Serialize-D20-W20-N")]
    public string SpanJson_Ser_D20_W20_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d20w20n);
    [Benchmark, BenchmarkCategory("Serialize-D20-W20-N")]
    public string Utf8Json_Ser_D20_W20_N() => Utf8Json.JsonSerializer.ToJsonString(_d20w20n);

    // ----- D20-W20-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-B")]
    public Node20<bool> STJRefGen_Deser_D20_W20_B() => JsonSerializer.Deserialize<Node20<bool>>(_d20w20b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-B")]
    public Node20<bool> STJSrcGen_Deser_D20_W20_B() => JsonSerializer.Deserialize(_d20w20b_s, FactorialJsonContext.Default.Node20Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-B")]
    public Node20<bool> Newtonsoft_Deser_D20_W20_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<bool>>(_d20w20b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-B")]
    public Node20<bool> SpanJson_Deser_D20_W20_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node20<bool>>(_d20w20b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W20-B")]
    public Node20<bool> Utf8Json_Deser_D20_W20_B() => Utf8Json.JsonSerializer.Deserialize<Node20<bool>>(_d20w20b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D20-W20-B")]
    public string STJRefGen_Ser_D20_W20_B() => JsonSerializer.Serialize(_d20w20b);
    [Benchmark, BenchmarkCategory("Serialize-D20-W20-B")]
    public string STJSrcGen_Ser_D20_W20_B() => JsonSerializer.Serialize(_d20w20b, FactorialJsonContext.Default.Node20Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D20-W20-B")]
    public string Newtonsoft_Ser_D20_W20_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d20w20b);
    [Benchmark, BenchmarkCategory("Serialize-D20-W20-B")]
    public string SpanJson_Ser_D20_W20_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d20w20b);
    [Benchmark, BenchmarkCategory("Serialize-D20-W20-B")]
    public string Utf8Json_Ser_D20_W20_B() => Utf8Json.JsonSerializer.ToJsonString(_d20w20b);

    // ----- D20-W50-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-T")]
    public Node50<string> STJRefGen_Deser_D20_W50_T() => JsonSerializer.Deserialize<Node50<string>>(_d20w50t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-T")]
    public Node50<string> STJSrcGen_Deser_D20_W50_T() => JsonSerializer.Deserialize(_d20w50t_s, FactorialJsonContext.Default.Node50String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-T")]
    public Node50<string> Newtonsoft_Deser_D20_W50_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node50<string>>(_d20w50t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-T")]
    public Node50<string> SpanJson_Deser_D20_W50_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node50<string>>(_d20w50t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-T")]
    public Node50<string> Utf8Json_Deser_D20_W50_T() => Utf8Json.JsonSerializer.Deserialize<Node50<string>>(_d20w50t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D20-W50-T")]
    public string STJRefGen_Ser_D20_W50_T() => JsonSerializer.Serialize(_d20w50t);
    [Benchmark, BenchmarkCategory("Serialize-D20-W50-T")]
    public string STJSrcGen_Ser_D20_W50_T() => JsonSerializer.Serialize(_d20w50t, FactorialJsonContext.Default.Node50String);
    [Benchmark, BenchmarkCategory("Serialize-D20-W50-T")]
    public string Newtonsoft_Ser_D20_W50_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d20w50t);
    [Benchmark, BenchmarkCategory("Serialize-D20-W50-T")]
    public string SpanJson_Ser_D20_W50_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d20w50t);
    [Benchmark, BenchmarkCategory("Serialize-D20-W50-T")]
    public string Utf8Json_Ser_D20_W50_T() => Utf8Json.JsonSerializer.ToJsonString(_d20w50t);

    // ----- D20-W50-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-N")]
    public Node50<int> STJRefGen_Deser_D20_W50_N() => JsonSerializer.Deserialize<Node50<int>>(_d20w50n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-N")]
    public Node50<int> STJSrcGen_Deser_D20_W50_N() => JsonSerializer.Deserialize(_d20w50n_s, FactorialJsonContext.Default.Node50Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-N")]
    public Node50<int> Newtonsoft_Deser_D20_W50_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node50<int>>(_d20w50n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-N")]
    public Node50<int> SpanJson_Deser_D20_W50_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node50<int>>(_d20w50n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-N")]
    public Node50<int> Utf8Json_Deser_D20_W50_N() => Utf8Json.JsonSerializer.Deserialize<Node50<int>>(_d20w50n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D20-W50-N")]
    public string STJRefGen_Ser_D20_W50_N() => JsonSerializer.Serialize(_d20w50n);
    [Benchmark, BenchmarkCategory("Serialize-D20-W50-N")]
    public string STJSrcGen_Ser_D20_W50_N() => JsonSerializer.Serialize(_d20w50n, FactorialJsonContext.Default.Node50Int32);
    [Benchmark, BenchmarkCategory("Serialize-D20-W50-N")]
    public string Newtonsoft_Ser_D20_W50_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d20w50n);
    [Benchmark, BenchmarkCategory("Serialize-D20-W50-N")]
    public string SpanJson_Ser_D20_W50_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d20w50n);
    [Benchmark, BenchmarkCategory("Serialize-D20-W50-N")]
    public string Utf8Json_Ser_D20_W50_N() => Utf8Json.JsonSerializer.ToJsonString(_d20w50n);

    // ----- D20-W50-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-B")]
    public Node50<bool> STJRefGen_Deser_D20_W50_B() => JsonSerializer.Deserialize<Node50<bool>>(_d20w50b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-B")]
    public Node50<bool> STJSrcGen_Deser_D20_W50_B() => JsonSerializer.Deserialize(_d20w50b_s, FactorialJsonContext.Default.Node50Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-B")]
    public Node50<bool> Newtonsoft_Deser_D20_W50_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node50<bool>>(_d20w50b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-B")]
    public Node50<bool> SpanJson_Deser_D20_W50_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node50<bool>>(_d20w50b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W50-B")]
    public Node50<bool> Utf8Json_Deser_D20_W50_B() => Utf8Json.JsonSerializer.Deserialize<Node50<bool>>(_d20w50b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D20-W50-B")]
    public string STJRefGen_Ser_D20_W50_B() => JsonSerializer.Serialize(_d20w50b);
    [Benchmark, BenchmarkCategory("Serialize-D20-W50-B")]
    public string STJSrcGen_Ser_D20_W50_B() => JsonSerializer.Serialize(_d20w50b, FactorialJsonContext.Default.Node50Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D20-W50-B")]
    public string Newtonsoft_Ser_D20_W50_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d20w50b);
    [Benchmark, BenchmarkCategory("Serialize-D20-W50-B")]
    public string SpanJson_Ser_D20_W50_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d20w50b);
    [Benchmark, BenchmarkCategory("Serialize-D20-W50-B")]
    public string Utf8Json_Ser_D20_W50_B() => Utf8Json.JsonSerializer.ToJsonString(_d20w50b);

    // ----- D20-W100-T -----

    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-T")]
    public Node100<string> STJRefGen_Deser_D20_W100_T() => JsonSerializer.Deserialize<Node100<string>>(_d20w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-T")]
    public Node100<string> STJSrcGen_Deser_D20_W100_T() => JsonSerializer.Deserialize(_d20w100t_s, FactorialJsonContext.Default.Node100String)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-T")]
    public Node100<string> Newtonsoft_Deser_D20_W100_T() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node100<string>>(_d20w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-T")]
    public Node100<string> SpanJson_Deser_D20_W100_T() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node100<string>>(_d20w100t_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-T")]
    public Node100<string> Utf8Json_Deser_D20_W100_T() => Utf8Json.JsonSerializer.Deserialize<Node100<string>>(_d20w100t_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D20-W100-T")]
    public string STJRefGen_Ser_D20_W100_T() => JsonSerializer.Serialize(_d20w100t);
    [Benchmark, BenchmarkCategory("Serialize-D20-W100-T")]
    public string STJSrcGen_Ser_D20_W100_T() => JsonSerializer.Serialize(_d20w100t, FactorialJsonContext.Default.Node100String);
    [Benchmark, BenchmarkCategory("Serialize-D20-W100-T")]
    public string Newtonsoft_Ser_D20_W100_T() => Newtonsoft.Json.JsonConvert.SerializeObject(_d20w100t);
    [Benchmark, BenchmarkCategory("Serialize-D20-W100-T")]
    public string SpanJson_Ser_D20_W100_T() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d20w100t);
    [Benchmark, BenchmarkCategory("Serialize-D20-W100-T")]
    public string Utf8Json_Ser_D20_W100_T() => Utf8Json.JsonSerializer.ToJsonString(_d20w100t);

    // ----- D20-W100-N -----

    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-N")]
    public Node100<int> STJRefGen_Deser_D20_W100_N() => JsonSerializer.Deserialize<Node100<int>>(_d20w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-N")]
    public Node100<int> STJSrcGen_Deser_D20_W100_N() => JsonSerializer.Deserialize(_d20w100n_s, FactorialJsonContext.Default.Node100Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-N")]
    public Node100<int> Newtonsoft_Deser_D20_W100_N() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node100<int>>(_d20w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-N")]
    public Node100<int> SpanJson_Deser_D20_W100_N() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node100<int>>(_d20w100n_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-N")]
    public Node100<int> Utf8Json_Deser_D20_W100_N() => Utf8Json.JsonSerializer.Deserialize<Node100<int>>(_d20w100n_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D20-W100-N")]
    public string STJRefGen_Ser_D20_W100_N() => JsonSerializer.Serialize(_d20w100n);
    [Benchmark, BenchmarkCategory("Serialize-D20-W100-N")]
    public string STJSrcGen_Ser_D20_W100_N() => JsonSerializer.Serialize(_d20w100n, FactorialJsonContext.Default.Node100Int32);
    [Benchmark, BenchmarkCategory("Serialize-D20-W100-N")]
    public string Newtonsoft_Ser_D20_W100_N() => Newtonsoft.Json.JsonConvert.SerializeObject(_d20w100n);
    [Benchmark, BenchmarkCategory("Serialize-D20-W100-N")]
    public string SpanJson_Ser_D20_W100_N() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d20w100n);
    [Benchmark, BenchmarkCategory("Serialize-D20-W100-N")]
    public string Utf8Json_Ser_D20_W100_N() => Utf8Json.JsonSerializer.ToJsonString(_d20w100n);

    // ----- D20-W100-B -----

    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-B")]
    public Node100<bool> STJRefGen_Deser_D20_W100_B() => JsonSerializer.Deserialize<Node100<bool>>(_d20w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-B")]
    public Node100<bool> STJSrcGen_Deser_D20_W100_B() => JsonSerializer.Deserialize(_d20w100b_s, FactorialJsonContext.Default.Node100Boolean)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-B")]
    public Node100<bool> Newtonsoft_Deser_D20_W100_B() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node100<bool>>(_d20w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-B")]
    public Node100<bool> SpanJson_Deser_D20_W100_B() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<Node100<bool>>(_d20w100b_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-D20-W100-B")]
    public Node100<bool> Utf8Json_Deser_D20_W100_B() => Utf8Json.JsonSerializer.Deserialize<Node100<bool>>(_d20w100b_s)!;

    [Benchmark, BenchmarkCategory("Serialize-D20-W100-B")]
    public string STJRefGen_Ser_D20_W100_B() => JsonSerializer.Serialize(_d20w100b);
    [Benchmark, BenchmarkCategory("Serialize-D20-W100-B")]
    public string STJSrcGen_Ser_D20_W100_B() => JsonSerializer.Serialize(_d20w100b, FactorialJsonContext.Default.Node100Boolean);
    [Benchmark, BenchmarkCategory("Serialize-D20-W100-B")]
    public string Newtonsoft_Ser_D20_W100_B() => Newtonsoft.Json.JsonConvert.SerializeObject(_d20w100b);
    [Benchmark, BenchmarkCategory("Serialize-D20-W100-B")]
    public string SpanJson_Ser_D20_W100_B() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_d20w100b);
    [Benchmark, BenchmarkCategory("Serialize-D20-W100-B")]
    public string Utf8Json_Ser_D20_W100_B() => Utf8Json.JsonSerializer.ToJsonString(_d20w100b);
}
