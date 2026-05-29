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

public class BenchConfigEnergyDiagnoserUncleanEnv : ManualConfig
{
    public BenchConfigEnergyDiagnoserUncleanEnv()
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

        AddDiagnoser(EnergyDiagnoser.Default);

        AddColumn(StatisticColumn.Iterations);
        AddColumn(new InvocationCountColumn());

        AddExporter(CsvMeasurementsExporter.Default);
    }
}