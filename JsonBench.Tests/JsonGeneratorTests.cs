using JsonGenerator;

namespace UnitTests;

public class JsonGeneratorTests
{
    [Fact]
    public void DefaultConfig_GeneratesValidJson()
    {
        var config = new JsonGenConfig();
        using var ms = new MemoryStream();
        var result = new JsonTreeBuilder(config).Generate(ms);

        Assert.True(result.FileSize > 0);
        Assert.True(result.LeafCount > 0);
        Assert.True(result.KeyCount > 0);
    }

    [Fact]
    public void LeafCount_MatchesChainFormula()
    {
        // Chain approach: each non-leaf level has (width-1) values + 1 child container
        // Leaf level has width values
        // Total = (width-1) × (depth-1) + width = width × depth - depth + 1
        var config = new JsonGenConfig
        {
            Width = 5,
            NestingDepth = 3,
            NestingMix = new NestingMix { Object = 1.0, Array = 0.0 },
        };
        using var ms = new MemoryStream();
        var result = new JsonTreeBuilder(config).Generate(ms);

        // depth=3, width=5: (5-1)×(3-1) + 5 = 8 + 5 = 13
        Assert.Equal(13, result.LeafCount);
    }

    [Fact]
    public void RootIsAlwaysObject()
    {
        // Even with array-only nesting mix, root should be an object
        var config = new JsonGenConfig
        {
            Width = 5,
            NestingDepth = 2,
            NestingMix = new NestingMix { Object = 0.0, Array = 1.0 },
        };
        using var ms = new MemoryStream();
        new JsonTreeBuilder(config).Generate(ms);

        ms.Position = 0;
        var result = JsonAnalyzer.Analyze(ms);

        // Root is object (1), inner containers are arrays
        Assert.True(result.ObjectCount >= 1);
    }

    [Fact]
    public void TextualOnly_ProducesOnlyStrings()
    {
        var config = new JsonGenConfig
        {
            ContentMix = new ContentMix { Textual = 1.0, Numeric = 0.0, Boolean = 0.0 },
            NestingDepth = 2,
            Width = 10,
        };
        using var ms = new MemoryStream();
        new JsonTreeBuilder(config).Generate(ms);

        ms.Position = 0;
        var result = JsonAnalyzer.Analyze(ms);

        Assert.Equal(1.0, result.TextualRatio);
        Assert.Equal(0.0, result.NumericRatio);
        Assert.Equal(0.0, result.BooleanRatio);
    }

    [Fact]
    public void NumericOnly_ProducesOnlyNumbers()
    {
        var config = new JsonGenConfig
        {
            ContentMix = new ContentMix { Textual = 0.0, Numeric = 1.0, Boolean = 0.0 },
            NestingDepth = 2,
            Width = 10,
        };
        using var ms = new MemoryStream();
        new JsonTreeBuilder(config).Generate(ms);

        ms.Position = 0;
        var result = JsonAnalyzer.Analyze(ms);

        Assert.Equal(0.0, result.TextualRatio);
        Assert.Equal(1.0, result.NumericRatio);
        Assert.Equal(0.0, result.BooleanRatio);
    }

    [Fact]
    public void BooleanOnly_ProducesOnlyBoolsAndNulls()
    {
        var config = new JsonGenConfig
        {
            ContentMix = new ContentMix { Textual = 0.0, Numeric = 0.0, Boolean = 1.0 },
            NestingDepth = 2,
            Width = 10,
        };
        using var ms = new MemoryStream();
        new JsonTreeBuilder(config).Generate(ms);

        ms.Position = 0;
        var result = JsonAnalyzer.Analyze(ms);

        Assert.Equal(0.0, result.TextualRatio);
        Assert.Equal(0.0, result.NumericRatio);
        Assert.Equal(1.0, result.BooleanRatio);
    }

    [Fact]
    public void IntegerOnly_ProducesNoFloats()
    {
        var config = new JsonGenConfig
        {
            ContentMix = new ContentMix { Textual = 0.0, Numeric = 1.0, Boolean = 0.0 },
            NumericMix = new NumericMix { Integer = 1.0, Float = 0.0 },
            NestingDepth = 2,
            Width = 10,
        };
        using var ms = new MemoryStream();
        new JsonTreeBuilder(config).Generate(ms);

        ms.Position = 0;
        var result = JsonAnalyzer.Analyze(ms);

        Assert.Equal(1.0, result.IntegerRatio);
    }

    [Fact]
    public void ObjectOnly_NestingMix_ProducesNoArrays()
    {
        var config = new JsonGenConfig
        {
            NestingDepth = 3,
            Width = 5,
            NestingMix = new NestingMix { Object = 1.0, Array = 0.0 },
        };
        using var ms = new MemoryStream();
        new JsonTreeBuilder(config).Generate(ms);

        ms.Position = 0;
        var result = JsonAnalyzer.Analyze(ms);

        Assert.True(result.ObjectCount > 0);
        Assert.Equal(0, result.ArrayCount);
    }

    [Fact]
    public void ArrayOnly_NestingMix_InnerContainersAreArrays()
    {
        var config = new JsonGenConfig
        {
            NestingDepth = 3,
            Width = 5,
            NestingMix = new NestingMix { Object = 0.0, Array = 1.0 },
        };
        using var ms = new MemoryStream();
        new JsonTreeBuilder(config).Generate(ms);

        ms.Position = 0;
        var result = JsonAnalyzer.Analyze(ms);

        // Root is always an object, but inner containers should be arrays
        Assert.Equal(1, result.ObjectCount);
        Assert.True(result.ArrayCount > 0);
    }

    [Fact]
    public void FlatStructure_LeafDepthIsShallow()
    {
        var config = new JsonGenConfig
        {
            NestingDepth = 2,
            Width = 10,
        };
        using var ms = new MemoryStream();
        new JsonTreeBuilder(config).Generate(ms);

        ms.Position = 0;
        var result = JsonAnalyzer.Analyze(ms);

        Assert.True(result.MaxLeafDepth <= 2);
    }

    [Fact]
    public void Redundancy_ProducesDuplicateValues()
    {
        var config = new JsonGenConfig
        {
            ContentMix = new ContentMix { Textual = 1.0, Numeric = 0.0, Boolean = 0.0 },
            NestingDepth = 2,
            Width = 50,
            RedundancyRatio = 0.8,
        };
        using var ms = new MemoryStream();
        new JsonTreeBuilder(config).Generate(ms);

        ms.Position = 0;
        var result = JsonAnalyzer.Analyze(ms);

        Assert.True(result.RedundancyRatio > 0.3, $"Expected significant redundancy, got {result.RedundancyRatio:P1}");
    }

    [Fact]
    public void NoRedundancy_ProducesMostlyUniqueValues()
    {
        var config = new JsonGenConfig
        {
            ContentMix = new ContentMix { Textual = 1.0, Numeric = 0.0, Boolean = 0.0 },
            NestingDepth = 2,
            Width = 50,
            RedundancyRatio = 0.0,
        };
        using var ms = new MemoryStream();
        new JsonTreeBuilder(config).Generate(ms);

        ms.Position = 0;
        var result = JsonAnalyzer.Analyze(ms);

        Assert.True(result.RedundancyRatio < 0.1, $"Expected low redundancy, got {result.RedundancyRatio:P1}");
    }

    [Fact]
    public void SameSeed_ProducesIdenticalOutput()
    {
        var config = new JsonGenConfig { Width = 10, NestingDepth = 3, Seed = 123 };

        using var ms1 = new MemoryStream();
        new JsonTreeBuilder(config).Generate(ms1);

        using var ms2 = new MemoryStream();
        new JsonTreeBuilder(config).Generate(ms2);

        Assert.Equal(ms1.ToArray(), ms2.ToArray());
    }

    [Fact]
    public void FixedStringLength_AllStringsHaveConfiguredLength()
    {
        var config = new JsonGenConfig
        {
            ContentMix = new ContentMix { Textual = 1.0, Numeric = 0.0, Boolean = 0.0 },
            StringMix = new StringMix { Ascii = 1.0, Unicode = 0.0, Escape = 0.0 },
            StringLength = 15,
            NestingDepth = 1,
            Width = 20,
        };
        using var ms = new MemoryStream();
        new JsonTreeBuilder(config).Generate(ms);

        ms.Position = 0;
        using var doc = System.Text.Json.JsonDocument.Parse(ms);
        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            var value = prop.Value.GetString()!;
            Assert.Equal(15, value.Length);
        }
    }

    [Fact]
    public void GenerationResult_ReportsCorrectMetadata()
    {
        var config = new JsonGenConfig
        {
            Width = 4,
            NestingDepth = 3,
            NestingMix = new NestingMix { Object = 1.0, Array = 0.0 },
        };
        using var ms = new MemoryStream();
        var result = new JsonTreeBuilder(config).Generate(ms);

        // Chain: depth=3, width=4, all objects:
        // Level 0 (root): 3 values + 1 child = 3 leaves
        // Level 1: 3 values + 1 child = 3 leaves
        // Level 2 (leaf): 4 values = 4 leaves
        // Total: 3 + 3 + 4 = 10 leaves
        // Keys: 4 (root) + 4 (level 1) + 4 (level 2) = 12
        Assert.Equal(10, result.LeafCount);
        Assert.Equal(12, result.KeyCount);
        Assert.Equal(ms.Length, result.FileSize);
    }
}
