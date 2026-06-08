```

BenchmarkDotNet v0.15.5-develop (2026-05-17), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 4.30GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   | 12.926 μs | 0.1564 μs | 0.1463 μs |     86,823 | 12.886 μs |      15.00 |                      55.82 |                   4846160.38 |
| STJRefGen_Deser  | 15.270 μs | 0.3446 μs | 1.0160 μs |     62,656 | 14.966 μs |     100.00 |                      73.40 |                   4599044.60 |
| STJSrcGen_Deser  | 15.470 μs | 0.3981 μs | 1.1738 μs |     65,888 | 15.193 μs |     100.00 |                      85.05 |                   5603539.52 |
| Utf8Json_Deser   | 20.095 μs | 0.4486 μs | 1.3226 μs |     50,429 | 19.612 μs |     100.00 |                     115.07 |                   5802778.53 |
| Newtonsoft_Deser | 31.423 μs | 0.7552 μs | 2.2267 μs |     28,907 | 30.650 μs |     100.00 |                     168.90 |                   4882283.62 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  5.878 μs | 0.1625 μs | 0.4791 μs |    157,936 |  5.803 μs |     100.00 |                      31.06 |                   4904913.03 |
| SpanJson_Ser     |  8.521 μs | 0.1698 μs | 0.4504 μs |    114,169 |  8.394 μs |      82.00 |                      51.38 |                   5865718.01 |
| STJRefGen_Ser    |  8.729 μs | 0.0866 μs | 0.0810 μs |    122,352 |  8.687 μs |      15.00 |                      38.39 |                   4697641.56 |
| Utf8Json_Ser     | 10.441 μs | 0.2308 μs | 0.6804 μs |     85,734 | 10.224 μs |     100.00 |                      59.39 |                   5091443.27 |
| Newtonsoft_Ser   | 16.989 μs | 0.5070 μs | 1.4949 μs |     57,002 | 16.693 μs |     100.00 |                     101.41 |                   5780468.18 |
