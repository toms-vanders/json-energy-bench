```

BenchmarkDotNet v0.15.5-develop (2026-05-26), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Coffee Lake), 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.108
  [Host] : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  Affinity=00000100  
IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   |  7.330 μs | 0.1453 μs | 0.1427 μs |    137,749 |  7.276 μs |      16.00 |                      93.80 |                  12921084.93 |
| STJSrcGen_Deser  |  9.788 μs | 0.1002 μs | 0.0938 μs |    102,848 |  9.729 μs |      15.00 |                     111.11 |                  11427952.98 |
| STJRefGen_Deser  | 10.159 μs | 0.1235 μs | 0.1156 μs |     99,120 | 10.122 μs |      15.00 |                      87.80 |                   8702750.56 |
| Utf8Json_Deser   | 12.400 μs | 0.1223 μs | 0.1144 μs |     81,154 | 12.332 μs |      15.00 |                     112.25 |                   9109254.72 |
| Newtonsoft_Deser | 20.269 μs | 0.2215 μs | 0.2072 μs |     49,696 | 20.173 μs |      15.00 |                     261.90 |                  13015369.49 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  3.658 μs | 0.0435 μs | 0.0407 μs |    275,936 |  3.636 μs |      15.00 |                      37.14 |                  10247407.05 |
| STJRefGen_Ser    |  4.839 μs | 0.0473 μs | 0.0443 μs |    207,760 |  4.818 μs |      15.00 |                      59.70 |                  12403812.40 |
| Utf8Json_Ser     |  6.717 μs | 0.1081 μs | 0.1011 μs |    149,135 |  6.682 μs |      15.00 |                      53.57 |                   7989513.70 |
| SpanJson_Ser     |  7.149 μs | 0.1073 μs | 0.1003 μs |    142,619 |  7.119 μs |      15.00 |                      79.09 |                  11279250.26 |
| Newtonsoft_Ser   | 12.007 μs | 0.2388 μs | 0.6332 μs |     16,416 | 11.799 μs |      82.00 |                     195.76 |                   3213545.42 |
