```

BenchmarkDotNet v0.15.5-develop (2026-05-17), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 4.50GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   |  8.522 μs | 0.2516 μs | 0.7419 μs |    106,205 |  8.324 μs |      100.0 |                      75.84 |                   8054541.41 |
| STJSrcGen_Deser  | 11.402 μs | 0.3432 μs | 1.0120 μs |     74,912 | 11.114 μs |      100.0 |                     100.14 |                   7501416.00 |
| STJRefGen_Deser  | 11.957 μs | 0.4271 μs | 1.2594 μs |     67,888 | 11.526 μs |      100.0 |                     104.87 |                   7119628.38 |
| Utf8Json_Deser   | 12.264 μs | 0.4097 μs | 1.2079 μs |     60,108 | 11.816 μs |      100.0 |                     106.11 |                   6378101.69 |
| Newtonsoft_Deser | 23.211 μs | 0.7381 μs | 2.1762 μs |     35,552 | 22.498 μs |      100.0 |                     204.96 |                   7286884.61 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  4.122 μs | 0.1489 μs | 0.4391 μs |    210,512 |  3.953 μs |      100.0 |                      36.23 |                   7627163.13 |
| STJRefGen_Ser    |  5.864 μs | 0.1967 μs | 0.5801 μs |    143,200 |  5.633 μs |      100.0 |                      51.84 |                   7423395.55 |
| SpanJson_Ser     |  6.602 μs | 0.1610 μs | 0.4748 μs |    134,517 |  6.425 μs |      100.0 |                      57.16 |                   7689287.58 |
| Utf8Json_Ser     |  7.795 μs | 0.1857 μs | 0.5474 μs |    134,921 |  7.626 μs |      100.0 |                      64.43 |                   8692332.57 |
| Newtonsoft_Ser   | 12.962 μs | 0.3550 μs | 1.0468 μs |     68,896 | 12.647 μs |      100.0 |                     111.59 |                   7687764.66 |
