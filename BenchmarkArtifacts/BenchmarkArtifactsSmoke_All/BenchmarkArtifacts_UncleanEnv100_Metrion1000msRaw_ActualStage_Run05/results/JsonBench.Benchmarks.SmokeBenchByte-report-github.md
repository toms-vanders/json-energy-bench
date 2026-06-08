```

BenchmarkDotNet v0.15.5-develop (2026-05-17), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 4.20GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   | 12.007 μs | 0.2394 μs | 0.3028 μs |     84,237 | 11.910 μs |      23.00 |                      67.69 |                   5701806.14 |
| STJSrcGen_Deser  | 15.642 μs | 0.3116 μs | 0.3588 μs |     64,832 | 15.515 μs |      20.00 |                      46.75 |                   3030938.52 |
| STJRefGen_Deser  | 15.745 μs | 0.3146 μs | 0.5985 μs |     64,176 | 15.577 μs |      45.00 |                      93.78 |                   6018424.28 |
| Utf8Json_Deser   | 17.538 μs | 0.2856 μs | 0.2671 μs |     58,076 | 17.458 μs |      15.00 |                      87.83 |                   5100919.34 |
| Newtonsoft_Deser | 31.039 μs | 0.7602 μs | 2.2416 μs |     34,687 | 31.172 μs |     100.00 |                     159.69 |                   5539076.03 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  5.671 μs | 0.0181 μs | 0.0169 μs |    169,200 |  5.670 μs |      15.00 |                      30.67 |                   5189833.39 |
| STJRefGen_Ser    |  7.989 μs | 0.0912 μs | 0.0853 μs |    125,616 |  7.950 μs |      15.00 |                      42.90 |                   5389038.25 |
| SpanJson_Ser     |  8.885 μs | 0.1750 μs | 0.2724 μs |    110,299 |  8.786 μs |      32.00 |                      49.73 |                   5485540.64 |
| Utf8Json_Ser     | 10.780 μs | 0.2091 μs | 0.2791 μs |     93,549 | 10.676 μs |      25.00 |                      61.61 |                   5763544.66 |
| Newtonsoft_Ser   | 17.899 μs | 0.4960 μs | 1.4624 μs |     59,853 | 17.077 μs |     100.00 |                      85.69 |                   5128755.51 |
