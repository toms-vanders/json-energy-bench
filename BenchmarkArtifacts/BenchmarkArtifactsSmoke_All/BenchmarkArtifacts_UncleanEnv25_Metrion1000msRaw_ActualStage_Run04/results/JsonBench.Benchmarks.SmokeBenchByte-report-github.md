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
| SpanJson_Deser   |  6.119 μs | 0.2015 μs | 0.5941 μs |    125,232 |  5.816 μs |      100.0 |                      89.18 |                  11167697.81 |
| STJSrcGen_Deser  |  8.373 μs | 0.2479 μs | 0.7309 μs |     91,168 |  8.267 μs |      100.0 |                     124.64 |                  11363497.70 |
| STJRefGen_Deser  |  8.394 μs | 0.2178 μs | 0.6423 μs |    130,368 |  8.212 μs |      100.0 |                     119.80 |                  15618737.77 |
| Utf8Json_Deser   |  9.218 μs | 0.2796 μs | 0.8245 μs |     93,569 |  9.278 μs |      100.0 |                     132.42 |                  12390230.86 |
| Newtonsoft_Deser | 16.813 μs | 0.5143 μs | 1.5164 μs |     46,816 | 16.099 μs |      100.0 |                     248.03 |                  11611810.10 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  3.065 μs | 0.0841 μs | 0.2479 μs |    267,312 |  3.065 μs |      100.0 |                      44.77 |                  11968574.17 |
| STJRefGen_Ser    |  4.494 μs | 0.1238 μs | 0.3651 μs |    172,752 |  4.437 μs |      100.0 |                      67.48 |                  11657673.44 |
| SpanJson_Ser     |  4.817 μs | 0.1147 μs | 0.3383 μs |    175,918 |  4.802 μs |      100.0 |                      69.76 |                  12271753.49 |
| Utf8Json_Ser     |  5.764 μs | 0.1742 μs | 0.5136 μs |    130,896 |  5.560 μs |      100.0 |                      82.90 |                  10851642.67 |
| Newtonsoft_Ser   |  9.906 μs | 0.2907 μs | 0.8572 μs |     78,048 |  9.576 μs |      100.0 |                     142.80 |                  11145292.42 |
