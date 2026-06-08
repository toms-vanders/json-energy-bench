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
| SpanJson_Deser   |  5.698 μs | 0.1127 μs | 0.2699 μs |    180,688 |  5.539 μs |      68.00 |                     143.83 |                  25989213.55 |
| STJSrcGen_Deser  |  7.563 μs | 0.1505 μs | 0.4145 μs |    113,008 |  7.340 μs |      88.00 |                     195.73 |                  22119122.29 |
| STJRefGen_Deser  |  7.723 μs | 0.1536 μs | 0.3710 μs |    133,760 |  7.511 μs |      69.00 |                     200.87 |                  26867879.81 |
| Utf8Json_Deser   |  8.630 μs | 0.1716 μs | 0.3942 μs |    101,678 |  8.406 μs |      63.00 |                     231.79 |                  23567507.75 |
| Newtonsoft_Deser | 14.496 μs | 0.2900 μs | 0.7433 μs |     58,656 | 14.090 μs |      77.00 |                     370.72 |                  21745082.73 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.826 μs | 0.0580 μs | 0.1711 μs |    297,904 |  2.739 μs |     100.00 |                      74.55 |                  22209831.52 |
| STJRefGen_Ser    |  4.005 μs | 0.0797 μs | 0.1909 μs |    251,888 |  3.921 μs |      68.00 |                     109.17 |                  27498890.69 |
| SpanJson_Ser     |  4.765 μs | 0.0950 μs | 0.2025 μs |    186,304 |  4.683 μs |      55.00 |                     120.63 |                  22474622.46 |
| Utf8Json_Ser     |  5.278 μs | 0.1054 μs | 0.2358 μs |    166,240 |  5.164 μs |      60.00 |                     133.44 |                  22183577.51 |
| Newtonsoft_Ser   |  9.188 μs | 0.1837 μs | 0.4808 μs |     94,528 |  8.935 μs |      80.00 |                     236.23 |                  22330223.46 |
