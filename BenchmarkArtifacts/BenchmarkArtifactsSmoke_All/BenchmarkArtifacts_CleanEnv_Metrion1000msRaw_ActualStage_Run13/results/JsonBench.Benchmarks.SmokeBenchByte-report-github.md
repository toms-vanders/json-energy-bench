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
| SpanJson_Deser   |  7.543 μs | 0.1502 μs | 0.2294 μs |     44,698 |      31.00 |                     111.10 |                   4966020.66 |
| STJSrcGen_Deser  |  9.946 μs | 0.1680 μs | 0.1571 μs |    101,504 |      15.00 |                     141.04 |                  14316186.92 |
| STJRefGen_Deser  | 10.017 μs | 0.1176 μs | 0.1100 μs |    100,784 |      15.00 |                      86.65 |                   8732534.07 |
| Utf8Json_Deser   | 10.953 μs | 0.2028 μs | 0.1992 μs |     91,966 |      16.00 |                     100.26 |                   9220058.28 |
| Newtonsoft_Deser | 18.584 μs | 0.2460 μs | 0.2301 μs |     54,272 |      15.00 |                     155.66 |                   8447808.63 |
|                  |           |           |           |            |            |                            |                              |
| STJSrcGen_Ser    |  3.647 μs | 0.0436 μs | 0.0408 μs |    276,896 |      15.00 |                      47.77 |                  13227913.86 |
| STJRefGen_Ser    |  4.741 μs | 0.0480 μs | 0.0449 μs |    211,696 |      15.00 |                      52.66 |                  11148744.14 |
| Utf8Json_Ser     |  6.870 μs | 0.1108 μs | 0.1036 μs |    147,579 |      15.00 |                      57.19 |                   8440154.83 |
| SpanJson_Ser     |  7.072 μs | 0.0807 μs | 0.0755 μs |    142,562 |      15.00 |                      97.65 |                  13921335.53 |
| Newtonsoft_Ser   | 11.980 μs | 0.1838 μs | 0.1719 μs |     84,576 |      15.00 |                     162.55 |                  13747898.97 |
