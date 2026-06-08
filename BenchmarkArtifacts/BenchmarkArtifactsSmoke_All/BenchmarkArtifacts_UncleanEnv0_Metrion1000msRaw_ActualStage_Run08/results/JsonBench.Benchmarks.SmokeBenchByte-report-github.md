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
| SpanJson_Deser   |  5.675 μs | 0.1123 μs | 0.2625 μs |    181,600 |  5.517 μs |      65.00 |                     146.31 |                  26570541.91 |
| STJSrcGen_Deser  |  7.601 μs | 0.1509 μs | 0.3586 μs |    135,072 |  7.406 μs |      67.00 |                     203.61 |                  27501460.09 |
| STJRefGen_Deser  |  7.718 μs | 0.1527 μs | 0.3716 μs |    132,848 |  7.527 μs |      70.00 |                     208.54 |                  27704444.23 |
| Utf8Json_Deser   | 11.839 μs | 0.2360 μs | 0.4873 μs |     85,347 | 11.710 μs |      52.00 |                     290.94 |                  24830450.63 |
| Newtonsoft_Deser | 15.496 μs | 0.3098 μs | 0.8323 μs |     55,520 | 15.059 μs |      84.00 |                     402.43 |                  22342803.95 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.832 μs | 0.0564 μs | 0.1372 μs |    362,992 |  2.766 μs |      70.00 |                      76.77 |                  27866857.44 |
| STJRefGen_Ser    |  4.023 μs | 0.0801 μs | 0.1934 μs |    256,176 |  3.917 μs |      69.00 |                     109.52 |                  28057427.24 |
| SpanJson_Ser     |  4.452 μs | 0.0880 μs | 0.1817 μs |    201,664 |  4.373 μs |      52.00 |                     107.49 |                  21677002.94 |
| Utf8Json_Ser     |  5.300 μs | 0.1055 μs | 0.2685 μs |    162,800 |  5.164 μs |      76.00 |                     130.91 |                  21311791.83 |
| Newtonsoft_Ser   |  9.135 μs | 0.1847 μs | 0.5447 μs |     95,216 |  8.853 μs |     100.00 |                     231.44 |                  22036569.01 |
