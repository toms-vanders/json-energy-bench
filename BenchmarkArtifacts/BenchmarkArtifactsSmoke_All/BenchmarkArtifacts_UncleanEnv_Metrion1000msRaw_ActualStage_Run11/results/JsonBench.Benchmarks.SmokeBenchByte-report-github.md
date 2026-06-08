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
| SpanJson_Deser   |  5.701 μs | 0.1140 μs | 0.2818 μs |    150,672 |  5.545 μs |      72.00 |                     136.10 |                  20505799.75 |
| STJSrcGen_Deser  |  7.684 μs | 0.1529 μs | 0.3807 μs |    111,520 |  7.490 μs |      73.00 |                     189.22 |                  21102193.85 |
| STJRefGen_Deser  |  7.837 μs | 0.1565 μs | 0.4440 μs |    106,384 |  7.592 μs |      93.00 |                     191.26 |                  20347016.66 |
| Utf8Json_Deser   |  9.702 μs | 0.1935 μs | 0.4782 μs |     88,672 |  9.449 μs |      72.00 |                     239.03 |                  21195593.89 |
| Newtonsoft_Deser | 14.650 μs | 0.2913 μs | 0.7361 μs |     58,928 | 14.241 μs |      75.00 |                     354.93 |                  20915320.45 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.794 μs | 0.0557 μs | 0.1534 μs |    298,608 |  2.711 μs |      88.00 |                      71.23 |                  21268774.71 |
| STJRefGen_Ser    |  4.050 μs | 0.0807 μs | 0.2303 μs |    200,224 |  3.931 μs |      94.00 |                     103.18 |                  20658938.62 |
| SpanJson_Ser     |  4.418 μs | 0.0879 μs | 0.1775 μs |    203,264 |  4.339 μs |      50.00 |                     110.23 |                  22406186.08 |
| Utf8Json_Ser     |  5.319 μs | 0.1059 μs | 0.2695 μs |    166,928 |  5.181 μs |      76.00 |                     129.45 |                  21609629.80 |
| Newtonsoft_Ser   |  9.231 μs | 0.1836 μs | 0.5385 μs |     93,472 |  8.951 μs |      99.00 |                     227.81 |                  21293459.46 |
