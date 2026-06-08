```

BenchmarkDotNet v0.15.5-develop (2026-05-16), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 0.80GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   |  5.774 μs | 0.1143 μs | 0.3204 μs |    143,888 |  5.615 μs |      91.00 |                     136.64 |                  19660947.63 |
| STJSrcGen_Deser  |  7.715 μs | 0.1598 μs | 0.4711 μs |    109,520 |  7.481 μs |     100.00 |                     186.67 |                  20444048.54 |
| STJRefGen_Deser  |  7.828 μs | 0.1558 μs | 0.3793 μs |    130,704 |  7.613 μs |      70.00 |                     180.61 |                  23606446.15 |
| Utf8Json_Deser   |  8.870 μs | 0.1770 μs | 0.4537 μs |     98,659 |  8.644 μs |      77.00 |                     221.20 |                  21823613.58 |
| Newtonsoft_Deser | 14.601 μs | 0.2928 μs | 0.8632 μs |     59,104 | 14.181 μs |     100.00 |                     354.68 |                  20963051.51 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.787 μs | 0.0554 μs | 0.1555 μs |    298,496 |  2.700 μs |      91.00 |                      70.78 |                  21127246.14 |
| STJRefGen_Ser    |  4.030 μs | 0.0801 μs | 0.2110 μs |    206,912 |  3.919 μs |      81.00 |                     100.10 |                  20711083.86 |
| SpanJson_Ser     |  4.866 μs | 0.0966 μs | 0.2162 μs |    186,176 |  4.768 μs |      60.00 |                     116.32 |                  21655193.06 |
| Utf8Json_Ser     |  5.314 μs | 0.1060 μs | 0.2599 μs |    165,248 |  5.193 μs |      71.00 |                     128.97 |                  21312097.58 |
| Newtonsoft_Ser   |  9.251 μs | 0.1836 μs | 0.5117 μs |     92,880 |  9.016 μs |      90.00 |                     237.35 |                  22044645.64 |
