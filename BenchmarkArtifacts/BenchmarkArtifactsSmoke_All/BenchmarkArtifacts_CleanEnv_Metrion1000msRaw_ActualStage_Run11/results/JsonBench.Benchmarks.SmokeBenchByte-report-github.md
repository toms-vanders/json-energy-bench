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
| SpanJson_Deser   |  7.450 μs | 0.1475 μs | 0.2465 μs |     44,193 |      36.00 |                     113.42 |                   5012227.55 |
| STJSrcGen_Deser  |  9.902 μs | 0.0933 μs | 0.0872 μs |    101,184 |      15.00 |                     138.22 |                  13985244.22 |
| STJRefGen_Deser  | 10.183 μs | 0.1359 μs | 0.1271 μs |     99,136 |      15.00 |                     115.70 |                  11469725.10 |
| Utf8Json_Deser   | 12.424 μs | 0.2368 μs | 0.2326 μs |     81,245 |      16.00 |                     167.08 |                  13574807.22 |
| Newtonsoft_Deser | 18.720 μs | 0.2512 μs | 0.2349 μs |     53,856 |      15.00 |                     220.06 |                  11851595.02 |
|                  |           |           |           |            |            |                            |                              |
| STJSrcGen_Ser    |  3.617 μs | 0.0673 μs | 0.0629 μs |    269,440 |      15.00 |                      53.40 |                  14387755.93 |
| STJRefGen_Ser    |  4.888 μs | 0.0602 μs | 0.0563 μs |    205,920 |      15.00 |                      50.73 |                  10446886.10 |
| Utf8Json_Ser     |  6.774 μs | 0.1178 μs | 0.1102 μs |    148,503 |      15.00 |                      85.40 |                  12682390.87 |
| SpanJson_Ser     |  7.089 μs | 0.1092 μs | 0.1022 μs |    143,142 |      15.00 |                      76.64 |                  10970080.27 |
| Newtonsoft_Ser   | 12.004 μs | 0.1840 μs | 0.1721 μs |     84,480 |      15.00 |                     116.30 |                   9825023.75 |
