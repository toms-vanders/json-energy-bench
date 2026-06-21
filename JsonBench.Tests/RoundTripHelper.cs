using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

namespace UnitTests;

/// <summary>
/// Round-trip integrity check shared by every workload-dimension test.
///
/// For one generated file and one model type <typeparamref name="T"/>, it verifies
/// that each library the benchmarks use preserves the document — that neither the
/// read path nor the write path silently drops part of the input. This is the
/// correctness guard behind the energy numbers: a library must not look low-energy
/// merely because it skipped work by discarding data.
///
/// Every comparison is made on the decoded JSON <em>tree</em> via
/// <see cref="JsonNode.DeepEquals"/>. Each library's output is funnelled back through
/// System.Text.Json before comparison, so per-library formatting choices — key order,
/// Unicode escaping, explicit-null vs. absent members, number text — are normalised
/// away. Only genuinely lost or altered data fails the assertion.
/// </summary>
public static class RoundTripHelper
{
    public static void AssertRoundTrips<T>(byte[] original, JsonTypeInfo<T> srcGen)
    {
        var expected = JsonNode.Parse(original);

        // System.Text.Json (reflection) is the reference reader/writer. If its own
        // model cannot reproduce the file, the model SHAPE does not match the data
        // (e.g. a Node{N} narrower than the file's objects) — and because every
        // library shares this model, that loss is invisible to library-vs-library
        // checks. Comparing the reference model straight back to the file catches it.
        var model = JsonSerializer.Deserialize<T>(original)!;
        AssertSameTree("STJRefGen model vs. file", expected, ReEmit(model));

        // READ path: deserialize with each library, re-emit through STJ, compare to
        // the file. Catches a library dropping data while reading.
        AssertSameTree("STJSrcGen deserialize", expected,
            ReEmit(JsonSerializer.Deserialize(original, srcGen)!));
        AssertSameTree("Newtonsoft deserialize", expected,
            ReEmit(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(original))!));
        AssertSameTree("SpanJson deserialize", expected,
            ReEmit(SpanJson.JsonSerializer.Generic.Utf8.Deserialize<T>(original)!));
        AssertSameTree("Utf8Json deserialize", expected,
            ReEmit(Utf8Json.JsonSerializer.Deserialize<T>(original)!));

        // WRITE path: serialize the reference model with each library, read the bytes
        // back + re-emit through STJ, compare to the file. Catches a library dropping
        // data while writing.
        AssertSameTree("STJSrcGen serialize", expected,
            ReadBackReEmit(JsonSerializer.SerializeToUtf8Bytes(model, srcGen)));
        AssertSameTree("Newtonsoft serialize", expected,
            ReadBackReEmit(Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(model))));
        AssertSameTree("SpanJson serialize", expected,
            ReadBackReEmit(SpanJson.JsonSerializer.Generic.Utf8.Serialize(model)));
        AssertSameTree("Utf8Json serialize", expected,
            ReadBackReEmit(Utf8Json.JsonSerializer.Serialize(model)));

        // Re-emit a model through STJ as a canonical tree.
        static JsonNode? ReEmit(T m) => JsonNode.Parse(JsonSerializer.SerializeToUtf8Bytes(m));

        // Read a library's bytes back through STJ, then re-emit as a canonical tree.
        static JsonNode? ReadBackReEmit(byte[] utf8) =>
            JsonNode.Parse(JsonSerializer.SerializeToUtf8Bytes(JsonSerializer.Deserialize<T>(utf8)!));
    }

    private static void AssertSameTree(string label, JsonNode? expected, JsonNode? actual) =>
        Assert.True(JsonNode.DeepEquals(expected, actual),
            $"{label}: decoded JSON tree differs from the source file (data lost or altered).");
}
