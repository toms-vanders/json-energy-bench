using System.Text.Json.Serialization;

namespace JsonBench.Models.Isolation;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(Node2<string>))]
[JsonSerializable(typeof(Node5<string>))]
[JsonSerializable(typeof(Node10<string>))]
[JsonSerializable(typeof(Node20<string>))]
[JsonSerializable(typeof(Node20<double>))]
[JsonSerializable(typeof(Node50<string>))]
[JsonSerializable(typeof(Node100<string>))]
[JsonSerializable(typeof(Node200<string>))]
[JsonSerializable(typeof(ItemsWrapper<Node20<string>>))]
public partial class IsolationJsonContext : JsonSerializerContext { }
