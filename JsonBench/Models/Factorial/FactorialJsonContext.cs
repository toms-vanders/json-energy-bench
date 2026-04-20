using System.Text.Json.Serialization;

namespace JsonBench.Models.Factorial;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(Node5<string>))]
[JsonSerializable(typeof(Node5<int>))]
[JsonSerializable(typeof(Node5<bool>))]
[JsonSerializable(typeof(Node20<string>))]
[JsonSerializable(typeof(Node20<int>))]
[JsonSerializable(typeof(Node20<bool>))]
[JsonSerializable(typeof(Node50<string>))]
[JsonSerializable(typeof(Node50<int>))]
[JsonSerializable(typeof(Node50<bool>))]
[JsonSerializable(typeof(Node100<string>))]
[JsonSerializable(typeof(Node100<int>))]
[JsonSerializable(typeof(Node100<bool>))]
public partial class FactorialJsonContext : JsonSerializerContext { }
