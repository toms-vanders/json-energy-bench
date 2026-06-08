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
| SpanJson_Deser   |  5.842 μs | 0.1160 μs | 0.3036 μs |    140,080 |  5.679 μs |      80.00 |                     154.73 |                  21674762.78 |
| STJSrcGen_Deser  |  7.910 μs | 0.1574 μs | 0.3919 μs |    130,496 |  7.677 μs |      73.00 |                     202.22 |                  26389290.62 |
| STJRefGen_Deser  |  7.955 μs | 0.1583 μs | 0.4057 μs |    129,568 |  7.782 μs |      77.00 |                     185.19 |                  23994261.64 |
| Utf8Json_Deser   |  8.687 μs | 0.1733 μs | 0.4185 μs |    104,574 |  8.443 μs |      69.00 |                     287.70 |                  30085789.45 |
| Newtonsoft_Deser | 14.689 μs | 0.2918 μs | 0.7987 μs |     59,808 | 14.274 μs |      87.00 |                     396.78 |                  23730856.38 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.840 μs | 0.0569 μs | 0.1649 μs |    283,424 |  2.758 μs |      97.00 |                      69.24 |                  19624143.36 |
| STJRefGen_Ser    |  4.163 μs | 0.0832 μs | 0.2265 μs |    189,040 |  4.053 μs |      86.00 |                     107.83 |                  20384948.72 |
| SpanJson_Ser     |  4.545 μs | 0.0904 μs | 0.2286 μs |    200,512 |  4.441 μs |      75.00 |                     119.18 |                  23896475.12 |
| Utf8Json_Ser     |  5.387 μs | 0.1074 μs | 0.2446 μs |    161,520 |  5.275 μs |      62.00 |                     133.59 |                  21576915.09 |
| Newtonsoft_Ser   |  9.451 μs | 0.1879 μs | 0.4949 μs |     90,000 |  9.198 μs |      81.00 |                     234.00 |                  21060033.71 |
