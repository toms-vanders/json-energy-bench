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
| SpanJson_Deser   |  5.684 μs | 0.1123 μs | 0.2647 μs |    181,280 |  5.532 μs |      66.00 |                     147.24 |                  26691251.21 |
| STJSrcGen_Deser  |  7.631 μs | 0.1519 μs | 0.3639 μs |    134,368 |  7.433 μs |      68.00 |                     198.61 |                  26687295.84 |
| STJRefGen_Deser  |  7.764 μs | 0.1545 μs | 0.3671 μs |    133,408 |  7.540 μs |      67.00 |                     203.39 |                  27133464.27 |
| Utf8Json_Deser   |  8.762 μs | 0.1738 μs | 0.4455 μs |     97,360 |  8.528 μs |      77.00 |                     231.15 |                  22505067.83 |
| Newtonsoft_Deser | 15.273 μs | 0.3039 μs | 0.8622 μs |     54,368 | 14.858 μs |      93.00 |                     397.26 |                  21597960.48 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.817 μs | 0.0563 μs | 0.1422 μs |    354,176 |  2.738 μs |      75.00 |                      75.21 |                  26636205.09 |
| STJRefGen_Ser    |  4.134 μs | 0.0824 μs | 0.1861 μs |    246,688 |  4.043 μs |      61.00 |                     110.62 |                  27288154.90 |
| Utf8Json_Ser     |  5.278 μs | 0.1055 μs | 0.2588 μs |    162,448 |  5.152 μs |      71.00 |                     132.24 |                  21481811.55 |
| SpanJson_Ser     |  5.290 μs | 0.1039 μs | 0.1900 μs |    193,120 |  5.182 μs |      42.00 |                     134.64 |                  26002435.11 |
| Newtonsoft_Ser   |  9.211 μs | 0.1924 μs | 0.5672 μs |     93,584 |  8.914 μs |     100.00 |                     240.39 |                  22496448.32 |
