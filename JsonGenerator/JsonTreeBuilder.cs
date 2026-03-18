using System.Text.Json;

namespace JsonGenerator;

/// <summary>
/// Builds a JSON chain structure and writes it directly to a stream via Utf8JsonWriter.
/// Chain approach: at each non-leaf level, (width - 1) entries are leaf values and
/// 1 entry is a nested container. At the leaf level, all entries are values.
/// Total values ≈ (width - 1) × (depth - 1) + width.
/// Root is always an object.
/// </summary>
public class JsonTreeBuilder
{
    private readonly JsonGenConfig _config;
    private readonly Random _random;
    private readonly ValueGenerator _valueGenerator;
    private readonly double _nestingObjectThreshold;
    private int _leafCount;
    private int _totalKeyCount;

    public JsonTreeBuilder(JsonGenConfig config)
    {
        _config = config;
        _random = new Random(config.Seed);
        _valueGenerator = new ValueGenerator(config, _random);
        _nestingObjectThreshold = config.NestingMix.Object;
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

    private void WriteContainer(Utf8JsonWriter writer, int currentDepth)
    {
        var isObject = _random.NextDouble() < _nestingObjectThreshold;

        if (isObject)
            WriteObject(writer, currentDepth);
        else
            WriteArray(writer, currentDepth);
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

        // Last field: nested container or null at leaf
        writer.WritePropertyName($"key_{keyIndex}");
        _totalKeyCount++;

        if (isLeafLevel)
            writer.WriteNullValue();
        else
            WriteContainer(writer, currentDepth + 1);

        writer.WriteEndObject();
    }

    private void WriteArray(Utf8JsonWriter writer, int currentDepth)
    {
        writer.WriteStartArray();

        var isLeafLevel = currentDepth + 1 >= _config.NestingDepth;

        if (isLeafLevel)
        {
            for (var i = 0; i < _config.Width; i++)
            {
                _valueGenerator.WriteValue(writer);
                _leafCount++;
            }
        }
        else
        {
            for (var i = 0; i < _config.Width - 1; i++)
            {
                _valueGenerator.WriteValue(writer);
                _leafCount++;
            }

            WriteContainer(writer, currentDepth + 1);
        }

        writer.WriteEndArray();
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
