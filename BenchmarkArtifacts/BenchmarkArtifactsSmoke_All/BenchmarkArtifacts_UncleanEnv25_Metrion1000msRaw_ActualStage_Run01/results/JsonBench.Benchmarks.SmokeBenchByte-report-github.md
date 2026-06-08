```

BenchmarkDotNet v0.15.5-develop (2026-05-17), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 0.80GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   |  6.318 μs | 0.1914 μs | 0.5644 μs |    131,591 |  6.161 μs |      100.0 |                      90.67 |                  11931484.80 |
| STJSrcGen_Deser  |  8.229 μs | 0.2188 μs | 0.6451 μs |    117,728 |  8.147 μs |      100.0 |                     116.42 |                  13706103.10 |
| STJRefGen_Deser  |  8.483 μs | 0.2081 μs | 0.6135 μs |    125,120 |  8.270 μs |      100.0 |                     119.21 |                  14915486.28 |
| Utf8Json_Deser   |  9.180 μs | 0.2858 μs | 0.8428 μs |     89,325 |  8.971 μs |      100.0 |                     131.13 |                  11713142.28 |
| Newtonsoft_Deser | 15.729 μs | 0.4496 μs | 1.3256 μs |     52,272 | 15.561 μs |      100.0 |                     229.59 |                  12001106.59 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  3.034 μs | 0.0930 μs | 0.2741 μs |    250,528 |  2.925 μs |      100.0 |                      44.66 |                  11189402.49 |
| STJRefGen_Ser    |  4.283 μs | 0.1101 μs | 0.3248 μs |    251,280 |  4.159 μs |      100.0 |                      62.63 |                  15736995.33 |
| SpanJson_Ser     |  4.792 μs | 0.1091 μs | 0.3216 μs |    179,565 |  4.694 μs |      100.0 |                      69.78 |                  12530363.72 |
| Utf8Json_Ser     |  5.839 μs | 0.1672 μs | 0.4931 μs |    141,675 |  5.821 μs |      100.0 |                      82.35 |                  11667354.52 |
| Newtonsoft_Ser   |  9.930 μs | 0.2958 μs | 0.8721 μs |     78,800 |  9.636 μs |      100.0 |                     146.39 |                  11535283.17 |
