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
| SpanJson_Deser   |  5.681 μs | 0.1132 μs | 0.2902 μs |    185,104 |  5.527 μs |      77.00 |                     136.40 |                  25248739.14 |
| STJSrcGen_Deser  |  7.505 μs | 0.1497 μs | 0.3529 μs |    136,208 |  7.311 μs |      66.00 |                     176.09 |                  23984829.50 |
| STJRefGen_Deser  |  7.885 μs | 0.1570 μs | 0.3791 μs |    128,080 |  7.691 μs |      69.00 |                     184.54 |                  23636311.02 |
| Utf8Json_Deser   |  8.628 μs | 0.1716 μs | 0.4491 μs |     98,726 |  8.389 μs |      80.00 |                     220.20 |                  21739364.66 |
| Newtonsoft_Deser | 15.468 μs | 0.3073 μs | 0.9014 μs |     55,536 | 15.018 μs |      99.00 |                     376.22 |                  20893833.21 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.786 μs | 0.0552 μs | 0.1280 μs |    371,184 |  2.718 μs |      64.00 |                      70.25 |                  26075285.36 |
| STJRefGen_Ser    |  3.911 μs | 0.0782 μs | 0.1949 μs |    258,832 |  3.799 μs |      73.00 |                     100.28 |                  25954825.66 |
| SpanJson_Ser     |  3.971 μs | 0.0791 μs | 0.2099 μs |    220,976 |  3.878 μs |      82.00 |                      98.54 |                  21775449.19 |
| Utf8Json_Ser     |  5.326 μs | 0.1057 μs | 0.2408 μs |    193,792 |  5.191 μs |      62.00 |                     124.80 |                  24185724.57 |
| Newtonsoft_Ser   |  9.143 μs | 0.1967 μs | 0.5800 μs |     94,432 |  8.848 μs |     100.00 |                     231.67 |                  21877347.44 |
