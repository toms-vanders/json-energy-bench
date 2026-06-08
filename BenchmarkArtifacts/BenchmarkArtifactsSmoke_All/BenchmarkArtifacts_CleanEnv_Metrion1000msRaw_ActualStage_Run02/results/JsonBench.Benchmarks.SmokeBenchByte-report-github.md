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
| SpanJson_Deser   |  7.382 μs | 0.0922 μs | 0.0862 μs |    136,411 |      15.00 |                      62.10 |                   8470628.45 |
| STJRefGen_Deser  |  9.994 μs | 0.1233 μs | 0.1154 μs |    100,848 |      15.00 |                     127.57 |                  12864722.89 |
| STJSrcGen_Deser  | 10.053 μs | 0.0998 μs | 0.0934 μs |    100,304 |      15.00 |                     110.96 |                  11129320.64 |
| Utf8Json_Deser   | 11.316 μs | 0.1130 μs | 0.1057 μs |     88,985 |      15.00 |                     123.88 |                  11023637.73 |
| Newtonsoft_Deser | 18.767 μs | 0.2301 μs | 0.2153 μs |     53,856 |      15.00 |                     203.33 |                  10950393.21 |
|                  |           |           |           |            |            |                            |                              |
| STJSrcGen_Ser    |  3.646 μs | 0.0441 μs | 0.0413 μs |    276,048 |      15.00 |                      38.41 |                  10602703.04 |
| STJRefGen_Ser    |  4.930 μs | 0.0609 μs | 0.0570 μs |    204,816 |      15.00 |                      56.00 |                  11469561.33 |
| SpanJson_Ser     |  6.276 μs | 0.1205 μs | 0.1127 μs |    162,197 |      15.00 |                      79.60 |                  12910870.06 |
| Utf8Json_Ser     |  6.707 μs | 0.0746 μs | 0.0698 μs |    150,096 |      15.00 |                      84.20 |                  12638625.18 |
| Newtonsoft_Ser   | 12.076 μs | 0.1735 μs | 0.1623 μs |     83,904 |      15.00 |                     130.34 |                  10936334.33 |
