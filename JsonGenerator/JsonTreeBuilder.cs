using System.Text.Json;

namespace JsonGenerator;

/// <summary>
/// Builds a JSON chain structure and writes it directly to a stream via Utf8JsonWriter.
/// Object-only nesting: at every level the object has Width keys; the first (Width − 1)
/// hold generated leaf values and the last key holds either a nested object (non-leaf
/// level) or a JSON null sentinel that terminates the chain (leaf level).
/// The terminating null is counted in KeyCount but not in LeafCount, so:
///     LeafCount = (Width − 1) × NestingDepth
///     KeyCount  = Width × NestingDepth
/// Per top-level item. When Count > 1 these counts are multiplied by Count and the
/// document is wrapped in {"Items": [...]}.
/// </summary>
public class JsonTreeBuilder
{
    private readonly JsonGenConfig _config;
    private readonly Random _random;
    private readonly ValueGenerator _valueGenerator;
    private int _leafCount;
    private int _totalKeyCount;

    public JsonTreeBuilder(JsonGenConfig config)
    {
        _config = config;
        _random = new Random(config.Seed);
        _valueGenerator = new ValueGenerator(config, _random);
    }

    /// <summary>
    /// Generates a complete JSON document and writes it to the given stream.
    /// Returns metadata about the generated file.
    /// </summary>
    public GenerationResult Generate(Stream output)
    {
        _leafCount = 0;
        _totalKeyCount = 0;

        var countingStream = new CountingStream(output);

        using var writer = new Utf8JsonWriter(countingStream, new JsonWriterOptions
        {
            Indented = false,
            SkipValidation = true
        });

        if (_config.Count == 1)
        {
            // Single object
            WriteObject(writer, currentDepth: 0);
        }
        else
        {
            // Wrapped in {"Items": [...]}
            writer.WriteStartObject();
            writer.WritePropertyName("Items");
            writer.WriteStartArray();

            for (var i = 0; i < _config.Count; i++)
                WriteObject(writer, currentDepth: 0);

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        writer.Flush();

        return new GenerationResult
        {
            FileSize = countingStream.BytesWritten,
            LeafCount = _leafCount,
            KeyCount = _totalKeyCount,
        };
    }

    private void WriteObject(Utf8JsonWriter writer, int currentDepth)
    {
        writer.WriteStartObject();

        var isLeafLevel = currentDepth + 1 >= _config.NestingDepth;
        var keyIndex = 0;

        // (width - 1) value fields
        for (var i = 0; i < _config.Width - 1; i++)
        {
            writer.WritePropertyName($"key_{keyIndex++}");
            _valueGenerator.WriteValue(writer);
            _leafCount++;
            _totalKeyCount++;
        }

        // Last field: nested object or null sentinel at leaf level
        writer.WritePropertyName($"key_{keyIndex}");
        _totalKeyCount++;

        if (isLeafLevel)
            writer.WriteNullValue();
        else
            WriteObject(writer, currentDepth + 1);

        writer.WriteEndObject();
    }
}

/// <summary>
/// Metadata about a generated JSON file.
/// </summary>
public record GenerationResult
{
    public long FileSize { get; init; }
    public int LeafCount { get; init; }
    public int KeyCount { get; init; }

    public override string ToString() =>
        $"FileSize={FileSize:N0} bytes ({FileSize / 1024.0:N1} KB), Leaves={LeafCount:N0}, Keys={KeyCount:N0}";
}
