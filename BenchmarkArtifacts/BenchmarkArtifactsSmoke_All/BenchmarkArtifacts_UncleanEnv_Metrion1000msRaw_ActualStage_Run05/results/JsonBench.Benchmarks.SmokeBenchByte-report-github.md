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
| SpanJson_Deser   |  5.838 μs | 0.1166 μs | 0.2633 μs |    175,984 |  5.698 μs |      61.00 |                     180.34 |                  31736189.22 |
| STJSrcGen_Deser  |  7.739 μs | 0.1526 μs | 0.3506 μs |    133,712 |  7.550 μs |      63.00 |                     224.89 |                  30070432.12 |
| STJRefGen_Deser  |  7.813 μs | 0.1560 μs | 0.3999 μs |    108,208 |  7.637 μs |      77.00 |                     223.84 |                  24221672.29 |
| Utf8Json_Deser   |  8.957 μs | 0.1775 μs | 0.4419 μs |     97,763 |  8.746 μs |      73.00 |                     261.22 |                  25537705.43 |
| Newtonsoft_Deser | 14.878 μs | 0.2953 μs | 0.8281 μs |     57,888 | 14.416 μs |      91.00 |                     381.33 |                  22074231.96 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.865 μs | 0.0571 μs | 0.1484 μs |    312,832 |  2.792 μs |      79.00 |                      94.25 |                  29484674.96 |
| STJRefGen_Ser    |  4.130 μs | 0.0825 μs | 0.1863 μs |    251,408 |  4.048 μs |      61.00 |                     111.59 |                  28054094.31 |
| SpanJson_Ser     |  4.617 μs | 0.0917 μs | 0.2013 μs |    202,928 |  4.523 μs |      58.00 |                     111.32 |                  22590051.74 |
| Utf8Json_Ser     |  5.338 μs | 0.1059 μs | 0.2618 μs |    153,856 |  5.215 μs |      72.00 |                     150.20 |                  23109696.99 |
| Newtonsoft_Ser   |  9.384 μs | 0.1861 μs | 0.5368 μs |     88,704 |  9.098 μs |      96.00 |                     277.72 |                  24634810.38 |
