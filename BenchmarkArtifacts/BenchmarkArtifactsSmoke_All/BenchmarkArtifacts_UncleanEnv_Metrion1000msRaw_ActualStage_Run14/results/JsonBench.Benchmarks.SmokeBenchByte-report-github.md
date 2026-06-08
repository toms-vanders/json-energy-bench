```

BenchmarkDotNet v0.15.5-develop (2026-05-16), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 0.80GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   |  5.785 μs | 0.1153 μs | 0.3097 μs |    144,560 |  5.621 μs |      84.00 |                     139.11 |                  20109852.33 |
| STJSrcGen_Deser  |  7.641 μs | 0.1515 μs | 0.3939 μs |    134,976 |  7.418 μs |      79.00 |                     179.53 |                  24232855.07 |
| STJRefGen_Deser  |  7.778 μs | 0.1541 μs | 0.3662 μs |    132,784 |  7.593 μs |      67.00 |                     187.38 |                  24881096.42 |
| Utf8Json_Deser   |  9.579 μs | 0.1908 μs | 0.5126 μs |     91,571 |  9.296 μs |      84.00 |                     229.91 |                  21052995.81 |
| Newtonsoft_Deser | 14.492 μs | 0.2884 μs | 0.7992 μs |     59,584 | 14.060 μs |      89.00 |                     345.37 |                  20578295.24 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.820 μs | 0.0564 μs | 0.1456 μs |    300,880 |  2.750 μs |      78.00 |                      70.10 |                  21090607.82 |
| STJRefGen_Ser    |  4.031 μs | 0.0800 μs | 0.2107 μs |    257,888 |  3.913 μs |      81.00 |                     101.66 |                  26217107.81 |
| SpanJson_Ser     |  4.413 μs | 0.0882 μs | 0.1917 μs |    205,408 |  4.313 μs |      57.00 |                     105.05 |                  21577753.69 |
| Utf8Json_Ser     |  5.290 μs | 0.1055 μs | 0.2446 μs |    165,680 |  5.173 μs |      64.00 |                     130.80 |                  21671374.45 |
| Newtonsoft_Ser   |  9.384 μs | 0.1864 μs | 0.5496 μs |     90,896 |  9.125 μs |     100.00 |                     227.94 |                  20719138.31 |
