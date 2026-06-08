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
| SpanJson_Deser   |  5.838 μs | 0.1162 μs | 0.2916 μs |    138,784 |  5.686 μs |      74.00 |                     167.21 |                  23206691.05 |
| STJSrcGen_Deser  |  7.849 μs | 0.1567 μs | 0.4596 μs |     98,896 |  7.630 μs |      99.00 |                     236.67 |                  23405807.09 |
| STJRefGen_Deser  |  7.904 μs | 0.1574 μs | 0.3890 μs |    107,616 |  7.680 μs |      72.00 |                     197.18 |                  21219246.40 |
| Utf8Json_Deser   |  8.966 μs | 0.1783 μs | 0.4339 μs |     95,638 |  8.727 μs |      70.00 |                     253.87 |                  24279888.45 |
| Newtonsoft_Deser | 14.806 μs | 0.2942 μs | 0.8395 μs |     58,848 | 14.382 μs |      94.00 |                     396.96 |                  23360563.66 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.863 μs | 0.0573 μs | 0.1568 μs |    284,400 |  2.787 μs |      87.00 |                      93.83 |                  26686255.04 |
| STJRefGen_Ser    |  4.109 μs | 0.0822 μs | 0.2166 μs |    206,256 |  4.007 μs |      81.00 |                     116.41 |                  24011090.68 |
| SpanJson_Ser     |  4.470 μs | 0.0890 μs | 0.1714 μs |    205,136 |  4.397 μs |      46.00 |                     111.80 |                  22934946.87 |
| Utf8Json_Ser     |  5.423 μs | 0.1078 μs | 0.2704 μs |    162,646 |  5.290 μs |      74.00 |                     162.02 |                  26351521.49 |
| Newtonsoft_Ser   |  9.434 μs | 0.1885 μs | 0.5530 μs |     91,472 |  9.150 μs |      99.00 |                     312.67 |                  28600164.56 |
