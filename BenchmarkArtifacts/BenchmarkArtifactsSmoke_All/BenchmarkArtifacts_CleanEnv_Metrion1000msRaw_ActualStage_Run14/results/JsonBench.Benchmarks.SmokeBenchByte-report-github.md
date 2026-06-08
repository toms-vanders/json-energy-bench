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
| SpanJson_Deser   |  7.308 μs | 0.0806 μs | 0.0754 μs |    138,157 |      15.00 |                      98.07 |                  13549468.93 |
| STJSrcGen_Deser  | 10.024 μs | 0.1220 μs | 0.1142 μs |    100,576 |      15.00 |                     142.24 |                  14306092.54 |
| STJRefGen_Deser  | 10.077 μs | 0.1223 μs | 0.1144 μs |    100,240 |      15.00 |                     102.67 |                  10291694.79 |
| Utf8Json_Deser   | 11.409 μs | 0.2248 μs | 0.2207 μs |     88,423 |      16.00 |                     170.90 |                  15111246.20 |
| Newtonsoft_Deser | 18.702 μs | 0.3347 μs | 0.3131 μs |     53,936 |      15.00 |                     255.57 |                  13784586.84 |
|                  |           |           |           |            |            |                            |                              |
| STJSrcGen_Ser    |  3.566 μs | 0.0452 μs | 0.0422 μs |    282,112 |      15.00 |                      32.42 |                   9144944.26 |
| STJRefGen_Ser    |  4.775 μs | 0.0569 μs | 0.0532 μs |    210,880 |      15.00 |                      42.56 |                   8974074.09 |
| SpanJson_Ser     |  6.242 μs | 0.1092 μs | 0.1021 μs |    161,437 |      15.00 |                      82.92 |                  13386688.16 |
| Utf8Json_Ser     |  6.675 μs | 0.1232 μs | 0.1153 μs |    151,483 |      15.00 |                      49.89 |                   7556808.98 |
| Newtonsoft_Ser   | 11.970 μs | 0.1853 μs | 0.1733 μs |     84,624 |      15.00 |                     118.14 |                   9997339.63 |
