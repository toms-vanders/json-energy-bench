using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using JsonBench.Configs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serialization.Bench;
using Serialization.Bench.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonBench;

[Config(typeof(BenchConfig))]
public class JsonSerializationBench
{
    [ParamsSource(nameof(WorkloadIds))]
    public string WorkloadId { get; set; } = "";

    private byte[] _jsonBytes = null!;
    private string _jsonString = null!;

    // Pre-deserialized objects for serialization benchmarks
    private JsonElement _stjElement;
    private JToken _newtonsoftToken = null!;

    public static IEnumerable<string> WorkloadIds
        => SmokeTestConfigs.GetAll().Select(x => x.Id);

    [GlobalSetup]
    public void Setup()
    {
        var path = SerializationHelper.TestDataFile("Smoke", $"{WorkloadId}.json");

        if (!File.Exists(path))
            throw new FileNotFoundException(
                $"Test data not found: {path}. Run SmokeTestConfigs.GenerateAll() first.");

        _jsonBytes = File.ReadAllBytes(path);
        _jsonString = Encoding.UTF8.GetString(_jsonBytes);

        // Pre-deserialize for serialization benchmarks
        _stjElement = JsonSerializer.Deserialize<JsonElement>(_jsonString);
        _newtonsoftToken = JToken.Parse(_jsonString);
    }

    // --- Deserialization (DOM parsing) ---

    [Benchmark, BenchmarkCategory("Deserialize")]
    public JsonDocument SystemTextJson_Deserialize()
        => JsonDocument.Parse(_jsonBytes);

    [Benchmark, BenchmarkCategory("Deserialize")]
    public JToken Newtonsoft_Deserialize()
        => JToken.Parse(_jsonString);

    [Benchmark, BenchmarkCategory("Deserialize")]
    public object Jil_Deserialize()
        => Jil.JSON.DeserializeDynamic(_jsonString);

    // SpanJson and Utf8Json don't have DOM APIs.
    // Using their tokenizer/reader to parse all tokens as the closest equivalent.

    [Benchmark, BenchmarkCategory("Deserialize")]
    public void SpanJson_Deserialize()
    {
        var reader = new SpanJson.JsonReader<byte>(_jsonBytes);
        while (reader.ReadUtf8NextToken() != SpanJson.JsonToken.None) { }
    }

    [Benchmark, BenchmarkCategory("Deserialize")]
    public void Utf8Json_Deserialize()
    {
        var reader = new Utf8Json.JsonReader(_jsonBytes);
        while (reader.GetCurrentJsonToken() != Utf8Json.JsonToken.None)
        {
            reader.ReadNextBlock();
        }
    }

    // --- Serialization ---

    [Benchmark, BenchmarkCategory("Serialize")]
    public string SystemTextJson_Serialize()
        => JsonSerializer.Serialize(_stjElement);

    [Benchmark, BenchmarkCategory("Serialize")]
    public string Newtonsoft_Serialize()
        => JsonConvert.SerializeObject(_newtonsoftToken);

    [Benchmark, BenchmarkCategory("Serialize")]
    public string Jil_Serialize()
        => Jil.JSON.SerializeDynamic(_newtonsoftToken);
}
