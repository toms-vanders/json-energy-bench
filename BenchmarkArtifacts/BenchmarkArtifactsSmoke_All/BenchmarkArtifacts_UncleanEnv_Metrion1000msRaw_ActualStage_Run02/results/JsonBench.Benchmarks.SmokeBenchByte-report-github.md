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
| SpanJson_Deser   |  5.882 μs | 0.1167 μs | 0.3075 μs |    139,552 |  5.723 μs |      81.00 |                     179.07 |                  24990138.96 |
| STJSrcGen_Deser  |  7.776 μs | 0.1540 μs | 0.3947 μs |    108,640 |  7.547 μs |      77.00 |                     209.74 |                  22786304.97 |
| STJRefGen_Deser  |  7.899 μs | 0.1572 μs | 0.3944 μs |    130,912 |  7.699 μs |      74.00 |                     216.79 |                  28379801.87 |
| Utf8Json_Deser   |  9.921 μs | 0.1975 μs | 0.5169 μs |     89,639 |  9.671 μs |      80.00 |                     249.32 |                  22348909.12 |
| Newtonsoft_Deser | 14.848 μs | 0.2946 μs | 0.7812 μs |     57,840 | 14.467 μs |      82.00 |                     434.40 |                  25125732.30 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.867 μs | 0.0570 μs | 0.1680 μs |    292,144 |  2.779 μs |     100.00 |                      75.41 |                  22031248.93 |
| STJRefGen_Ser    |  4.096 μs | 0.0819 μs | 0.2085 μs |    207,376 |  3.995 μs |      76.00 |                     122.10 |                  25320733.34 |
| Utf8Json_Ser     |  5.360 μs | 0.1067 μs | 0.2576 μs |    161,935 |  5.236 μs |      69.00 |                     162.22 |                  26269526.06 |
| SpanJson_Ser     |  5.623 μs | 0.1117 μs | 0.2612 μs |    160,032 |  5.493 μs |      65.00 |                     137.63 |                  22025192.48 |
| Newtonsoft_Ser   |  9.557 μs | 0.1910 μs | 0.5291 μs |     93,760 |  9.284 μs |      89.00 |                     287.02 |                  26910969.38 |
