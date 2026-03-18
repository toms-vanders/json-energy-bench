using System.Text;
using System.Text.Json;

namespace JsonGenerator;

/// <summary>
/// Generates leaf values (strings, numbers, bools, nulls) based on config ratios.
/// Uses fixed value lengths for reproducibility and controlled benchmarking.
/// Handles redundancy by maintaining a pool of previously generated values.
/// </summary>
public class ValueGenerator
{
    private readonly JsonGenConfig _config;
    private readonly Random _random;
    private readonly List<Action<Utf8JsonWriter>> _valuePool = new();

    // Precomputed cumulative thresholds for weighted random selection
    private readonly double _contentTextualThreshold;
    private readonly double _contentNumericThreshold;

    private readonly double _stringAsciiThreshold;
    private readonly double _stringUnicodeThreshold;

    private readonly double _numericIntegerThreshold;

    private readonly double _boolTrueThreshold;
    private readonly double _boolFalseThreshold;

    // Precomputed integer/float ranges from config
    private readonly int _integerMin;
    private readonly int _integerMax;
    private readonly int _floatIntMin;
    private readonly int _floatIntMax;

    // Unicode ranges for generating non-ASCII characters
    private static readonly (int Start, int End)[] UnicodeRanges =
    [
        (0x00C0, 0x00FF), // Latin Extended
        (0x0400, 0x04FF), // Cyrillic
        (0x4E00, 0x4FFF), // CJK subset
        (0x0600, 0x06FF), // Arabic
    ];

    private static readonly string[] EscapeSequences =
        ["\n", "\t", "\\", "\"", "\r", "\b", "\f"];

    public ValueGenerator(JsonGenConfig config, Random random)
    {
        _config = config;
        _random = random;

        // Precompute cumulative thresholds
        _contentTextualThreshold = config.ContentMix.Textual;
        _contentNumericThreshold = _contentTextualThreshold + config.ContentMix.Numeric;

        _stringAsciiThreshold = config.StringMix.Ascii;
        _stringUnicodeThreshold = _stringAsciiThreshold + config.StringMix.Unicode;

        _numericIntegerThreshold = config.NumericMix.Integer;

        _boolTrueThreshold = config.BoolMix.True;
        _boolFalseThreshold = _boolTrueThreshold + config.BoolMix.False;

        // Precompute integer range from digit count (e.g., 6 digits → 100000-999999)
        _integerMin = (int)Math.Pow(10, config.IntegerDigits - 1);
        _integerMax = (int)Math.Pow(10, config.IntegerDigits) - 1;

        // Precompute float integer part range
        _floatIntMin = (int)Math.Pow(10, config.FloatIntegerDigits - 1);
        _floatIntMax = (int)Math.Pow(10, config.FloatIntegerDigits) - 1;
    }

    /// <summary>
    /// Writes a random leaf value to the writer based on configured ratios.
    /// </summary>
    public void WriteValue(Utf8JsonWriter writer)
    {
        // Redundancy check: reuse a previous value with configured probability
        if (_valuePool.Count > 0 && _random.NextDouble() < _config.RedundancyRatio)
        {
            var cached = _valuePool[_random.Next(_valuePool.Count)];
            cached(writer);
            return;
        }

        // Generate a fresh value and optionally store it for redundancy
        Action<Utf8JsonWriter> writeAction = PickContentType() switch
        {
            LeafType.String => CreateStringWriter(),
            LeafType.Number => CreateNumberWriter(),
            LeafType.Boolean => CreateBoolWriter(),
            _ => throw new InvalidOperationException()
        };

        writeAction(writer);
        _valuePool.Add(writeAction);
    }

    private LeafType PickContentType()
    {
        var roll = _random.NextDouble();
        if (roll < _contentTextualThreshold) return LeafType.String;
        if (roll < _contentNumericThreshold) return LeafType.Number;
        return LeafType.Boolean;
    }

    private Action<Utf8JsonWriter> CreateStringWriter()
    {
        var value = GenerateString();
        return w => w.WriteStringValue(value);
    }

    private Action<Utf8JsonWriter> CreateNumberWriter()
    {
        if (_random.NextDouble() < _numericIntegerThreshold)
        {
            var value = _random.Next(_integerMin, _integerMax + 1);
            return w => w.WriteNumberValue(value);
        }
        else
        {
            var intPart = _random.Next(_floatIntMin, _floatIntMax + 1);
            var decimalMax = (int)Math.Pow(10, _config.FloatDecimalPlaces);
            var decPart = _random.Next(0, decimalMax);
            var value = intPart + decPart / (double)decimalMax;
            return w => w.WriteNumberValue(Math.Round(value, _config.FloatDecimalPlaces));
        }
    }

    private Action<Utf8JsonWriter> CreateBoolWriter()
    {
        var roll = _random.NextDouble();
        if (roll < _boolTrueThreshold)
            return w => w.WriteBooleanValue(true);
        if (roll < _boolFalseThreshold)
            return w => w.WriteBooleanValue(false);
        return w => w.WriteNullValue();
    }

    private string GenerateString()
    {
        var length = _config.StringLength;
        var roll = _random.NextDouble();

        if (roll < _stringAsciiThreshold)
            return GenerateAsciiString(length);
        if (roll < _stringUnicodeThreshold)
            return GenerateUnicodeString(length);
        return GenerateEscapeHeavyString(length);
    }

    private string GenerateAsciiString(int length)
    {
        return string.Create(length, _random, (span, rng) =>
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
            for (var i = 0; i < span.Length; i++)
                span[i] = chars[rng.Next(chars.Length)];
        });
    }

    private string GenerateUnicodeString(int length)
    {
        var sb = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            var range = UnicodeRanges[_random.Next(UnicodeRanges.Length)];
            sb.Append((char)_random.Next(range.Start, range.End + 1));
        }
        return sb.ToString();
    }

    private string GenerateEscapeHeavyString(int length)
    {
        var sb = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            // ~30% chance of inserting an escape sequence
            if (_random.NextDouble() < 0.3)
            {
                sb.Append(EscapeSequences[_random.Next(EscapeSequences.Length)]);
            }
            else
            {
                sb.Append((char)_random.Next('a', 'z' + 1));
            }
        }
        return sb.ToString();
    }

    private enum LeafType
    {
        String,
        Number,
        Boolean
    }
}
