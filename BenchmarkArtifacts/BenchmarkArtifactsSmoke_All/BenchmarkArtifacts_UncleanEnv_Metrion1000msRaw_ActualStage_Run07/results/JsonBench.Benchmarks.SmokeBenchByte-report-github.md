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
| SpanJson_Deser   |  5.744 μs | 0.1144 μs | 0.3227 μs |    145,392 |  5.579 μs |      92.00 |                     137.03 |                  19923317.68 |
| STJRefGen_Deser  |  7.738 μs | 0.1547 μs | 0.3823 μs |    133,312 |  7.539 μs |      72.00 |                     191.21 |                  25490791.25 |
| STJSrcGen_Deser  |  7.749 μs | 0.1545 μs | 0.3876 μs |    133,440 |  7.565 μs |      74.00 |                     188.60 |                  25166593.55 |
| Utf8Json_Deser   |  9.745 μs | 0.1941 μs | 0.4688 μs |     86,864 |  9.477 μs |      69.00 |                     242.43 |                  21058303.02 |
| Newtonsoft_Deser | 14.486 μs | 0.2883 μs | 0.7744 μs |     59,040 | 14.090 μs |      84.00 |                     359.32 |                  21214321.78 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.811 μs | 0.0615 μs | 0.1813 μs |    292,688 |  2.714 μs |     100.00 |                      70.70 |                  20691951.82 |
| STJRefGen_Ser    |  3.989 μs | 0.0795 μs | 0.1905 μs |    259,712 |  3.879 μs |      68.00 |                     101.60 |                  26385506.88 |
| SpanJson_Ser     |  4.509 μs | 0.0903 μs | 0.1844 μs |    200,064 |  4.411 μs |      51.00 |                     110.04 |                  22014392.93 |
| Utf8Json_Ser     |  5.276 μs | 0.1053 μs | 0.2717 μs |    168,176 |  5.143 μs |      78.00 |                     131.20 |                  22064326.07 |
| Newtonsoft_Ser   |  9.375 μs | 0.1987 μs | 0.5859 μs |     90,464 |  9.069 μs |     100.00 |                     228.44 |                  20665769.81 |
