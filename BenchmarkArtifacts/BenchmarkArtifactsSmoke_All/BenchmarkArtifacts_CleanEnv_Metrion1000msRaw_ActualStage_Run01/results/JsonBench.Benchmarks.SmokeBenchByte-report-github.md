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
| SpanJson_Deser   |  7.505 μs | 0.0770 μs | 0.0720 μs |    134,494 |      15.00 |                      74.61 |                  10034746.41 |
| STJSrcGen_Deser  | 10.062 μs | 0.0959 μs | 0.0897 μs |    100,256 |      15.00 |                     138.19 |                  13854082.30 |
| STJRefGen_Deser  | 10.354 μs | 0.1737 μs | 0.1625 μs |     96,512 |      15.00 |                      91.99 |                   8877734.26 |
| Utf8Json_Deser   | 10.876 μs | 0.2136 μs | 0.2098 μs |     92,828 |      16.00 |                     130.83 |                  12144320.98 |
| Newtonsoft_Deser | 20.075 μs | 0.2715 μs | 0.2540 μs |     50,144 |      15.00 |                     161.30 |                   8088071.26 |
|                  |           |           |           |            |            |                            |                              |
| STJSrcGen_Ser    |  3.702 μs | 0.0602 μs | 0.0564 μs |    272,816 |      15.00 |                      39.72 |                  10835202.51 |
| STJRefGen_Ser    |  4.762 μs | 0.0635 μs | 0.0594 μs |    211,552 |      15.00 |                      61.46 |                  13001977.70 |
| Utf8Json_Ser     |  6.672 μs | 0.1097 μs | 0.1026 μs |    148,235 |      15.00 |                      79.43 |                  11774353.37 |
| SpanJson_Ser     |  7.026 μs | 0.1046 μs | 0.0979 μs |    144,283 |      15.00 |                      98.07 |                  14149439.55 |
| Newtonsoft_Ser   | 12.072 μs | 0.1555 μs | 0.1454 μs |     84,064 |      15.00 |                      94.59 |                   7951890.87 |
