using System.Diagnostics.Tracing;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing.Parsers;
using Perfolizer.Horology;
using Perfolizer.Mathematics.OutlierDetection;
using JsonBench.Columns;
using JsonBench.Helpers;

namespace JsonBench;

public class BenchConfig : ManualConfig
{
    public BenchConfig()
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
        AddDiagnoser(MemoryDiagnoser.Default);
        // AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig()));
        
        AddDiagnoser(new EventPipeProfiler(EventPipeProfile.CpuSampling, performExtraBenchmarksRun: true));

        // AddDiagnoser(new EventPipeProfiler(
        //     profile: EventPipeProfile.CpuSampling,
        //     providers: new[]
        //     {
        //         new EventPipeProvider(ClrTraceEventParser.ProviderName,
        //             EventLevel.Verbose,
        //             (long)(ClrTraceEventParser.Keywords.GC
        //                    | ClrTraceEventParser.Keywords.Jit
        //                    | ClrTraceEventParser.Keywords.JitTracing
        //                    | ClrTraceEventParser.Keywords.Exception)),
        //         new EventPipeProvider("System.Buffers.ArrayPoolEventSource",
        //             EventLevel.Informational, long.MaxValue),
        //     },
        //     performExtraBenchmarksRun: true));


        // AddDiagnoser(ThreadingDiagnoser.Default);
        // AddDiagnoser(PerfCollectProfiler.Default); // requires sudo
        
        AddColumn(StatisticColumn.Iterations);
        AddColumn(new InvocationCountColumn());
        
        AddExporter(CsvMeasurementsExporter.Default);
    }
}