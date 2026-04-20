using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Size isolation benchmark: varies object count via log10 scaling (5 levels), string I/O.
/// All levels use ItemsWrapper (Count >= 10).
/// Baseline: D5, W20, Textual, Object-only, ASCII, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class SizeIsolationStringBench
{
    private string _c10_s = null!; private ItemsWrapper<Node20<string>> _c10 = null!;
    private string _c100_s = null!; private ItemsWrapper<Node20<string>> _c100 = null!;
    private string _c1k_s = null!; private ItemsWrapper<Node20<string>> _c1k = null!;
    private string _c10k_s = null!; private ItemsWrapper<Node20<string>> _c10k = null!;
    private string _c100k_s = null!; private ItemsWrapper<Node20<string>> _c100k = null!;

    [GlobalSetup]
    public void Setup()
    {
        _c10_s = Load("C10"); _c10 = JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c10_s)!;
        _c100_s = Load("C100"); _c100 = JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c100_s)!;
        _c1k_s = Load("C1K"); _c1k = JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c1k_s)!;
        _c10k_s = Load("C10K"); _c10k = JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c10k_s)!;
        _c100k_s = Load("C100K"); _c100k = JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c100k_s)!;
    }

    private static string Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoSize", $"{id}.json");
        return File.ReadAllText(path);
    }

    // ===================== C10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-C10")]
    public ItemsWrapper<Node20<string>> STJRefGen_Deser_C10() => JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C10")]
    public ItemsWrapper<Node20<string>> STJSrcGen_Deser_C10() => JsonSerializer.Deserialize(_c10_s, IsolationJsonContext.Default.ItemsWrapperNode20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-C10")]
    public ItemsWrapper<Node20<string>> Newtonsoft_Deser_C10() => Newtonsoft.Json.JsonConvert.DeserializeObject<ItemsWrapper<Node20<string>>>(_c10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C10")]
    public ItemsWrapper<Node20<string>> SpanJson_Deser_C10() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<ItemsWrapper<Node20<string>>>(_c10_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C10")]
    public ItemsWrapper<Node20<string>> Utf8Json_Deser_C10() => Utf8Json.JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c10_s)!;

    [Benchmark, BenchmarkCategory("Serialize-C10")]
    public string STJRefGen_Ser_C10() => JsonSerializer.Serialize(_c10);
    [Benchmark, BenchmarkCategory("Serialize-C10")]
    public string STJSrcGen_Ser_C10() => JsonSerializer.Serialize(_c10, IsolationJsonContext.Default.ItemsWrapperNode20String);
    [Benchmark, BenchmarkCategory("Serialize-C10")]
    public string Newtonsoft_Ser_C10() => Newtonsoft.Json.JsonConvert.SerializeObject(_c10);
    [Benchmark, BenchmarkCategory("Serialize-C10")]
    public string SpanJson_Ser_C10() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_c10);
    [Benchmark, BenchmarkCategory("Serialize-C10")]
    public string Utf8Json_Ser_C10() => Utf8Json.JsonSerializer.ToJsonString(_c10);

    // ===================== C100 =====================

    [Benchmark, BenchmarkCategory("Deserialize-C100")]
    public ItemsWrapper<Node20<string>> STJRefGen_Deser_C100() => JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c100_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C100")]
    public ItemsWrapper<Node20<string>> STJSrcGen_Deser_C100() => JsonSerializer.Deserialize(_c100_s, IsolationJsonContext.Default.ItemsWrapperNode20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-C100")]
    public ItemsWrapper<Node20<string>> Newtonsoft_Deser_C100() => Newtonsoft.Json.JsonConvert.DeserializeObject<ItemsWrapper<Node20<string>>>(_c100_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C100")]
    public ItemsWrapper<Node20<string>> SpanJson_Deser_C100() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<ItemsWrapper<Node20<string>>>(_c100_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C100")]
    public ItemsWrapper<Node20<string>> Utf8Json_Deser_C100() => Utf8Json.JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c100_s)!;

    [Benchmark, BenchmarkCategory("Serialize-C100")]
    public string STJRefGen_Ser_C100() => JsonSerializer.Serialize(_c100);
    [Benchmark, BenchmarkCategory("Serialize-C100")]
    public string STJSrcGen_Ser_C100() => JsonSerializer.Serialize(_c100, IsolationJsonContext.Default.ItemsWrapperNode20String);
    [Benchmark, BenchmarkCategory("Serialize-C100")]
    public string Newtonsoft_Ser_C100() => Newtonsoft.Json.JsonConvert.SerializeObject(_c100);
    [Benchmark, BenchmarkCategory("Serialize-C100")]
    public string SpanJson_Ser_C100() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_c100);
    [Benchmark, BenchmarkCategory("Serialize-C100")]
    public string Utf8Json_Ser_C100() => Utf8Json.JsonSerializer.ToJsonString(_c100);

    // ===================== C1K =====================

    [Benchmark, BenchmarkCategory("Deserialize-C1K")]
    public ItemsWrapper<Node20<string>> STJRefGen_Deser_C1K() => JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c1k_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C1K")]
    public ItemsWrapper<Node20<string>> STJSrcGen_Deser_C1K() => JsonSerializer.Deserialize(_c1k_s, IsolationJsonContext.Default.ItemsWrapperNode20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-C1K")]
    public ItemsWrapper<Node20<string>> Newtonsoft_Deser_C1K() => Newtonsoft.Json.JsonConvert.DeserializeObject<ItemsWrapper<Node20<string>>>(_c1k_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C1K")]
    public ItemsWrapper<Node20<string>> SpanJson_Deser_C1K() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<ItemsWrapper<Node20<string>>>(_c1k_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C1K")]
    public ItemsWrapper<Node20<string>> Utf8Json_Deser_C1K() => Utf8Json.JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c1k_s)!;

    [Benchmark, BenchmarkCategory("Serialize-C1K")]
    public string STJRefGen_Ser_C1K() => JsonSerializer.Serialize(_c1k);
    [Benchmark, BenchmarkCategory("Serialize-C1K")]
    public string STJSrcGen_Ser_C1K() => JsonSerializer.Serialize(_c1k, IsolationJsonContext.Default.ItemsWrapperNode20String);
    [Benchmark, BenchmarkCategory("Serialize-C1K")]
    public string Newtonsoft_Ser_C1K() => Newtonsoft.Json.JsonConvert.SerializeObject(_c1k);
    [Benchmark, BenchmarkCategory("Serialize-C1K")]
    public string SpanJson_Ser_C1K() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_c1k);
    [Benchmark, BenchmarkCategory("Serialize-C1K")]
    public string Utf8Json_Ser_C1K() => Utf8Json.JsonSerializer.ToJsonString(_c1k);

    // ===================== C10K =====================

    [Benchmark, BenchmarkCategory("Deserialize-C10K")]
    public ItemsWrapper<Node20<string>> STJRefGen_Deser_C10K() => JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c10k_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C10K")]
    public ItemsWrapper<Node20<string>> STJSrcGen_Deser_C10K() => JsonSerializer.Deserialize(_c10k_s, IsolationJsonContext.Default.ItemsWrapperNode20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-C10K")]
    public ItemsWrapper<Node20<string>> Newtonsoft_Deser_C10K() => Newtonsoft.Json.JsonConvert.DeserializeObject<ItemsWrapper<Node20<string>>>(_c10k_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C10K")]
    public ItemsWrapper<Node20<string>> SpanJson_Deser_C10K() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<ItemsWrapper<Node20<string>>>(_c10k_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C10K")]
    public ItemsWrapper<Node20<string>> Utf8Json_Deser_C10K() => Utf8Json.JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c10k_s)!;

    [Benchmark, BenchmarkCategory("Serialize-C10K")]
    public string STJRefGen_Ser_C10K() => JsonSerializer.Serialize(_c10k);
    [Benchmark, BenchmarkCategory("Serialize-C10K")]
    public string STJSrcGen_Ser_C10K() => JsonSerializer.Serialize(_c10k, IsolationJsonContext.Default.ItemsWrapperNode20String);
    [Benchmark, BenchmarkCategory("Serialize-C10K")]
    public string Newtonsoft_Ser_C10K() => Newtonsoft.Json.JsonConvert.SerializeObject(_c10k);
    [Benchmark, BenchmarkCategory("Serialize-C10K")]
    public string SpanJson_Ser_C10K() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_c10k);
    [Benchmark, BenchmarkCategory("Serialize-C10K")]
    public string Utf8Json_Ser_C10K() => Utf8Json.JsonSerializer.ToJsonString(_c10k);

    // ===================== C100K =====================

    [Benchmark, BenchmarkCategory("Deserialize-C100K")]
    public ItemsWrapper<Node20<string>> STJRefGen_Deser_C100K() => JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c100k_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C100K")]
    public ItemsWrapper<Node20<string>> STJSrcGen_Deser_C100K() => JsonSerializer.Deserialize(_c100k_s, IsolationJsonContext.Default.ItemsWrapperNode20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-C100K")]
    public ItemsWrapper<Node20<string>> Newtonsoft_Deser_C100K() => Newtonsoft.Json.JsonConvert.DeserializeObject<ItemsWrapper<Node20<string>>>(_c100k_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C100K")]
    public ItemsWrapper<Node20<string>> SpanJson_Deser_C100K() => SpanJson.JsonSerializer.Generic.Utf16.Deserialize<ItemsWrapper<Node20<string>>>(_c100k_s)!;
    [Benchmark, BenchmarkCategory("Deserialize-C100K")]
    public ItemsWrapper<Node20<string>> Utf8Json_Deser_C100K() => Utf8Json.JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c100k_s)!;

    [Benchmark, BenchmarkCategory("Serialize-C100K")]
    public string STJRefGen_Ser_C100K() => JsonSerializer.Serialize(_c100k);
    [Benchmark, BenchmarkCategory("Serialize-C100K")]
    public string STJSrcGen_Ser_C100K() => JsonSerializer.Serialize(_c100k, IsolationJsonContext.Default.ItemsWrapperNode20String);
    [Benchmark, BenchmarkCategory("Serialize-C100K")]
    public string Newtonsoft_Ser_C100K() => Newtonsoft.Json.JsonConvert.SerializeObject(_c100k);
    [Benchmark, BenchmarkCategory("Serialize-C100K")]
    public string SpanJson_Ser_C100K() => SpanJson.JsonSerializer.Generic.Utf16.Serialize(_c100k);
    [Benchmark, BenchmarkCategory("Serialize-C100K")]
    public string Utf8Json_Ser_C100K() => Utf8Json.JsonSerializer.ToJsonString(_c100k);
}
