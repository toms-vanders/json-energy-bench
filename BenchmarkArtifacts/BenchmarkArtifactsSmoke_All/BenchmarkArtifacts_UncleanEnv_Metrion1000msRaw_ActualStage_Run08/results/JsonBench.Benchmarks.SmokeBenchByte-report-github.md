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
| SpanJson_Deser   |  5.684 μs | 0.1136 μs | 0.3128 μs |    145,376 |  5.538 μs |      88.00 |                     142.32 |                  20690367.25 |
| STJSrcGen_Deser  |  7.669 μs | 0.1528 μs | 0.4079 μs |    109,024 |  7.445 μs |      83.00 |                     186.02 |                  20280517.29 |
| STJRefGen_Deser  |  7.784 μs | 0.1549 μs | 0.4025 μs |    133,488 |  7.545 μs |      79.00 |                     183.44 |                  24486825.56 |
| Utf8Json_Deser   |  9.133 μs | 0.1823 μs | 0.4077 μs |     94,368 |  8.924 μs |      60.00 |                     226.24 |                  21350131.04 |
| Newtonsoft_Deser | 14.472 μs | 0.2885 μs | 0.7549 μs |     58,720 | 14.060 μs |      80.00 |                     359.25 |                  21095297.30 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.801 μs | 0.0596 μs | 0.1757 μs |    294,416 |  2.713 μs |     100.00 |                      72.37 |                  21306956.74 |
| STJRefGen_Ser    |  4.042 μs | 0.0806 μs | 0.2193 μs |    203,728 |  3.933 μs |      86.00 |                     101.65 |                  20708537.38 |
| SpanJson_Ser     |  4.458 μs | 0.0891 μs | 0.2101 μs |    197,360 |  4.364 μs |      66.00 |                     110.67 |                  21842305.69 |
| Utf8Json_Ser     |  5.328 μs | 0.1059 μs | 0.2368 μs |    163,232 |  5.202 μs |      60.00 |                     131.38 |                  21445891.76 |
| Newtonsoft_Ser   |  9.369 μs | 0.1860 μs | 0.5217 μs |     91,792 |  9.084 μs |      91.00 |                     229.53 |                  21069140.67 |
