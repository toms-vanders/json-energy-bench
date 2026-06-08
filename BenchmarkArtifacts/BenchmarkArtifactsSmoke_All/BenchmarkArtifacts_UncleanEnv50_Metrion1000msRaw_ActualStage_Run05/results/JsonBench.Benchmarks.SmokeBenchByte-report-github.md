```

BenchmarkDotNet v0.15.5-develop (2026-05-17), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 0.80GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   |  8.431 μs | 0.2063 μs | 0.6082 μs |    105,699 |  8.221 μs |      100.0 |                      74.20 |                   7843119.33 |
| STJSrcGen_Deser  | 11.312 μs | 0.3339 μs | 0.9845 μs |     76,032 | 10.953 μs |      100.0 |                     100.63 |                   7651233.90 |
| STJRefGen_Deser  | 11.615 μs | 0.3601 μs | 1.0618 μs |     72,480 | 11.347 μs |      100.0 |                     101.26 |                   7339276.76 |
| Utf8Json_Deser   | 12.332 μs | 0.3521 μs | 1.0381 μs |     73,889 | 12.014 μs |      100.0 |                     100.39 |                   7417650.94 |
| Newtonsoft_Deser | 23.058 μs | 0.7128 μs | 2.1017 μs |     40,240 | 22.257 μs |      100.0 |                     201.05 |                   8090350.67 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  4.347 μs | 0.1529 μs | 0.4508 μs |    203,440 |  4.229 μs |      100.0 |                      32.34 |                   6579287.71 |
| STJRefGen_Ser    |  6.157 μs | 0.1871 μs | 0.5518 μs |    137,568 |  5.964 μs |      100.0 |                      50.23 |                   6910489.21 |
| SpanJson_Ser     |  6.803 μs | 0.1966 μs | 0.5797 μs |    128,953 |  6.700 μs |      100.0 |                      54.15 |                   6982578.10 |
| Utf8Json_Ser     |  8.210 μs | 0.2252 μs | 0.6639 μs |    107,752 |  8.053 μs |      100.0 |                      67.83 |                   7309264.37 |
| Newtonsoft_Ser   | 13.370 μs | 0.3769 μs | 1.1114 μs |     63,904 | 13.064 μs |      100.0 |                     108.01 |                   6902008.27 |
