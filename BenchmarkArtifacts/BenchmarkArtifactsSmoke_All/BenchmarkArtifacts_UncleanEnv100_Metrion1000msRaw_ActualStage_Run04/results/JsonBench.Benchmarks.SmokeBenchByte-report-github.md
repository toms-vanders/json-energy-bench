```

BenchmarkDotNet v0.15.5-develop (2026-05-17), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 4.20GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   | 11.775 μs | 0.0945 μs | 0.0884 μs |     85,442 | 11.760 μs |      15.00 |                      58.45 |                   4994258.31 |
| STJSrcGen_Deser  | 15.652 μs | 0.3060 μs | 0.3143 μs |     64,752 | 15.514 μs |      17.00 |                      79.84 |                   5169545.05 |
| STJRefGen_Deser  | 15.894 μs | 0.3164 μs | 0.8167 μs |     64,768 | 15.489 μs |      78.00 |                      79.38 |                   5141165.57 |
| Utf8Json_Deser   | 17.743 μs | 0.1768 μs | 0.1653 μs |     56,505 | 17.695 μs |      15.00 |                      93.04 |                   5256990.43 |
| Newtonsoft_Deser | 28.376 μs | 0.2712 μs | 0.2537 μs |     35,471 | 28.270 μs |      15.00 |                     136.88 |                   4855258.31 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  6.122 μs | 0.1196 μs | 0.1753 μs |    180,704 |  6.156 μs |      29.00 |                      25.00 |                   4517340.87 |
| STJRefGen_Ser    |  7.708 μs | 0.1013 μs | 0.0947 μs |    129,632 |  7.678 μs |      15.00 |                      41.74 |                   5411207.42 |
| SpanJson_Ser     |  8.068 μs | 0.0649 μs | 0.0607 μs |    125,533 |  8.049 μs |      15.00 |                      43.13 |                   5413978.82 |
| Utf8Json_Ser     | 11.002 μs | 0.2129 μs | 0.2769 μs |     91,969 | 10.903 μs |      24.00 |                      63.44 |                   5834764.82 |
| Newtonsoft_Ser   | 16.563 μs | 0.2189 μs | 0.2047 μs |     60,485 | 16.455 μs |      15.00 |                      89.37 |                   5405464.63 |
