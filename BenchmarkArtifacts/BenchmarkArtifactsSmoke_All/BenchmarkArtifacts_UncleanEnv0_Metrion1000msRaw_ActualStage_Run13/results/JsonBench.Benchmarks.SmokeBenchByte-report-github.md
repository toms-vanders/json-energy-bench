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
| SpanJson_Deser   |  5.665 μs | 0.1127 μs | 0.2657 μs |    180,256 |  5.507 μs |      66.00 |                     140.33 |                  25296177.92 |
| STJSrcGen_Deser  |  7.656 μs | 0.1521 μs | 0.4214 μs |    111,680 |  7.420 μs |      89.00 |                     201.67 |                  22522358.40 |
| STJRefGen_Deser  |  7.783 μs | 0.1548 μs | 0.3884 μs |    130,656 |  7.578 μs |      74.00 |                     195.95 |                  25602555.61 |
| Utf8Json_Deser   |  8.272 μs | 0.1634 μs | 0.3753 μs |    105,568 |  8.066 μs |      63.00 |                     216.00 |                  22802266.61 |
| Newtonsoft_Deser | 15.363 μs | 0.3049 μs | 0.7127 μs |     66,000 | 14.961 μs |      65.00 |                     394.72 |                  26051367.79 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.816 μs | 0.0560 μs | 0.1330 μs |    364,560 |  2.737 μs |      67.00 |                      74.74 |                  27248902.22 |
| STJRefGen_Ser    |  3.990 μs | 0.0795 μs | 0.1964 μs |    255,408 |  3.886 μs |      72.00 |                     104.97 |                  26810589.71 |
| SpanJson_Ser     |  4.260 μs | 0.0847 μs | 0.2047 μs |    208,928 |  4.157 μs |      69.00 |                     109.77 |                  22934746.16 |
| Utf8Json_Ser     |  5.273 μs | 0.1051 μs | 0.2414 μs |    193,936 |  5.143 μs |      63.00 |                     135.26 |                  26232583.73 |
| Newtonsoft_Ser   |  9.232 μs | 0.1896 μs | 0.5591 μs |     93,840 |  8.929 μs |     100.00 |                     248.57 |                  23326041.32 |
