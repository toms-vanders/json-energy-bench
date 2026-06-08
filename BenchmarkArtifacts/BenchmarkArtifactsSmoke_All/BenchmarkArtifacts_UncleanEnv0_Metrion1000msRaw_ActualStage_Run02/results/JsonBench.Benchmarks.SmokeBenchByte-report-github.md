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
| SpanJson_Deser   |  5.666 μs | 0.1116 μs | 0.2355 μs |    181,600 |  5.526 μs |      54.00 |                     139.01 |                  25245115.05 |
| STJSrcGen_Deser  |  7.647 μs | 0.1518 μs | 0.3201 μs |    135,456 |  7.467 μs |      54.00 |                     196.49 |                  26615999.76 |
| STJRefGen_Deser  |  7.916 μs | 0.1579 μs | 0.3844 μs |    131,504 |  7.719 μs |      70.00 |                     201.22 |                  26461012.12 |
| Utf8Json_Deser   |  8.770 μs | 0.1730 μs | 0.4010 μs |    100,048 |  8.566 μs |      64.00 |                     232.09 |                  23220261.67 |
| Newtonsoft_Deser | 15.495 μs | 0.3096 μs | 0.8832 μs |     54,512 | 15.052 μs |      94.00 |                     391.05 |                  21316698.38 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.801 μs | 0.0559 μs | 0.1338 μs |    364,928 |  2.732 μs |      68.00 |                      71.89 |                  26236013.30 |
| STJRefGen_Ser    |  4.001 μs | 0.0793 μs | 0.1975 μs |    255,728 |  3.887 μs |      73.00 |                     103.70 |                  26518085.68 |
| SpanJson_Ser     |  4.066 μs | 0.0807 μs | 0.2010 μs |    219,856 |  3.977 μs |      73.00 |                     101.79 |                  22379832.81 |
| Utf8Json_Ser     |  5.315 μs | 0.1059 μs | 0.2636 μs |    166,448 |  5.162 μs |      73.00 |                     133.04 |                  22144503.85 |
| Newtonsoft_Ser   |  9.287 μs | 0.1942 μs | 0.5727 μs |     91,440 |  8.988 μs |     100.00 |                     242.20 |                  22147039.86 |
