using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using Perfolizer.Horology;
using Perfolizer.Mathematics.OutlierDetection;
using JsonBench.Columns;
using JsonBench.Helpers;

namespace JsonBench;

public class BenchConfigMetrionUncleanEnv : ManualConfig
{
    public BenchConfigMetrionUncleanEnv()
    {
        AddJob(Job.Default
            .WithId("Energy")
            .WithIterationTime(TimeInterval.Second)
            .WithOutlierMode(OutlierMode.DontRemove)
        );

        WithArtifactsPath(SerializationHelper.BenchmarkArtifactPath());
        WithOptions(ConfigOptions.KeepBenchmarkFiles);

        AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByCategory);
        WithOrderer(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest));

        AddDiagnoser(new MetrionEnergyProfiler(new MetrionEnergyProfilerConfig(
            metrionBinaryPath: "/home/test/tools/metrion-internal/.venv/bin/metrion",
            metrionDatabaseDirectory: "/home/test/tools/metrion-internal",
            keepMetrionFiles: false,
            measurePerIteration: false,
            affinityMask: null // (IntPtr)(~(1 << 2) & 0xFFFF)
        )));

        AddColumn(StatisticColumn.Iterations);
        AddColumn(new InvocationCountColumn());

        AddExporter(CsvMeasurementsExporter.Default);
    }
}