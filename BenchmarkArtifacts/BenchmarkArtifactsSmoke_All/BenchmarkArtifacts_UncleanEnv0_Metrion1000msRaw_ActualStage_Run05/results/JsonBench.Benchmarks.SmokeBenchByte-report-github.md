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
| SpanJson_Deser   |  5.680 μs | 0.1135 μs | 0.2740 μs |    181,504 |  5.526 μs |      69.00 |                     144.24 |                  26179518.97 |
| STJSrcGen_Deser  |  7.595 μs | 0.1509 μs | 0.3672 μs |    135,440 |  7.399 μs |      70.00 |                     199.51 |                  27021637.73 |
| STJRefGen_Deser  |  7.798 μs | 0.1554 μs | 0.3571 μs |    132,752 |  7.598 μs |      63.00 |                     204.40 |                  27134980.82 |
| Utf8Json_Deser   |  8.370 μs | 0.1658 μs | 0.3973 μs |    105,124 |  8.168 μs |      68.00 |                     208.82 |                  21951662.63 |
| Newtonsoft_Deser | 15.526 μs | 0.3098 μs | 0.8585 μs |     55,536 | 15.052 μs |      89.00 |                     384.09 |                  21330791.62 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.798 μs | 0.0557 μs | 0.1457 μs |    365,504 |  2.713 μs |      80.00 |                      74.16 |                  27104991.26 |
| STJRefGen_Ser    |  4.020 μs | 0.0799 μs | 0.1853 μs |    253,776 |  3.908 μs |      64.00 |                     108.03 |                  27414437.96 |
| SpanJson_Ser     |  4.580 μs | 0.0905 μs | 0.2168 μs |    195,296 |  4.469 μs |      68.00 |                     111.85 |                  21843171.41 |
| Utf8Json_Ser     |  5.232 μs | 0.1041 μs | 0.2196 μs |    190,224 |  5.123 μs |      54.00 |                     134.14 |                  25516850.93 |
| Newtonsoft_Ser   |  9.259 μs | 0.1899 μs | 0.5601 μs |     92,880 |  8.972 μs |     100.00 |                     234.95 |                  21821807.37 |
