using System.Text;
using BenchmarkDotNet.Attributes;
using JsonBench.Models.Isolation;
using JsonBench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench.Benchmarks.Isolation;

/// <summary>
/// Numeric length isolation benchmark: varies numeric value length (5 levels: 2, 3, 5,
/// 7, 10 digits), byte[] I/O. Two variants compared at matched digit counts: integer
/// (Node20&lt;int&gt;, labels I2..I10) and float (Node20&lt;double&gt;, labels F2..F10).
/// Content is 100% numeric. Baseline: D5, W20, Object-only, R0
/// </summary>
[Config(typeof(BenchConfig))]
public class NumericIsolationByteBench
{
    private byte[] _i2_b = null!; private Node20<int> _i2 = null!;
    private byte[] _i3_b = null!; private Node20<int> _i3 = null!;
    private byte[] _i5_b = null!; private Node20<int> _i5 = null!;
    private byte[] _i7_b = null!; private Node20<int> _i7 = null!;
    private byte[] _i10_b = null!; private Node20<int> _i10 = null!;

    private byte[] _f2_b = null!; private Node20<double> _f2 = null!;
    private byte[] _f3_b = null!; private Node20<double> _f3 = null!;
    private byte[] _f5_b = null!; private Node20<double> _f5 = null!;
    private byte[] _f7_b = null!; private Node20<double> _f7 = null!;
    private byte[] _f10_b = null!; private Node20<double> _f10 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _i2_b = Load("I2"); _i2 = JsonSerializer.Deserialize<Node20<int>>(_i2_b)!;
        _i3_b = Load("I3"); _i3 = JsonSerializer.Deserialize<Node20<int>>(_i3_b)!;
        _i5_b = Load("I5"); _i5 = JsonSerializer.Deserialize<Node20<int>>(_i5_b)!;
        _i7_b = Load("I7"); _i7 = JsonSerializer.Deserialize<Node20<int>>(_i7_b)!;
        _i10_b = Load("I10"); _i10 = JsonSerializer.Deserialize<Node20<int>>(_i10_b)!;

        _f2_b = Load("F2"); _f2 = JsonSerializer.Deserialize<Node20<double>>(_f2_b)!;
        _f3_b = Load("F3"); _f3 = JsonSerializer.Deserialize<Node20<double>>(_f3_b)!;
        _f5_b = Load("F5"); _f5 = JsonSerializer.Deserialize<Node20<double>>(_f5_b)!;
        _f7_b = Load("F7"); _f7 = JsonSerializer.Deserialize<Node20<double>>(_f7_b)!;
        _f10_b = Load("F10"); _f10 = JsonSerializer.Deserialize<Node20<double>>(_f10_b)!;
    }

    private static byte[] Load(string id)
    {
        var path = SerializationHelper.TestDataFile("IsoNumeric", $"{id}.json");
        return File.ReadAllBytes(path);
    }

    // ===================== I2 =====================

    [Benchmark, BenchmarkCategory("Deserialize-I2")]
    public Node20<int> STJRefGen_Deser_I2() => JsonSerializer.Deserialize<Node20<int>>(_i2_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I2")]
    public Node20<int> STJSrcGen_Deser_I2() => JsonSerializer.Deserialize(_i2_b, IsolationJsonContext.Default.Node20Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-I2")]
    public Node20<int> Newtonsoft_Deser_I2() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<int>>(Encoding.UTF8.GetString(_i2_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-I2")]
    public Node20<int> SpanJson_Deser_I2() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<int>>(_i2_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I2")]
    public Node20<int> Utf8Json_Deser_I2() => Utf8Json.JsonSerializer.Deserialize<Node20<int>>(_i2_b)!;

    [Benchmark, BenchmarkCategory("Serialize-I2")]
    public byte[] STJRefGen_Ser_I2() => JsonSerializer.SerializeToUtf8Bytes(_i2);
    [Benchmark, BenchmarkCategory("Serialize-I2")]
    public byte[] STJSrcGen_Ser_I2() => JsonSerializer.SerializeToUtf8Bytes(_i2, IsolationJsonContext.Default.Node20Int32);
    [Benchmark, BenchmarkCategory("Serialize-I2")]
    public byte[] Newtonsoft_Ser_I2() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_i2));
    [Benchmark, BenchmarkCategory("Serialize-I2")]
    public byte[] SpanJson_Ser_I2() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_i2);
    [Benchmark, BenchmarkCategory("Serialize-I2")]
    public byte[] Utf8Json_Ser_I2() => Utf8Json.JsonSerializer.Serialize(_i2);

    // ===================== I3 =====================

    [Benchmark, BenchmarkCategory("Deserialize-I3")]
    public Node20<int> STJRefGen_Deser_I3() => JsonSerializer.Deserialize<Node20<int>>(_i3_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I3")]
    public Node20<int> STJSrcGen_Deser_I3() => JsonSerializer.Deserialize(_i3_b, IsolationJsonContext.Default.Node20Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-I3")]
    public Node20<int> Newtonsoft_Deser_I3() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<int>>(Encoding.UTF8.GetString(_i3_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-I3")]
    public Node20<int> SpanJson_Deser_I3() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<int>>(_i3_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I3")]
    public Node20<int> Utf8Json_Deser_I3() => Utf8Json.JsonSerializer.Deserialize<Node20<int>>(_i3_b)!;

    [Benchmark, BenchmarkCategory("Serialize-I3")]
    public byte[] STJRefGen_Ser_I3() => JsonSerializer.SerializeToUtf8Bytes(_i3);
    [Benchmark, BenchmarkCategory("Serialize-I3")]
    public byte[] STJSrcGen_Ser_I3() => JsonSerializer.SerializeToUtf8Bytes(_i3, IsolationJsonContext.Default.Node20Int32);
    [Benchmark, BenchmarkCategory("Serialize-I3")]
    public byte[] Newtonsoft_Ser_I3() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_i3));
    [Benchmark, BenchmarkCategory("Serialize-I3")]
    public byte[] SpanJson_Ser_I3() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_i3);
    [Benchmark, BenchmarkCategory("Serialize-I3")]
    public byte[] Utf8Json_Ser_I3() => Utf8Json.JsonSerializer.Serialize(_i3);

    // ===================== I5 =====================

    [Benchmark, BenchmarkCategory("Deserialize-I5")]
    public Node20<int> STJRefGen_Deser_I5() => JsonSerializer.Deserialize<Node20<int>>(_i5_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I5")]
    public Node20<int> STJSrcGen_Deser_I5() => JsonSerializer.Deserialize(_i5_b, IsolationJsonContext.Default.Node20Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-I5")]
    public Node20<int> Newtonsoft_Deser_I5() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<int>>(Encoding.UTF8.GetString(_i5_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-I5")]
    public Node20<int> SpanJson_Deser_I5() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<int>>(_i5_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I5")]
    public Node20<int> Utf8Json_Deser_I5() => Utf8Json.JsonSerializer.Deserialize<Node20<int>>(_i5_b)!;

    [Benchmark, BenchmarkCategory("Serialize-I5")]
    public byte[] STJRefGen_Ser_I5() => JsonSerializer.SerializeToUtf8Bytes(_i5);
    [Benchmark, BenchmarkCategory("Serialize-I5")]
    public byte[] STJSrcGen_Ser_I5() => JsonSerializer.SerializeToUtf8Bytes(_i5, IsolationJsonContext.Default.Node20Int32);
    [Benchmark, BenchmarkCategory("Serialize-I5")]
    public byte[] Newtonsoft_Ser_I5() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_i5));
    [Benchmark, BenchmarkCategory("Serialize-I5")]
    public byte[] SpanJson_Ser_I5() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_i5);
    [Benchmark, BenchmarkCategory("Serialize-I5")]
    public byte[] Utf8Json_Ser_I5() => Utf8Json.JsonSerializer.Serialize(_i5);

    // ===================== I7 =====================

    [Benchmark, BenchmarkCategory("Deserialize-I7")]
    public Node20<int> STJRefGen_Deser_I7() => JsonSerializer.Deserialize<Node20<int>>(_i7_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I7")]
    public Node20<int> STJSrcGen_Deser_I7() => JsonSerializer.Deserialize(_i7_b, IsolationJsonContext.Default.Node20Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-I7")]
    public Node20<int> Newtonsoft_Deser_I7() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<int>>(Encoding.UTF8.GetString(_i7_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-I7")]
    public Node20<int> SpanJson_Deser_I7() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<int>>(_i7_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I7")]
    public Node20<int> Utf8Json_Deser_I7() => Utf8Json.JsonSerializer.Deserialize<Node20<int>>(_i7_b)!;

    [Benchmark, BenchmarkCategory("Serialize-I7")]
    public byte[] STJRefGen_Ser_I7() => JsonSerializer.SerializeToUtf8Bytes(_i7);
    [Benchmark, BenchmarkCategory("Serialize-I7")]
    public byte[] STJSrcGen_Ser_I7() => JsonSerializer.SerializeToUtf8Bytes(_i7, IsolationJsonContext.Default.Node20Int32);
    [Benchmark, BenchmarkCategory("Serialize-I7")]
    public byte[] Newtonsoft_Ser_I7() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_i7));
    [Benchmark, BenchmarkCategory("Serialize-I7")]
    public byte[] SpanJson_Ser_I7() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_i7);
    [Benchmark, BenchmarkCategory("Serialize-I7")]
    public byte[] Utf8Json_Ser_I7() => Utf8Json.JsonSerializer.Serialize(_i7);

    // ===================== I10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-I10")]
    public Node20<int> STJRefGen_Deser_I10() => JsonSerializer.Deserialize<Node20<int>>(_i10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I10")]
    public Node20<int> STJSrcGen_Deser_I10() => JsonSerializer.Deserialize(_i10_b, IsolationJsonContext.Default.Node20Int32)!;
    [Benchmark, BenchmarkCategory("Deserialize-I10")]
    public Node20<int> Newtonsoft_Deser_I10() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<int>>(Encoding.UTF8.GetString(_i10_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-I10")]
    public Node20<int> SpanJson_Deser_I10() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<int>>(_i10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-I10")]
    public Node20<int> Utf8Json_Deser_I10() => Utf8Json.JsonSerializer.Deserialize<Node20<int>>(_i10_b)!;

    [Benchmark, BenchmarkCategory("Serialize-I10")]
    public byte[] STJRefGen_Ser_I10() => JsonSerializer.SerializeToUtf8Bytes(_i10);
    [Benchmark, BenchmarkCategory("Serialize-I10")]
    public byte[] STJSrcGen_Ser_I10() => JsonSerializer.SerializeToUtf8Bytes(_i10, IsolationJsonContext.Default.Node20Int32);
    [Benchmark, BenchmarkCategory("Serialize-I10")]
    public byte[] Newtonsoft_Ser_I10() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_i10));
    [Benchmark, BenchmarkCategory("Serialize-I10")]
    public byte[] SpanJson_Ser_I10() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_i10);
    [Benchmark, BenchmarkCategory("Serialize-I10")]
    public byte[] Utf8Json_Ser_I10() => Utf8Json.JsonSerializer.Serialize(_i10);

    // ===================== F2 =====================

    [Benchmark, BenchmarkCategory("Deserialize-F2")]
    public Node20<double> STJRefGen_Deser_F2() => JsonSerializer.Deserialize<Node20<double>>(_f2_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-F2")]
    public Node20<double> STJSrcGen_Deser_F2() => JsonSerializer.Deserialize(_f2_b, IsolationJsonContext.Default.Node20Double)!;
    [Benchmark, BenchmarkCategory("Deserialize-F2")]
    public Node20<double> Newtonsoft_Deser_F2() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(Encoding.UTF8.GetString(_f2_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-F2")]
    public Node20<double> SpanJson_Deser_F2() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<double>>(_f2_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-F2")]
    public Node20<double> Utf8Json_Deser_F2() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(_f2_b)!;

    [Benchmark, BenchmarkCategory("Serialize-F2")]
    public byte[] STJRefGen_Ser_F2() => JsonSerializer.SerializeToUtf8Bytes(_f2);
    [Benchmark, BenchmarkCategory("Serialize-F2")]
    public byte[] STJSrcGen_Ser_F2() => JsonSerializer.SerializeToUtf8Bytes(_f2, IsolationJsonContext.Default.Node20Double);
    [Benchmark, BenchmarkCategory("Serialize-F2")]
    public byte[] Newtonsoft_Ser_F2() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_f2));
    [Benchmark, BenchmarkCategory("Serialize-F2")]
    public byte[] SpanJson_Ser_F2() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_f2);
    [Benchmark, BenchmarkCategory("Serialize-F2")]
    public byte[] Utf8Json_Ser_F2() => Utf8Json.JsonSerializer.Serialize(_f2);

    // ===================== F3 =====================

    [Benchmark, BenchmarkCategory("Deserialize-F3")]
    public Node20<double> STJRefGen_Deser_F3() => JsonSerializer.Deserialize<Node20<double>>(_f3_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-F3")]
    public Node20<double> STJSrcGen_Deser_F3() => JsonSerializer.Deserialize(_f3_b, IsolationJsonContext.Default.Node20Double)!;
    [Benchmark, BenchmarkCategory("Deserialize-F3")]
    public Node20<double> Newtonsoft_Deser_F3() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(Encoding.UTF8.GetString(_f3_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-F3")]
    public Node20<double> SpanJson_Deser_F3() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<double>>(_f3_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-F3")]
    public Node20<double> Utf8Json_Deser_F3() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(_f3_b)!;

    [Benchmark, BenchmarkCategory("Serialize-F3")]
    public byte[] STJRefGen_Ser_F3() => JsonSerializer.SerializeToUtf8Bytes(_f3);
    [Benchmark, BenchmarkCategory("Serialize-F3")]
    public byte[] STJSrcGen_Ser_F3() => JsonSerializer.SerializeToUtf8Bytes(_f3, IsolationJsonContext.Default.Node20Double);
    [Benchmark, BenchmarkCategory("Serialize-F3")]
    public byte[] Newtonsoft_Ser_F3() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_f3));
    [Benchmark, BenchmarkCategory("Serialize-F3")]
    public byte[] SpanJson_Ser_F3() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_f3);
    [Benchmark, BenchmarkCategory("Serialize-F3")]
    public byte[] Utf8Json_Ser_F3() => Utf8Json.JsonSerializer.Serialize(_f3);

    // ===================== F5 =====================

    [Benchmark, BenchmarkCategory("Deserialize-F5")]
    public Node20<double> STJRefGen_Deser_F5() => JsonSerializer.Deserialize<Node20<double>>(_f5_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-F5")]
    public Node20<double> STJSrcGen_Deser_F5() => JsonSerializer.Deserialize(_f5_b, IsolationJsonContext.Default.Node20Double)!;
    [Benchmark, BenchmarkCategory("Deserialize-F5")]
    public Node20<double> Newtonsoft_Deser_F5() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(Encoding.UTF8.GetString(_f5_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-F5")]
    public Node20<double> SpanJson_Deser_F5() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<double>>(_f5_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-F5")]
    public Node20<double> Utf8Json_Deser_F5() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(_f5_b)!;

    [Benchmark, BenchmarkCategory("Serialize-F5")]
    public byte[] STJRefGen_Ser_F5() => JsonSerializer.SerializeToUtf8Bytes(_f5);
    [Benchmark, BenchmarkCategory("Serialize-F5")]
    public byte[] STJSrcGen_Ser_F5() => JsonSerializer.SerializeToUtf8Bytes(_f5, IsolationJsonContext.Default.Node20Double);
    [Benchmark, BenchmarkCategory("Serialize-F5")]
    public byte[] Newtonsoft_Ser_F5() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_f5));
    [Benchmark, BenchmarkCategory("Serialize-F5")]
    public byte[] SpanJson_Ser_F5() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_f5);
    [Benchmark, BenchmarkCategory("Serialize-F5")]
    public byte[] Utf8Json_Ser_F5() => Utf8Json.JsonSerializer.Serialize(_f5);

    // ===================== F7 =====================

    [Benchmark, BenchmarkCategory("Deserialize-F7")]
    public Node20<double> STJRefGen_Deser_F7() => JsonSerializer.Deserialize<Node20<double>>(_f7_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-F7")]
    public Node20<double> STJSrcGen_Deser_F7() => JsonSerializer.Deserialize(_f7_b, IsolationJsonContext.Default.Node20Double)!;
    [Benchmark, BenchmarkCategory("Deserialize-F7")]
    public Node20<double> Newtonsoft_Deser_F7() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(Encoding.UTF8.GetString(_f7_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-F7")]
    public Node20<double> SpanJson_Deser_F7() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<double>>(_f7_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-F7")]
    public Node20<double> Utf8Json_Deser_F7() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(_f7_b)!;

    [Benchmark, BenchmarkCategory("Serialize-F7")]
    public byte[] STJRefGen_Ser_F7() => JsonSerializer.SerializeToUtf8Bytes(_f7);
    [Benchmark, BenchmarkCategory("Serialize-F7")]
    public byte[] STJSrcGen_Ser_F7() => JsonSerializer.SerializeToUtf8Bytes(_f7, IsolationJsonContext.Default.Node20Double);
    [Benchmark, BenchmarkCategory("Serialize-F7")]
    public byte[] Newtonsoft_Ser_F7() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_f7));
    [Benchmark, BenchmarkCategory("Serialize-F7")]
    public byte[] SpanJson_Ser_F7() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_f7);
    [Benchmark, BenchmarkCategory("Serialize-F7")]
    public byte[] Utf8Json_Ser_F7() => Utf8Json.JsonSerializer.Serialize(_f7);

    // ===================== F10 =====================

    [Benchmark, BenchmarkCategory("Deserialize-F10")]
    public Node20<double> STJRefGen_Deser_F10() => JsonSerializer.Deserialize<Node20<double>>(_f10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-F10")]
    public Node20<double> STJSrcGen_Deser_F10() => JsonSerializer.Deserialize(_f10_b, IsolationJsonContext.Default.Node20Double)!;
    [Benchmark, BenchmarkCategory("Deserialize-F10")]
    public Node20<double> Newtonsoft_Deser_F10() => Newtonsoft.Json.JsonConvert.DeserializeObject<Node20<double>>(Encoding.UTF8.GetString(_f10_b))!;
    [Benchmark, BenchmarkCategory("Deserialize-F10")]
    public Node20<double> SpanJson_Deser_F10() => SpanJson.JsonSerializer.Generic.Utf8.Deserialize<Node20<double>>(_f10_b)!;
    [Benchmark, BenchmarkCategory("Deserialize-F10")]
    public Node20<double> Utf8Json_Deser_F10() => Utf8Json.JsonSerializer.Deserialize<Node20<double>>(_f10_b)!;

    [Benchmark, BenchmarkCategory("Serialize-F10")]
    public byte[] STJRefGen_Ser_F10() => JsonSerializer.SerializeToUtf8Bytes(_f10);
    [Benchmark, BenchmarkCategory("Serialize-F10")]
    public byte[] STJSrcGen_Ser_F10() => JsonSerializer.SerializeToUtf8Bytes(_f10, IsolationJsonContext.Default.Node20Double);
    [Benchmark, BenchmarkCategory("Serialize-F10")]
    public byte[] Newtonsoft_Ser_F10() => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_f10));
    [Benchmark, BenchmarkCategory("Serialize-F10")]
    public byte[] SpanJson_Ser_F10() => SpanJson.JsonSerializer.Generic.Utf8.Serialize(_f10);
    [Benchmark, BenchmarkCategory("Serialize-F10")]
    public byte[] Utf8Json_Ser_F10() => Utf8Json.JsonSerializer.Serialize(_f10);
}
