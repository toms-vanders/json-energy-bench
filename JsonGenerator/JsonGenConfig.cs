namespace JsonGenerator;

/// <summary>
/// Configuration for synthetic JSON generation.
/// Structure-first: depth and width define the tree shape, file size follows.
/// All mix parameters use ratios that should sum to ~1.0.
/// </summary>
public record JsonGenConfig
{
    /// <summary> Nesting depth. Leaf values are placed at this depth.</summary>
    public int NestingDepth { get; init; } = 4;

    /// <summary>Number of children per container (fields per object, elements per array).</summary>
    public int Width { get; init; } = 10;

    /// <summary>Value type distribution: (textual, numeric, boolean).</summary>
    public ContentMix ContentMix { get; init; } = new();

    /// <summary>String character composition: (ascii, unicode, escape).</summary>
    public StringMix StringMix { get; init; } = new();

    /// <summary>Number representation: (integer, float).</summary>
    public NumericMix NumericMix { get; init; } = new();

    /// <summary>Bool/null distribution: (trueRatio, falseRatio, nullRatio).</summary>
    public BoolMix BoolMix { get; init; } = new();

    /// <summary>Container type distribution for inner levels: (object, array). Root is always an object.</summary>
    public NestingMix NestingMix { get; init; } = new();

    /// <summary>Fraction of leaf values that are duplicates (0.0 to 1.0).</summary>
    public double RedundancyRatio { get; init; } = 0.0;

    /// <summary>Fixed length for generated string values.</summary>
    public int StringLength { get; init; } = 20;

    /// <summary>Number of digits for generated integers (e.g., 6 → 100000-999999).</summary>
    public int IntegerDigits { get; init; } = 6;

    /// <summary>Number of integer digits for the whole part of floats.</summary>
    public int FloatIntegerDigits { get; init; } = 5;

    /// <summary>Number of decimal places for generated floats (e.g., 2 → 12345.67).</summary>
    public int FloatDecimalPlaces { get; init; } = 2;

    /// <summary>Number of top-level items. 1 = single object, >1 = wrapped in {"Items": [...]}.</summary>
    public int Count { get; init; } = 1;

    /// <summary>Random seed for reproducibility.</summary>
    public int Seed { get; init; } = 42;
}

/// <summary>
/// Distribution of value types. Ratios should sum to 1.0.
/// </summary>
public record ContentMix
{
    public double Textual { get; init; } = 0.34;
    public double Numeric { get; init; } = 0.33;
    public double Boolean { get; init; } = 0.33;
}

/// <summary>
/// Distribution of string character composition. Ratios should sum to 1.0.
/// </summary>
public record StringMix
{
    public double Ascii { get; init; } = 1.0;
    public double Unicode { get; init; } = 0.0;
    public double Escape { get; init; } = 0.0;
}

/// <summary>
/// Distribution of number representation. Ratios should sum to 1.0.
/// </summary>
public record NumericMix
{
    public double Integer { get; init; } = 1.0;
    public double Float { get; init; } = 0.0;
}

/// <summary>
/// Distribution of boolean/null values. Ratios should sum to 1.0.
/// </summary>
public record BoolMix
{
    public double True { get; init; } = 0.5;
    public double False { get; init; } = 0.5;
    public double Null { get; init; } = 0.0;
}

/// <summary>
/// Distribution of container types at inner nesting levels. Ratios should sum to 1.0.
/// Root is always an object regardless of this setting.
/// </summary>
public record NestingMix
{
    public double Object { get; init; } = 0.5;
    public double Array { get; init; } = 0.5;
}
