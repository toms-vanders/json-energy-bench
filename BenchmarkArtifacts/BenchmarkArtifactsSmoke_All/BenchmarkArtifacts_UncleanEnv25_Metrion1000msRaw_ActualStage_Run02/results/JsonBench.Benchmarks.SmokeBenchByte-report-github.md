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
| SpanJson_Deser   |  6.273 μs | 0.2046 μs | 0.6033 μs |    141,571 |  6.064 μs |      100.0 |                      92.20 |                  13053277.90 |
| STJSrcGen_Deser  |  8.480 μs | 0.2296 μs | 0.6770 μs |     96,752 |  8.466 μs |      100.0 |                     122.89 |                  11889733.25 |
| STJRefGen_Deser  |  8.530 μs | 0.2258 μs | 0.6656 μs |    112,288 |  8.349 μs |      100.0 |                     121.96 |                  13694845.15 |
| Utf8Json_Deser   |  9.090 μs | 0.2886 μs | 0.8509 μs |     83,530 |  8.843 μs |      100.0 |                     133.39 |                  11142281.25 |
| Newtonsoft_Deser | 17.420 μs | 0.5150 μs | 1.5184 μs |     47,184 | 17.480 μs |      100.0 |                     255.18 |                  12040209.30 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  3.054 μs | 0.1019 μs | 0.3005 μs |    260,144 |  2.947 μs |      100.0 |                      44.97 |                  11699800.32 |
| STJRefGen_Ser    |  4.343 μs | 0.1417 μs | 0.4179 μs |    178,048 |  4.187 μs |      100.0 |                      65.21 |                  11610457.97 |
| SpanJson_Ser     |  4.778 μs | 0.1147 μs | 0.3382 μs |    181,104 |  4.746 μs |      100.0 |                      68.75 |                  12451764.15 |
| Utf8Json_Ser     |  5.787 μs | 0.1621 μs | 0.4780 μs |    139,027 |  5.526 μs |      100.0 |                      84.94 |                  11809293.62 |
| Newtonsoft_Ser   |  9.979 μs | 0.2953 μs | 0.8706 μs |     86,480 |  9.537 μs |      100.0 |                     143.32 |                  12394635.66 |
