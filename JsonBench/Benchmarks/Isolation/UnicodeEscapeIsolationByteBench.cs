using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Unicode escape isolation benchmark: varies \uXXXX escape density (5 levels), byte[] I/O.
/// Baseline: D5, W20, Textual, Object-only, ASCII, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class UnicodeEscapeIsolationByteBench
{
    private byte[] _ue0_b = null!; private Node20<string> _ue0 = null!;
    private byte[] _ue10_b = null!; private Node20<string> _ue10 = null!;
    private byte[] _ue30_b = null!; private Node20<string> _ue30 = null!;
    private byte[] _ue50_b = null!; private Node20<string> _ue50 = null!;
    private byte[] _ue70_b = null!; private Node20<string> _ue70 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _ue0_b = Load("UE0"); _ue0 = JsonSerializer.Deserialize<Node20<string>>(_ue0_b)!;
        _ue10_b = Load("UE10"); _ue10 = JsonSerializer.Deserialize<Node20<string>>(_ue10_b)!;
        _ue30_b = Load("UE30"); _ue30 = JsonSerializer.Deserialize<Node20<string>>(_ue30_b)!;
        _ue50_b = Load("UE50"); _ue50 = JsonSerializer.Deserialize<Node20<string>>(_ue50_b)!;
        _ue70_b = Load("UE70"); _ue70 = JsonSerializer.Deserialize<Node20<string>>(_ue70_b)!;
    }

    private static byte[] Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoUnicodeEscape", $"{id}.json");
        return File.ReadAllBytes(path);
    }

    // ===================== UE0 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE0")]
    public Node20<string> STJRefGen_Deser_UE0() => JsonSerializer.Deserialize<Node20<string>>(_ue0_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE0")]
    public Node20<string> STJSrcGen_Deser_UE0() => JsonSerializer.Deserialize(_ue0_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE0")]
    public Node20<string> Newtonsoft_Deser_UE0() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_ue0_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE0")]
    public Node20<string> SpanJson_Deser_UE0() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_ue0_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE0")]
    public Node20<string> Utf8Json_Deser_UE0() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_ue0_b)!;

    [Benchmark, BenchmarkCategory("Serialize-UE0")]
    public byte[] STJRefGen_Ser_UE0() => JsonSerializer.SerializeToUtf8Bytes(_ue0);
    [Benchmark, BenchmarkCategory("Serialize-UE0")]
    public byte[] STJSrcGen_Ser_UE0() => JsonSerializer.SerializeToUtf8Bytes(_ue0, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-UE0")]
    public byte[] Newtonsoft_Ser_UE0() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_ue0));
    [Benchmark, BenchmarkCategory("Serialize-UE0")]
    public byte[] SpanJson_Ser_UE0() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_ue0);
    [Benchmark, BenchmarkCategory("Serialize-UE0")]
    public byte[] Utf8Json_Ser_UE0() => Utf8Json.JsonSerializer.Serialize(_ue0);

    // ===================== UE10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> STJRefGen_Deser_UE10() => JsonSerializer.Deserialize<Node20<string>>(_ue10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> STJSrcGen_Deser_UE10() => JsonSerializer.Deserialize(_ue10_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> Newtonsoft_Deser_UE10() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_ue10_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> SpanJson_Deser_UE10() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_ue10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE10")]
    public Node20<string> Utf8Json_Deser_UE10() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_ue10_b)!;

    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public byte[] STJRefGen_Ser_UE10() => JsonSerializer.SerializeToUtf8Bytes(_ue10);
    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public byte[] STJSrcGen_Ser_UE10() => JsonSerializer.SerializeToUtf8Bytes(_ue10, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public byte[] Newtonsoft_Ser_UE10() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_ue10));
    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public byte[] SpanJson_Ser_UE10() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_ue10);
    [Benchmark, BenchmarkCategory("Serialize-UE10")]
    public byte[] Utf8Json_Ser_UE10() => Utf8Json.JsonSerializer.Serialize(_ue10);

    // ===================== UE30 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE30")]
    public Node20<string> STJRefGen_Deser_UE30() => JsonSerializer.Deserialize<Node20<string>>(_ue30_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE30")]
    public Node20<string> STJSrcGen_Deser_UE30() => JsonSerializer.Deserialize(_ue30_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE30")]
    public Node20<string> Newtonsoft_Deser_UE30() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_ue30_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE30")]
    public Node20<string> SpanJson_Deser_UE30() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_ue30_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE30")]
    public Node20<string> Utf8Json_Deser_UE30() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_ue30_b)!;

    [Benchmark, BenchmarkCategory("Serialize-UE30")]
    public byte[] STJRefGen_Ser_UE30() => JsonSerializer.SerializeToUtf8Bytes(_ue30);
    [Benchmark, BenchmarkCategory("Serialize-UE30")]
    public byte[] STJSrcGen_Ser_UE30() => JsonSerializer.SerializeToUtf8Bytes(_ue30, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-UE30")]
    public byte[] Newtonsoft_Ser_UE30() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_ue30));
    [Benchmark, BenchmarkCategory("Serialize-UE30")]
    public byte[] SpanJson_Ser_UE30() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_ue30);
    [Benchmark, BenchmarkCategory("Serialize-UE30")]
    public byte[] Utf8Json_Ser_UE30() => Utf8Json.JsonSerializer.Serialize(_ue30);

    // ===================== UE50 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> STJRefGen_Deser_UE50() => JsonSerializer.Deserialize<Node20<string>>(_ue50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> STJSrcGen_Deser_UE50() => JsonSerializer.Deserialize(_ue50_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> Newtonsoft_Deser_UE50() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_ue50_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> SpanJson_Deser_UE50() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_ue50_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE50")]
    public Node20<string> Utf8Json_Deser_UE50() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_ue50_b)!;

    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public byte[] STJRefGen_Ser_UE50() => JsonSerializer.SerializeToUtf8Bytes(_ue50);
    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public byte[] STJSrcGen_Ser_UE50() => JsonSerializer.SerializeToUtf8Bytes(_ue50, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public byte[] Newtonsoft_Ser_UE50() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_ue50));
    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public byte[] SpanJson_Ser_UE50() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_ue50);
    [Benchmark, BenchmarkCategory("Serialize-UE50")]
    public byte[] Utf8Json_Ser_UE50() => Utf8Json.JsonSerializer.Serialize(_ue50);

    // ===================== UE70 =====================

    [Benchmark, BenchmarkCategory("Deserialize-UE70")]
    public Node20<string> STJRefGen_Deser_UE70() => JsonSerializer.Deserialize<Node20<string>>(_ue70_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE70")]
    public Node20<string> STJSrcGen_Deser_UE70() => JsonSerializer.Deserialize(_ue70_b, IsolationJsonContext.Default.Node20String)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE70")]
    public Node20<string> Newtonsoft_Deser_UE70() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<string>>(Encoding.UTF8.GetString(_ue70_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-UE70")]
    public Node20<string> SpanJson_Deser_UE70() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<string>>(_ue70_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-UE70")]
    public Node20<string> Utf8Json_Deser_UE70() => Utf8Json.JsonSerializer.Deserialize<Node20<string>>(_ue70_b)!;

    [Benchmark, BenchmarkCategory("Serialize-UE70")]
    public byte[] STJRefGen_Ser_UE70() => JsonSerializer.SerializeToUtf8Bytes(_ue70);
    [Benchmark, BenchmarkCategory("Serialize-UE70")]
    public byte[] STJSrcGen_Ser_UE70() => JsonSerializer.SerializeToUtf8Bytes(_ue70, IsolationJsonContext.Default.Node20String);
    [Benchmark, BenchmarkCategory("Serialize-UE70")]
    public byte[] Newtonsoft_Ser_UE70() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_ue70));
    [Benchmark, BenchmarkCategory("Serialize-UE70")]
    public byte[] SpanJson_Ser_UE70() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_ue70);
    [Benchmark, BenchmarkCategory("Serialize-UE70")]
    public byte[] Utf8Json_Ser_UE70() => Utf8Json.JsonSerializer.Serialize(_ue70);
}
