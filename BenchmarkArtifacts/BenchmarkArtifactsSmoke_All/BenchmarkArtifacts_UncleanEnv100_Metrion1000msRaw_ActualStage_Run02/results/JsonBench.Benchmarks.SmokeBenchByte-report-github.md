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
| SpanJson_Deser   | 11.849 μs | 0.0724 μs | 0.0677 μs |     84,334 | 11.849 μs |      15.00 |                      63.94 |                   5392099.55 |
| STJRefGen_Deser  | 15.940 μs | 0.1006 μs | 0.0941 μs |     62,624 | 15.900 μs |      15.00 |                      89.82 |                   5624808.62 |
| STJSrcGen_Deser  | 16.042 μs | 0.3186 μs | 0.7320 μs |     63,648 | 15.646 μs |      63.00 |                      94.81 |                   6034504.28 |
| Utf8Json_Deser   | 22.546 μs | 0.1498 μs | 0.1401 μs |     44,401 | 22.499 μs |      15.00 |                     118.92 |                   5280106.22 |
| Newtonsoft_Deser | 28.830 μs | 0.3011 μs | 0.2816 μs |     34,982 | 28.764 μs |      15.00 |                     166.17 |                   5812888.48 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  5.540 μs | 0.0242 μs | 0.0226 μs |    179,360 |  5.544 μs |      15.00 |                      30.38 |                   5448958.66 |
| STJRefGen_Ser    |  8.519 μs | 0.1691 μs | 0.3218 μs |    128,160 |  8.636 μs |      45.00 |                      48.55 |                   6222462.76 |
| Utf8Json_Ser     | 10.694 μs | 0.0612 μs | 0.0572 μs |     89,169 | 10.683 μs |      15.00 |                      60.32 |                   5379040.89 |
| SpanJson_Ser     | 10.966 μs | 0.2140 μs | 0.2929 μs |     92,084 | 10.862 μs |      26.00 |                      60.13 |                   5537141.41 |
| Newtonsoft_Ser   | 16.659 μs | 0.3241 μs | 0.3183 μs |     59,255 | 16.609 μs |      16.00 |                      89.80 |                   5321101.37 |
