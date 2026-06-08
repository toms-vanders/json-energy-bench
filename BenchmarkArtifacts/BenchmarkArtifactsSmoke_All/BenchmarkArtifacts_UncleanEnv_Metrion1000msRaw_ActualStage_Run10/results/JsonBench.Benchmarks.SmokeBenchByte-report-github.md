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
| SpanJson_Deser   |  5.776 μs | 0.1156 μs | 0.3202 μs |    146,080 |  5.604 μs |      89.00 |                     140.83 |                  20572343.24 |
| STJSrcGen_Deser  |  7.595 μs | 0.1516 μs | 0.3775 μs |    135,568 |  7.401 μs |      73.00 |                     183.27 |                  24845312.17 |
| STJRefGen_Deser  |  7.745 μs | 0.1541 μs | 0.4371 μs |    103,856 |  7.529 μs |      93.00 |                     185.67 |                  19283046.30 |
| Utf8Json_Deser   |  8.870 μs | 0.1771 μs | 0.4310 μs |     98,752 |  8.665 μs |      70.00 |                     217.54 |                  21482364.72 |
| Newtonsoft_Deser | 14.507 μs | 0.2889 μs | 0.8426 μs |     60,048 | 14.087 μs |      98.00 |                     348.59 |                  20931971.48 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.829 μs | 0.0566 μs | 0.1625 μs |    295,248 |  2.741 μs |      95.00 |                      69.43 |                  20498119.74 |
| STJRefGen_Ser    |  3.839 μs | 0.0763 μs | 0.2102 μs |    219,744 |  3.721 μs |      88.00 |                      97.48 |                  21420218.54 |
| SpanJson_Ser     |  4.818 μs | 0.0952 μs | 0.1987 μs |    186,128 |  4.727 μs |      53.00 |                     116.15 |                  21619189.96 |
| Utf8Json_Ser     |  5.306 μs | 0.1052 μs | 0.2520 μs |    161,264 |  5.183 μs |      68.00 |                     127.43 |                  20549215.71 |
| Newtonsoft_Ser   |  9.246 μs | 0.1836 μs | 0.5412 μs |     92,480 |  8.981 μs |     100.00 |                     231.89 |                  21445530.32 |
