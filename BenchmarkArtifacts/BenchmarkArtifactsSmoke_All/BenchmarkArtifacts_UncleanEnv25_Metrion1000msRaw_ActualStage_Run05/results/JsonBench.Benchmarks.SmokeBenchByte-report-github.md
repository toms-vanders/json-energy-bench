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
| SpanJson_Deser   |  6.407 μs | 0.2002 μs | 0.5902 μs |    122,046 |  6.401 μs |      100.0 |                      93.72 |                  11438731.93 |
| STJSrcGen_Deser  |  8.349 μs | 0.2299 μs | 0.6779 μs |    128,496 |  8.263 μs |      100.0 |                     122.72 |                  15768684.67 |
| STJRefGen_Deser  |  8.499 μs | 0.2114 μs | 0.6234 μs |    129,040 |  8.392 μs |      100.0 |                     124.32 |                  16042853.78 |
| Utf8Json_Deser   | 10.371 μs | 0.3013 μs | 0.8885 μs |     78,057 |  9.991 μs |      100.0 |                     149.82 |                  11694574.27 |
| Newtonsoft_Deser | 16.985 μs | 0.4966 μs | 1.4642 μs |     46,224 | 16.527 μs |      100.0 |                     247.13 |                  11423522.05 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  3.066 μs | 0.1004 μs | 0.2960 μs |    255,392 |  2.963 μs |      100.0 |                      44.90 |                  11467425.34 |
| STJRefGen_Ser    |  4.310 μs | 0.1379 μs | 0.4065 μs |    171,120 |  4.149 μs |      100.0 |                      62.53 |                  10700888.73 |
| SpanJson_Ser     |  4.937 μs | 0.1274 μs | 0.3758 μs |    175,728 |  4.859 μs |      100.0 |                      72.34 |                  12711643.59 |
| Utf8Json_Ser     |  5.832 μs | 0.1707 μs | 0.5033 μs |    143,345 |  5.624 μs |      100.0 |                      84.93 |                  12174633.58 |
| Newtonsoft_Ser   |  9.947 μs | 0.2607 μs | 0.7685 μs |     80,640 |  9.700 μs |      100.0 |                     146.78 |                  11836029.62 |
