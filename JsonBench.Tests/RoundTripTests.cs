using JsonBench.Helpers;
using JsonBench.Models.Isolation;

namespace UnitTests;

/// <summary>
/// Per-dimension serialise→deserialise round-trip integrity tests. Each method mirrors
/// the (model type, source-gen type-info) pairing that the matching isolation benchmark
/// uses, and sweeps that benchmark's levels. The shared work lives in
/// <see cref="RoundTripHelper.AssertRoundTrips{T}"/>.
///
/// Covers every isolation dimension. Depth, StringLength, StringComposition and
/// Redundancy share Node20&lt;string&gt;; Numeric splits int/double; Width changes type
/// per level; Size wraps the model in ItemsWrapper. The Factorial matrix follows the
/// same pattern and lives in <see cref="FactorialRoundTripTests"/>.
/// </summary>
public class RoundTripTests
{
    private static byte[] Load(string subset, string id) =>
        File.ReadAllBytes(SerializationHelper.TestDataFile(subset, $"{id}.json"));

    // ---------- Depth: fixed Node20<string>, D1..D40 ----------
    [Theory]
    [InlineData("D1")]
    [InlineData("D2")]
    [InlineData("D4")]
    [InlineData("D8")]
    [InlineData("D15")]
    [InlineData("D25")]
    [InlineData("D40")]
    public void Depth_RoundTrips(string id) =>
        RoundTripHelper.AssertRoundTrips<Node20<string>>(
            Load("IsoDepth", id), IsolationJsonContext.Default.Node20String);

    // ---------- Numeric integers: Node20<int>, I2..I10 ----------
    [Theory]
    [InlineData("I2")]
    [InlineData("I3")]
    [InlineData("I5")]
    [InlineData("I7")]
    [InlineData("I10")]
    public void NumericInteger_RoundTrips(string id) =>
        RoundTripHelper.AssertRoundTrips<Node20<int>>(
            Load("IsoNumeric", id), IsolationJsonContext.Default.Node20Int32);

    // ---------- Numeric floats: Node20<double>, F2..F10 ----------
    [Theory]
    [InlineData("F2")]
    [InlineData("F3")]
    [InlineData("F5")]
    [InlineData("F7")]
    [InlineData("F10")]
    public void NumericFloat_RoundTrips(string id) =>
        RoundTripHelper.AssertRoundTrips<Node20<double>>(
            Load("IsoNumeric", id), IsolationJsonContext.Default.Node20Double);

    // ---------- Width: model type changes per level, W2..W200 ----------
    [Fact]
    public void Width_RoundTrips()
    {
        RoundTripHelper.AssertRoundTrips<Node2<string>>(Load("IsoWidth", "W2"), IsolationJsonContext.Default.Node2String);
        RoundTripHelper.AssertRoundTrips<Node5<string>>(Load("IsoWidth", "W5"), IsolationJsonContext.Default.Node5String);
        RoundTripHelper.AssertRoundTrips<Node10<string>>(Load("IsoWidth", "W10"), IsolationJsonContext.Default.Node10String);
        RoundTripHelper.AssertRoundTrips<Node20<string>>(Load("IsoWidth", "W20"), IsolationJsonContext.Default.Node20String);
        RoundTripHelper.AssertRoundTrips<Node50<string>>(Load("IsoWidth", "W50"), IsolationJsonContext.Default.Node50String);
        RoundTripHelper.AssertRoundTrips<Node100<string>>(Load("IsoWidth", "W100"), IsolationJsonContext.Default.Node100String);
        RoundTripHelper.AssertRoundTrips<Node200<string>>(Load("IsoWidth", "W200"), IsolationJsonContext.Default.Node200String);
    }

    // ---------- Size: ItemsWrapper<Node20<string>>, C10..C1K ----------
    // C10K (30 MB) and C100K (291 MB) are excluded: the count dimension exercises the
    // same per-object code path at every level, so faithful round-tripping at C1K
    // establishes it for the larger counts without the multi-GB cost of materialising
    // and re-emitting a 100k-object graph nine times.
    [Theory]
    [InlineData("C10")]
    [InlineData("C100")]
    [InlineData("C1K")]
    public void Size_RoundTrips(string id) =>
        RoundTripHelper.AssertRoundTrips<ItemsWrapper<Node20<string>>>(
            Load("IsoSize", id), IsolationJsonContext.Default.ItemsWrapperNode20String);

    // ---------- String length: Node20<string>, L5..L10000 ----------
    [Theory]
    [InlineData("L5")]
    [InlineData("L20")]
    [InlineData("L50")]
    [InlineData("L200")]
    [InlineData("L500")]
    [InlineData("L2000")]
    [InlineData("L10000")]
    public void StringLength_RoundTrips(string id) =>
        RoundTripHelper.AssertRoundTrips<Node20<string>>(
            Load("IsoStringLength", id), IsolationJsonContext.Default.Node20String);

    // ---------- String composition: Node20<string>, ASCII / Unicode / Escape / UnicodeEscape ----------
    [Theory]
    [InlineData("A0")]
    [InlineData("U5")]
    [InlineData("U10")]
    [InlineData("U25")]
    [InlineData("U50")]
    [InlineData("U100")]
    [InlineData("E5")]
    [InlineData("E10")]
    [InlineData("E25")]
    [InlineData("E50")]
    [InlineData("E100")]
    [InlineData("UE5")]
    [InlineData("UE10")]
    [InlineData("UE25")]
    [InlineData("UE50")]
    [InlineData("UE100")]
    public void StringComposition_RoundTrips(string id) =>
        RoundTripHelper.AssertRoundTrips<Node20<string>>(
            Load("IsoStringComp", id), IsolationJsonContext.Default.Node20String);

    // ---------- Redundancy: Node20<string>, R0..R95 ----------
    [Theory]
    [InlineData("R0")]
    [InlineData("R25")]
    [InlineData("R50")]
    [InlineData("R75")]
    [InlineData("R95")]
    public void Redundancy_RoundTrips(string id) =>
        RoundTripHelper.AssertRoundTrips<Node20<string>>(
            Load("IsoRedundancy", id), IsolationJsonContext.Default.Node20String);
}
