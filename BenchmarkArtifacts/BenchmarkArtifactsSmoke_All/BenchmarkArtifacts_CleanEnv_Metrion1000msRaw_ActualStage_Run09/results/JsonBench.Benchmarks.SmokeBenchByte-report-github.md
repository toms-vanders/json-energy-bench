```

BenchmarkDotNet v0.15.5-develop (2026-05-26), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Coffee Lake), 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.108
  [Host] : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  Affinity=00000100  
IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   |  7.451 μs | 0.0812 μs | 0.0760 μs |    135,493 |      15.00 |                      80.25 |                  10872878.08 |
| STJSrcGen_Deser  |  9.750 μs | 0.1608 μs | 0.1505 μs |    103,248 |      15.00 |                      99.98 |                  10322835.69 |
| STJRefGen_Deser  |  9.825 μs | 0.1236 μs | 0.1156 μs |    102,560 |      15.00 |                     110.58 |                  11341059.59 |
| Utf8Json_Deser   | 11.287 μs | 0.1150 μs | 0.1076 μs |     89,248 |      15.00 |                     106.01 |                   9461545.66 |
| Newtonsoft_Deser | 18.775 μs | 0.2382 μs | 0.2228 μs |     53,840 |      15.00 |                     120.47 |                   6486054.87 |
|                  |           |           |           |            |            |                            |                              |
| STJSrcGen_Ser    |  3.641 μs | 0.0467 μs | 0.0437 μs |    277,184 |      15.00 |                      33.02 |                   9152345.67 |
| STJRefGen_Ser    |  4.713 μs | 0.0579 μs | 0.0541 μs |    213,664 |      15.00 |                      32.39 |                   6919869.83 |
| SpanJson_Ser     |  6.384 μs | 0.1136 μs | 0.1062 μs |    156,374 |      15.00 |                      93.38 |                  14602823.61 |
| Utf8Json_Ser     |  6.776 μs | 0.0815 μs | 0.0763 μs |    148,673 |      15.00 |                      52.56 |                   7814330.78 |
| Newtonsoft_Ser   | 12.062 μs | 0.1834 μs | 0.1716 μs |     84,176 |      15.00 |                     105.30 |                   8863528.01 |
