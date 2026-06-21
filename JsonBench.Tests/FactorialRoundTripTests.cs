using JsonBench.Helpers;
using JsonBench.Models.Factorial;

namespace UnitTests;

/// <summary>
/// Round-trip integrity for the factorial matrix (Depth × Width × Content). The model
/// type is fixed by width (Node5/20/50/100) and content (T→string, N→int, B→bool);
/// depth only varies the file, so each (width, content) case sweeps all four depths.
/// Both the default and value-normalised matrices are covered.
///
/// This lives in its own file so the unqualified Node{N} names bind to
/// JsonBench.Models.Factorial — a different set of models from the isolation suite's
/// JsonBench.Models.Isolation in <see cref="RoundTripTests"/>.
/// </summary>
public class FactorialRoundTripTests
{
    private static readonly string[] Depths = { "D2", "D5", "D10", "D20" };

    private static byte[] Load(string subset, string id) =>
        File.ReadAllBytes(SerializationHelper.TestDataFile(subset, $"{id}.json"));

    public static IEnumerable<object[]> Cases()
    {
        (string W, string C)[] shapes =
        {
            ("W5", "T"), ("W5", "N"), ("W5", "B"),
            ("W20", "T"), ("W20", "N"), ("W20", "B"),
            ("W50", "T"), ("W50", "N"), ("W50", "B"),
            ("W100", "T"), ("W100", "N"), ("W100", "B"),
        };
        foreach (var subset in new[] { "Factorial", "FactorialNormalized" })
            foreach (var (w, c) in shapes)
                yield return new object[] { subset, w, c };
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void Factorial_RoundTrips(string subset, string w, string c)
    {
        foreach (var d in Depths)
        {
            var bytes = Load(subset, $"{d}-{w}-{c}");
            switch (w, c)
            {
                case ("W5", "T"): RoundTripHelper.AssertRoundTrips<Node5<string>>(bytes, FactorialJsonContext.Default.Node5String); break;
                case ("W5", "N"): RoundTripHelper.AssertRoundTrips<Node5<int>>(bytes, FactorialJsonContext.Default.Node5Int32); break;
                case ("W5", "B"): RoundTripHelper.AssertRoundTrips<Node5<bool>>(bytes, FactorialJsonContext.Default.Node5Boolean); break;
                case ("W20", "T"): RoundTripHelper.AssertRoundTrips<Node20<string>>(bytes, FactorialJsonContext.Default.Node20String); break;
                case ("W20", "N"): RoundTripHelper.AssertRoundTrips<Node20<int>>(bytes, FactorialJsonContext.Default.Node20Int32); break;
                case ("W20", "B"): RoundTripHelper.AssertRoundTrips<Node20<bool>>(bytes, FactorialJsonContext.Default.Node20Boolean); break;
                case ("W50", "T"): RoundTripHelper.AssertRoundTrips<Node50<string>>(bytes, FactorialJsonContext.Default.Node50String); break;
                case ("W50", "N"): RoundTripHelper.AssertRoundTrips<Node50<int>>(bytes, FactorialJsonContext.Default.Node50Int32); break;
                case ("W50", "B"): RoundTripHelper.AssertRoundTrips<Node50<bool>>(bytes, FactorialJsonContext.Default.Node50Boolean); break;
                case ("W100", "T"): RoundTripHelper.AssertRoundTrips<Node100<string>>(bytes, FactorialJsonContext.Default.Node100String); break;
                case ("W100", "N"): RoundTripHelper.AssertRoundTrips<Node100<int>>(bytes, FactorialJsonContext.Default.Node100Int32); break;
                case ("W100", "B"): RoundTripHelper.AssertRoundTrips<Node100<bool>>(bytes, FactorialJsonContext.Default.Node100Boolean); break;
                default: throw new Xunit.Sdk.XunitException($"unmapped factorial shape {w}-{c}");
            }
        }
    }
}
