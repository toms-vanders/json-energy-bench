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
| SpanJson_Deser   |  7.405 μs | 0.1097 μs | 0.1026 μs |    136,613 |      15.00 |                      85.77 |                  11717136.60 |
| STJSrcGen_Deser  |  9.737 μs | 0.1171 μs | 0.1095 μs |    103,712 |      15.00 |                     101.45 |                  10521361.84 |
| STJRefGen_Deser  | 10.155 μs | 0.1173 μs | 0.1097 μs |     99,504 |      15.00 |                     133.86 |                  13319164.40 |
| Utf8Json_Deser   | 11.335 μs | 0.1736 μs | 0.1624 μs |     88,731 |      15.00 |                     136.24 |                  12089003.11 |
| Newtonsoft_Deser | 18.759 μs | 0.2365 μs | 0.2212 μs |     53,840 |      15.00 |                     198.85 |                  10705976.46 |
|                  |           |           |           |            |            |                            |                              |
| STJSrcGen_Ser    |  3.614 μs | 0.0477 μs | 0.0446 μs |    278,528 |      15.00 |                      39.92 |                  11118238.43 |
| STJRefGen_Ser    |  4.807 μs | 0.0574 μs | 0.0537 μs |    209,296 |      15.00 |                      44.54 |                   9321030.08 |
| Utf8Json_Ser     |  6.735 μs | 0.1181 μs | 0.1105 μs |    149,845 |      15.00 |                      71.75 |                  10751795.35 |
| SpanJson_Ser     |  7.091 μs | 0.1102 μs | 0.1031 μs |    142,163 |      15.00 |                      89.51 |                  12725484.14 |
| Newtonsoft_Ser   | 12.046 μs | 0.1736 μs | 0.1624 μs |     84,608 |      15.00 |                     122.77 |                  10387741.33 |
