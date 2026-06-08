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
| SpanJson_Deser   |  5.698 μs | 0.1132 μs | 0.2647 μs |    179,040 |  5.541 μs |      65.00 |                     145.05 |                  25969958.69 |
| STJSrcGen_Deser  |  7.629 μs | 0.1513 μs | 0.3711 μs |    132,080 |  7.407 μs |      71.00 |                     202.62 |                  26762480.74 |
| STJRefGen_Deser  |  7.759 μs | 0.1541 μs | 0.3602 μs |    130,560 |  7.542 μs |      65.00 |                     196.80 |                  25694551.88 |
| Utf8Json_Deser   |  8.728 μs | 0.1726 μs | 0.4034 μs |    100,468 |  8.510 μs |      65.00 |                     227.76 |                  22882461.66 |
| Newtonsoft_Deser | 15.631 μs | 0.3108 μs | 0.8716 μs |     54,880 | 15.192 μs |      91.00 |                     389.32 |                  21365887.88 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.797 μs | 0.0557 μs | 0.1268 μs |    367,184 |  2.725 μs |      62.00 |                      73.92 |                  27140493.06 |
| STJRefGen_Ser    |  3.968 μs | 0.0792 μs | 0.1771 μs |    257,856 |  3.864 μs |      60.00 |                     102.75 |                  26494630.85 |
| SpanJson_Ser     |  4.156 μs | 0.0831 μs | 0.2053 μs |    212,896 |  4.071 μs |      72.00 |                     109.59 |                  23330884.25 |
| Utf8Json_Ser     |  5.202 μs | 0.1039 μs | 0.2530 μs |    167,712 |  5.066 μs |      70.00 |                     130.34 |                  21858919.90 |
| Newtonsoft_Ser   |  9.121 μs | 0.1952 μs | 0.5757 μs |     94,048 |  8.817 μs |     100.00 |                     239.47 |                  22521969.08 |
