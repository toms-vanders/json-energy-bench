using System.Runtime.CompilerServices;

namespace Serialization.Bench.Helpers;

public static class SerializationHelper
{
    private static string RepoRoot([CallerFilePath] string callerFilePath = "")
        => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(callerFilePath)!, "..", ".."));

    public static string TestDataPath(string subset = "")
        => Path.Combine(RepoRoot(), "TestData", subset);

    public static string TestDataFile(string subset, string fileName)
        => Path.Combine(TestDataPath(subset), fileName);

    public static string BenchmarkArtifactPath()
        => Path.Combine(RepoRoot(), "BenchmarkArtifacts");
}
