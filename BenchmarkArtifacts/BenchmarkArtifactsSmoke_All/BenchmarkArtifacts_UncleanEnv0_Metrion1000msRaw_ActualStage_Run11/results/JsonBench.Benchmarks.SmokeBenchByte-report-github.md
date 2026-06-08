```

BenchmarkDotNet v0.15.5-develop (2026-05-26), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 0.80GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.108
  [Host] : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   |  5.701 μs | 0.1130 μs | 0.2597 μs |    178,944 |  5.551 μs |      63.00 |                     147.40 |                  26375940.38 |
| STJSrcGen_Deser  |  7.749 μs | 0.1536 μs | 0.3620 μs |    133,296 |  7.536 μs |      66.00 |                     196.44 |                  26185002.65 |
| STJRefGen_Deser  |  7.849 μs | 0.1554 μs | 0.3724 μs |    133,936 |  7.631 μs |      68.00 |                     205.20 |                  27483843.02 |
| Utf8Json_Deser   |  8.751 μs | 0.1748 μs | 0.4188 μs |    100,064 |  8.526 μs |      68.00 |                     226.69 |                  22683257.13 |
| Newtonsoft_Deser | 14.482 μs | 0.2894 μs | 0.7776 μs |     59,120 | 14.061 μs |      84.00 |                     380.79 |                  22512030.29 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.789 μs | 0.0553 μs | 0.1397 μs |    362,272 |  2.708 μs |      75.00 |                      75.12 |                  27214844.71 |
| STJRefGen_Ser    |  4.111 μs | 0.0818 μs | 0.1830 μs |    250,624 |  4.006 μs |      60.00 |                     113.02 |                  28324333.15 |
| Utf8Json_Ser     |  5.304 μs | 0.1061 μs | 0.2563 μs |    163,408 |  5.176 μs |      69.00 |                     134.07 |                  21908419.73 |
| SpanJson_Ser     |  5.429 μs | 0.1072 μs | 0.2285 μs |    161,248 |  5.329 μs |      55.00 |                     134.13 |                  21627845.69 |
| Newtonsoft_Ser   |  9.315 μs | 0.1854 μs | 0.5466 μs |     92,880 |  9.016 μs |     100.00 |                     246.50 |                  22894512.42 |
