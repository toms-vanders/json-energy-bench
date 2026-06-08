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
| SpanJson_Deser   |  5.741 μs | 0.1149 μs | 0.2730 μs |    180,336 |  5.582 μs |      67.00 |                     133.18 |                  24017413.47 |
| STJSrcGen_Deser  |  7.651 μs | 0.1519 μs | 0.4407 μs |    104,160 |  7.416 μs |      97.00 |                     188.12 |                  19594144.38 |
| STJRefGen_Deser  |  7.842 μs | 0.1558 μs | 0.3821 μs |    127,824 |  7.617 μs |      71.00 |                     187.65 |                  23986206.38 |
| Utf8Json_Deser   |  8.961 μs | 0.1783 μs | 0.4474 μs |     94,832 |  8.706 μs |      74.00 |                     216.22 |                  20504919.63 |
| Newtonsoft_Deser | 14.392 μs | 0.2861 μs | 0.7881 μs |     57,728 | 13.982 μs |      88.00 |                     350.14 |                  20212866.76 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.821 μs | 0.0564 μs | 0.1573 μs |    301,088 |  2.735 μs |      90.00 |                      72.07 |                  21698547.62 |
| STJRefGen_Ser    |  4.012 μs | 0.0798 μs | 0.1987 μs |    208,960 |  3.919 μs |      73.00 |                     100.39 |                  20976802.64 |
| SpanJson_Ser     |  4.700 μs | 0.0930 μs | 0.2002 μs |    192,176 |  4.618 μs |      56.00 |                     109.87 |                  21114658.04 |
| Utf8Json_Ser     |  5.314 μs | 0.1060 μs | 0.2326 μs |    165,760 |  5.199 μs |      58.00 |                     130.02 |                  21552278.17 |
| Newtonsoft_Ser   |  9.086 μs | 0.1907 μs | 0.5624 μs |     92,800 |  8.799 μs |     100.00 |                     226.20 |                  20991147.25 |
