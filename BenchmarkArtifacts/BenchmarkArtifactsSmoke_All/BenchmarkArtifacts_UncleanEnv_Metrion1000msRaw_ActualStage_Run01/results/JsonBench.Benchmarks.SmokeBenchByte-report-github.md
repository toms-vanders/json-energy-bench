```

BenchmarkDotNet v0.15.5-develop (2026-05-15), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 0.80GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   |  5.802 μs | 0.1147 μs | 0.2941 μs |    144,752 |  5.635 μs |      77.00 |                     153.54 |                  22225401.16 |
| STJSrcGen_Deser  |  7.827 μs | 0.1562 μs | 0.3713 μs |    105,712 |  7.616 μs |      67.00 |                     168.78 |                  17841646.45 |
| STJRefGen_Deser  |  7.975 μs | 0.1593 μs | 0.4055 μs |    130,992 |  7.813 μs |      76.00 |                     198.56 |                  26009878.45 |
| Utf8Json_Deser   |  9.026 μs | 0.1792 μs | 0.4874 μs |     97,372 |  8.784 μs |      86.00 |                     247.72 |                  24120683.03 |
| Newtonsoft_Deser | 14.805 μs | 0.2942 μs | 0.8003 μs |     58,032 | 14.416 μs |      86.00 |                     332.07 |                  19270751.86 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.843 μs | 0.0569 μs | 0.1678 μs |    300,288 |  2.748 μs |     100.00 |                      76.47 |                  22964379.37 |
| STJRefGen_Ser    |  4.149 μs | 0.0827 μs | 0.2279 μs |    203,760 |  4.037 μs |      88.00 |                     118.72 |                  24190546.96 |
| SpanJson_Ser     |  4.542 μs | 0.0906 μs | 0.2007 μs |    193,648 |  4.433 μs |      59.00 |                     112.49 |                  21783349.66 |
| Utf8Json_Ser     |  5.398 μs | 0.1075 μs | 0.2513 μs |    156,528 |  5.256 μs |      65.00 |                     138.51 |                  21679923.29 |
| Newtonsoft_Ser   |  9.480 μs | 0.1919 μs | 0.5657 μs |     91,232 |  9.187 μs |     100.00 |                     254.42 |                  23211260.57 |
