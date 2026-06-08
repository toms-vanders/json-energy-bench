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
| SpanJson_Deser   |  5.684 μs | 0.1132 μs | 0.2624 μs |    178,528 |  5.547 μs |      64.00 |                     141.92 |                  25336459.21 |
| STJRefGen_Deser  |  7.750 μs | 0.1545 μs | 0.3580 μs |    134,416 |  7.562 μs |      64.00 |                     207.04 |                  27830030.34 |
| STJSrcGen_Deser  |  7.762 μs | 0.1547 μs | 0.3427 μs |    132,848 |  7.567 μs |      59.00 |                     200.55 |                  26642892.39 |
| Utf8Json_Deser   |  8.688 μs | 0.1725 μs | 0.4066 μs |    100,940 |  8.482 μs |      66.00 |                     223.13 |                  22522736.14 |
| Newtonsoft_Deser | 14.372 μs | 0.2846 μs | 0.7193 μs |     71,920 | 13.949 μs |      75.00 |                     361.64 |                  26008871.54 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.801 μs | 0.0558 μs | 0.1409 μs |    368,848 |  2.727 μs |      75.00 |                      75.12 |                  27707590.66 |
| STJRefGen_Ser    |  3.945 μs | 0.0784 μs | 0.1879 μs |    260,864 |  3.845 μs |      68.00 |                     104.06 |                  27144878.20 |
| SpanJson_Ser     |  4.075 μs | 0.0807 μs | 0.2069 μs |    217,936 |  3.975 μs |      77.00 |                     100.83 |                  21975043.22 |
| Utf8Json_Ser     |  5.273 μs | 0.1048 μs | 0.2300 μs |    184,992 |  5.159 μs |      58.00 |                     134.16 |                  24817712.89 |
| Newtonsoft_Ser   |  9.194 μs | 0.1838 μs | 0.5032 μs |     94,192 |  8.923 μs |      87.00 |                     242.46 |                  22837471.40 |
