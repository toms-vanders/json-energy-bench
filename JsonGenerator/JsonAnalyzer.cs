using System.Text.Json;

namespace JsonGenerator;

/// <summary>
/// Analyzes a generated JSON file and reports actual measured dimensions.
/// </summary>
public static class JsonAnalyzer
{
    public static JsonAnalysisResult Analyze(Stream jsonStream)
    {
        var fileSize = jsonStream.Length;

        using var doc = JsonDocument.Parse(jsonStream);
        var root = doc.RootElement;

        var stats = new ValueStats();
        var depthStats = new DepthStats();

        Walk(root, currentDepth: 0, stats, depthStats);

        var totalValues = stats.StringCount + stats.IntegerCount + stats.FloatCount
                          + stats.TrueCount + stats.FalseCount + stats.NullCount;

        var textualWeight = totalValues > 0 ? (double)stats.StringCount / totalValues : 0;
        var numericWeight = totalValues > 0 ? (double)(stats.IntegerCount + stats.FloatCount) / totalValues : 0;
        var booleanWeight = totalValues > 0 ? (double)(stats.TrueCount + stats.FalseCount + stats.NullCount) / totalValues : 0;

        var integerRatio = (stats.IntegerCount + stats.FloatCount) > 0
            ? (double)stats.IntegerCount / (stats.IntegerCount + stats.FloatCount)
            : 0;

        var redundancy = totalValues > 0
            ? 1.0 - (double)stats.UniqueValues.Count / totalValues
            : 0;

        return new JsonAnalysisResult
        {
            FileSize = fileSize,
            TotalLeafValues = totalValues,
            TextualRatio = textualWeight,
            NumericRatio = numericWeight,
            BooleanRatio = booleanWeight,
            IntegerRatio = integerRatio,
            FloatRatio = 1.0 - integerRatio,
            TrueCount = stats.TrueCount,
            FalseCount = stats.FalseCount,
            NullCount = stats.NullCount,
            MaxDepth = depthStats.MaxDepth,
            MinLeafDepth = depthStats.MinLeafDepth,
            MaxLeafDepth = depthStats.MaxLeafDepth,
            ObjectCount = depthStats.ObjectCount,
            ArrayCount = depthStats.ArrayCount,
            RedundancyRatio = redundancy
        };
    }

    public static JsonAnalysisResult Analyze(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        return Analyze(stream);
    }

    private static void Walk(JsonElement element, int currentDepth, ValueStats stats, DepthStats depthStats)
    {
        depthStats.MaxDepth = Math.Max(depthStats.MaxDepth, currentDepth);

        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                depthStats.ObjectCount++;
                foreach (var property in element.EnumerateObject())
                    Walk(property.Value, currentDepth + 1, stats, depthStats);
                break;

            case JsonValueKind.Array:
                depthStats.ArrayCount++;
                foreach (var item in element.EnumerateArray())
                    Walk(item, currentDepth + 1, stats, depthStats);
                break;

            case JsonValueKind.String:
                stats.StringCount++;
                TrackUnique(stats, element.GetString()!);
                TrackLeafDepth(depthStats, currentDepth);
                break;

            case JsonValueKind.Number:
                if (element.TryGetInt64(out var intVal))
                {
                    stats.IntegerCount++;
                    TrackUnique(stats, intVal.ToString());
                }
                else
                {
                    stats.FloatCount++;
                    TrackUnique(stats, element.GetDouble().ToString("R"));
                }
                TrackLeafDepth(depthStats, currentDepth);
                break;

            case JsonValueKind.True:
                stats.TrueCount++;
                TrackUnique(stats, "true");
                TrackLeafDepth(depthStats, currentDepth);
                break;

            case JsonValueKind.False:
                stats.FalseCount++;
                TrackUnique(stats, "false");
                TrackLeafDepth(depthStats, currentDepth);
                break;

            case JsonValueKind.Null:
                stats.NullCount++;
                TrackUnique(stats, "null");
                TrackLeafDepth(depthStats, currentDepth);
                break;
        }
    }

    private static void TrackUnique(ValueStats stats, string representation)
    {
        stats.UniqueValues.Add(representation);
    }

    private static void TrackLeafDepth(DepthStats depthStats, int depth)
    {
        depthStats.MinLeafDepth = Math.Min(depthStats.MinLeafDepth, depth);
        depthStats.MaxLeafDepth = Math.Max(depthStats.MaxLeafDepth, depth);
    }

    private class ValueStats
    {
        public int StringCount;
        public int IntegerCount;
        public int FloatCount;
        public int TrueCount;
        public int FalseCount;
        public int NullCount;
        public HashSet<string> UniqueValues = new();
    }

    private class DepthStats
    {
        public int MaxDepth;
        public int MinLeafDepth = int.MaxValue;
        public int MaxLeafDepth;
        public int ObjectCount;
        public int ArrayCount;
    }
}

public record JsonAnalysisResult
{
    public long FileSize { get; init; }
    public int TotalLeafValues { get; init; }

    // Content mix
    public double TextualRatio { get; init; }
    public double NumericRatio { get; init; }
    public double BooleanRatio { get; init; }

    // Numeric subtypes
    public double IntegerRatio { get; init; }
    public double FloatRatio { get; init; }

    // Bool breakdown
    public int TrueCount { get; init; }
    public int FalseCount { get; init; }
    public int NullCount { get; init; }

    // Structure
    public int MaxDepth { get; init; }
    public int MinLeafDepth { get; init; }
    public int MaxLeafDepth { get; init; }
    public int ObjectCount { get; init; }
    public int ArrayCount { get; init; }

    // Redundancy
    public double RedundancyRatio { get; init; }

    public override string ToString() =>
        $"""
         === JSON Analysis ===
         File size:       {FileSize:N0} bytes ({FileSize / 1024.0:N1} KB)
         Leaf values:     {TotalLeafValues:N0}

         Content mix:     Textual={TextualRatio:P1}  Numeric={NumericRatio:P1}  Boolean={BooleanRatio:P1}
         Numeric split:   Integer={IntegerRatio:P1}  Float={FloatRatio:P1}
         Bool breakdown:  true={TrueCount}  false={FalseCount}  null={NullCount}

         Structure:       MaxDepth={MaxDepth}  LeafDepth=[{MinLeafDepth}..{MaxLeafDepth}]
         Containers:      Objects={ObjectCount}  Arrays={ArrayCount}

         Redundancy:      {RedundancyRatio:P1}
         """;
}
