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
| SpanJson_Deser   |  5.758 μs | 0.1146 μs | 0.2767 μs |    180,208 |  5.599 μs |      69.00 |                     139.24 |                  25091919.10 |
| STJRefGen_Deser  |  7.673 μs | 0.1534 μs | 0.3849 μs |    133,232 |  7.457 μs |      74.00 |                     185.84 |                  24759861.85 |
| STJSrcGen_Deser  |  7.684 μs | 0.1533 μs | 0.3612 μs |    134,368 |  7.472 μs |      66.00 |                     187.13 |                  25144499.49 |
| Utf8Json_Deser   |  8.867 μs | 0.1760 μs | 0.4414 μs |     98,448 |  8.642 μs |      74.00 |                     214.31 |                  21098145.84 |
| Newtonsoft_Deser | 15.602 μs | 0.3103 μs | 0.8598 μs |     55,408 | 15.210 μs |      89.00 |                     385.26 |                  21346219.73 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.811 μs | 0.0560 μs | 0.1373 μs |    297,840 |  2.742 μs |      71.00 |                      69.12 |                  20587266.04 |
| STJRefGen_Ser    |  3.994 μs | 0.0795 μs | 0.2306 μs |    196,688 |  3.885 μs |      97.00 |                     101.20 |                  19904825.84 |
| SpanJson_Ser     |  4.488 μs | 0.0895 μs | 0.1809 μs |    210,304 |  4.419 μs |      50.00 |                     104.32 |                  21937939.02 |
| Utf8Json_Ser     |  5.324 μs | 0.1065 μs | 0.2593 μs |    163,504 |  5.208 μs |      70.00 |                     134.95 |                  22064617.92 |
| Newtonsoft_Ser   |  9.267 μs | 0.1855 μs | 0.5470 μs |     92,448 |  8.984 μs |     100.00 |                     231.06 |                  21360583.55 |
