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
| SpanJson_Deser   |  5.689 μs | 0.1138 μs | 0.2875 μs |    184,976 |  5.540 μs |      75.00 |                     146.55 |                  27109093.23 |
| STJSrcGen_Deser  |  7.636 μs | 0.1515 μs | 0.4044 μs |    113,680 |  7.421 μs |      83.00 |                     200.07 |                  22743925.63 |
| STJRefGen_Deser  |  7.764 μs | 0.1529 μs | 0.3513 μs |    132,128 |  7.542 μs |      63.00 |                     198.62 |                  26243655.17 |
| Utf8Json_Deser   |  8.388 μs | 0.1677 μs | 0.3987 μs |    105,388 |  8.165 μs |      67.00 |                     217.57 |                  22929664.73 |
| Newtonsoft_Deser | 15.347 μs | 0.3057 μs | 0.8266 μs |     54,608 | 14.909 μs |      85.00 |                     397.34 |                  21697882.65 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.792 μs | 0.0554 μs | 0.1348 μs |    366,752 |  2.719 μs |      70.00 |                      75.92 |                  27844504.02 |
| STJRefGen_Ser    |  3.993 μs | 0.0797 μs | 0.1954 μs |    254,864 |  3.884 μs |      71.00 |                     103.98 |                  26501912.02 |
| SpanJson_Ser     |  4.692 μs | 0.0939 μs | 0.1960 μs |    189,696 |  4.597 μs |      53.00 |                     117.34 |                  22259060.87 |
| Utf8Json_Ser     |  5.278 μs | 0.1047 μs | 0.2446 μs |    162,928 |  5.161 μs |      65.00 |                     135.66 |                  22102681.50 |
| Newtonsoft_Ser   |  9.230 μs | 0.1961 μs | 0.5781 μs |     91,680 |  8.925 μs |     100.00 |                     239.17 |                  21927174.09 |
