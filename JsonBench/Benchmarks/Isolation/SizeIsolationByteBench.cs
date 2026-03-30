using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Size isolation benchmark: varies object count via log10 scaling (5 levels), byte[] I/O.
/// All levels use ItemsWrapper (Count >= 10).
/// Baseline: D5, W20, Textual, Object-only, ASCII, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class SizeIsolationByteBench
{
    private byte[] _c10_b = null!; private ItemsWrapper<Node20<string>> _c10 = null!;
    private byte[] _c100_b = null!; private ItemsWrapper<Node20<string>> _c100 = null!;
    private byte[] _c1k_b = null!; private ItemsWrapper<Node20<string>> _c1k = null!;
    private byte[] _c10k_b = null!; private ItemsWrapper<Node20<string>> _c10k = null!;
    private byte[] _c100k_b = null!; private ItemsWrapper<Node20<string>> _c100k = null!;

    [GlobalSetup]
    public void Setup()
    {
        _c10_b = Load("C10"); _c10 = JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c10_b)!;
        _c100_b = Load("C100"); _c100 = JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c100_b)!;
        _c1k_b = Load("C1K"); _c1k = JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c1k_b)!;
        _c10k_b = Load("C10K"); _c10k = JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c10k_b)!;
        _c100k_b = Load("C100K"); _c100k = JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c100k_b)!;
    }

    private static byte[] Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoSize", $"{id}.json");
        return File.ReadAllBytes(path);
    }

    // ===================== C10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-C10")]
    public ItemsWrapper<Node20<string>> STJ_Deser_C10() => JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C10")]
    public ItemsWrapper<Node20<string>> Newtonsoft_Deser_C10() => Newtonsoft.Json.JsonConvert.DeserializeObject<ItemsWrapper<Node20<string>>>(Encoding.UTF8.GetString(_c10_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-C10")]
    public ItemsWrapper<Node20<string>> SpanJson_Deser_C10() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<ItemsWrapper<Node20<string>>>(_c10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C10")]
    public ItemsWrapper<Node20<string>> Utf8Json_Deser_C10() => Utf8Json.JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C10")]
    public ItemsWrapper<Node20<string>> Jil_Deser_C10() => Jil.JSON.Deserialize<ItemsWrapper<Node20<string>>>(Encoding.UTF8.GetString(_c10_b))!;

    [Benchmark, BenchmarkCategory("Serialize-C10")]
    public byte[] STJ_Ser_C10() => JsonSerializer.SerializeToUtf8Bytes(_c10);
    [Benchmark, BenchmarkCategory("Serialize-C10")]
    public byte[] Newtonsoft_Ser_C10() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_c10));
    [Benchmark, BenchmarkCategory("Serialize-C10")]
    public byte[] SpanJson_Ser_C10() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_c10);
    [Benchmark, BenchmarkCategory("Serialize-C10")]
    public byte[] Utf8Json_Ser_C10() => Utf8Json.JsonSerializer.Serialize(_c10);
    [Benchmark, BenchmarkCategory("Serialize-C10")]
    public byte[] Jil_Ser_C10() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_c10));

    // ===================== C100 =====================

    [Benchmark, BenchmarkCategory("Deserialize-C100")]
    public ItemsWrapper<Node20<string>> STJ_Deser_C100() => JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C100")]
    public ItemsWrapper<Node20<string>> Newtonsoft_Deser_C100() => Newtonsoft.Json.JsonConvert.DeserializeObject<ItemsWrapper<Node20<string>>>(Encoding.UTF8.GetString(_c100_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-C100")]
    public ItemsWrapper<Node20<string>> SpanJson_Deser_C100() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<ItemsWrapper<Node20<string>>>(_c100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C100")]
    public ItemsWrapper<Node20<string>> Utf8Json_Deser_C100() => Utf8Json.JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c100_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C100")]
    public ItemsWrapper<Node20<string>> Jil_Deser_C100() => Jil.JSON.Deserialize<ItemsWrapper<Node20<string>>>(Encoding.UTF8.GetString(_c100_b))!;

    [Benchmark, BenchmarkCategory("Serialize-C100")]
    public byte[] STJ_Ser_C100() => JsonSerializer.SerializeToUtf8Bytes(_c100);
    [Benchmark, BenchmarkCategory("Serialize-C100")]
    public byte[] Newtonsoft_Ser_C100() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_c100));
    [Benchmark, BenchmarkCategory("Serialize-C100")]
    public byte[] SpanJson_Ser_C100() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_c100);
    [Benchmark, BenchmarkCategory("Serialize-C100")]
    public byte[] Utf8Json_Ser_C100() => Utf8Json.JsonSerializer.Serialize(_c100);
    [Benchmark, BenchmarkCategory("Serialize-C100")]
    public byte[] Jil_Ser_C100() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_c100));

    // ===================== C1K =====================

    [Benchmark, BenchmarkCategory("Deserialize-C1K")]
    public ItemsWrapper<Node20<string>> STJ_Deser_C1K() => JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c1k_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C1K")]
    public ItemsWrapper<Node20<string>> Newtonsoft_Deser_C1K() => Newtonsoft.Json.JsonConvert.DeserializeObject<ItemsWrapper<Node20<string>>>(Encoding.UTF8.GetString(_c1k_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-C1K")]
    public ItemsWrapper<Node20<string>> SpanJson_Deser_C1K() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<ItemsWrapper<Node20<string>>>(_c1k_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C1K")]
    public ItemsWrapper<Node20<string>> Utf8Json_Deser_C1K() => Utf8Json.JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c1k_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C1K")]
    public ItemsWrapper<Node20<string>> Jil_Deser_C1K() => Jil.JSON.Deserialize<ItemsWrapper<Node20<string>>>(Encoding.UTF8.GetString(_c1k_b))!;

    [Benchmark, BenchmarkCategory("Serialize-C1K")]
    public byte[] STJ_Ser_C1K() => JsonSerializer.SerializeToUtf8Bytes(_c1k);
    [Benchmark, BenchmarkCategory("Serialize-C1K")]
    public byte[] Newtonsoft_Ser_C1K() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_c1k));
    [Benchmark, BenchmarkCategory("Serialize-C1K")]
    public byte[] SpanJson_Ser_C1K() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_c1k);
    [Benchmark, BenchmarkCategory("Serialize-C1K")]
    public byte[] Utf8Json_Ser_C1K() => Utf8Json.JsonSerializer.Serialize(_c1k);
    [Benchmark, BenchmarkCategory("Serialize-C1K")]
    public byte[] Jil_Ser_C1K() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_c1k));

    // ===================== C10K =====================

    [Benchmark, BenchmarkCategory("Deserialize-C10K")]
    public ItemsWrapper<Node20<string>> STJ_Deser_C10K() => JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c10k_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C10K")]
    public ItemsWrapper<Node20<string>> Newtonsoft_Deser_C10K() => Newtonsoft.Json.JsonConvert.DeserializeObject<ItemsWrapper<Node20<string>>>(Encoding.UTF8.GetString(_c10k_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-C10K")]
    public ItemsWrapper<Node20<string>> SpanJson_Deser_C10K() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<ItemsWrapper<Node20<string>>>(_c10k_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C10K")]
    public ItemsWrapper<Node20<string>> Utf8Json_Deser_C10K() => Utf8Json.JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c10k_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C10K")]
    public ItemsWrapper<Node20<string>> Jil_Deser_C10K() => Jil.JSON.Deserialize<ItemsWrapper<Node20<string>>>(Encoding.UTF8.GetString(_c10k_b))!;

    [Benchmark, BenchmarkCategory("Serialize-C10K")]
    public byte[] STJ_Ser_C10K() => JsonSerializer.SerializeToUtf8Bytes(_c10k);
    [Benchmark, BenchmarkCategory("Serialize-C10K")]
    public byte[] Newtonsoft_Ser_C10K() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_c10k));
    [Benchmark, BenchmarkCategory("Serialize-C10K")]
    public byte[] SpanJson_Ser_C10K() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_c10k);
    [Benchmark, BenchmarkCategory("Serialize-C10K")]
    public byte[] Utf8Json_Ser_C10K() => Utf8Json.JsonSerializer.Serialize(_c10k);
    [Benchmark, BenchmarkCategory("Serialize-C10K")]
    public byte[] Jil_Ser_C10K() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_c10k));

    // ===================== C100K =====================

    [Benchmark, BenchmarkCategory("Deserialize-C100K")]
    public ItemsWrapper<Node20<string>> STJ_Deser_C100K() => JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c100k_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C100K")]
    public ItemsWrapper<Node20<string>> Newtonsoft_Deser_C100K() => Newtonsoft.Json.JsonConvert.DeserializeObject<ItemsWrapper<Node20<string>>>(Encoding.UTF8.GetString(_c100k_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-C100K")]
    public ItemsWrapper<Node20<string>> SpanJson_Deser_C100K() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<ItemsWrapper<Node20<string>>>(_c100k_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C100K")]
    public ItemsWrapper<Node20<string>> Utf8Json_Deser_C100K() => Utf8Json.JsonSerializer.Deserialize<ItemsWrapper<Node20<string>>>(_c100k_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-C100K")]
    public ItemsWrapper<Node20<string>> Jil_Deser_C100K() => Jil.JSON.Deserialize<ItemsWrapper<Node20<string>>>(Encoding.UTF8.GetString(_c100k_b))!;

    [Benchmark, BenchmarkCategory("Serialize-C100K")]
    public byte[] STJ_Ser_C100K() => JsonSerializer.SerializeToUtf8Bytes(_c100k);
    [Benchmark, BenchmarkCategory("Serialize-C100K")]
    public byte[] Newtonsoft_Ser_C100K() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_c100k));
    [Benchmark, BenchmarkCategory("Serialize-C100K")]
    public byte[] SpanJson_Ser_C100K() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_c100k);
    [Benchmark, BenchmarkCategory("Serialize-C100K")]
    public byte[] Utf8Json_Ser_C100K() => Utf8Json.JsonSerializer.Serialize(_c100k);
    [Benchmark, BenchmarkCategory("Serialize-C100K")]
    public byte[] Jil_Ser_C100K() => Encoding.UTF8.GetBytes(Jil.JSON.Serialize(_c100k));
}
