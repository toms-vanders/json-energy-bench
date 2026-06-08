```

BenchmarkDotNet v0.15.5-develop (2026-05-17), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 4.30GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   | 11.717 μs | 0.2800 μs | 0.8255 μs |     86,790 | 11.409 μs |     100.00 |                      62.67 |                   5439213.22 |
| STJSrcGen_Deser  | 15.269 μs | 0.4015 μs | 1.1838 μs |     58,096 | 14.791 μs |     100.00 |                      81.79 |                   4751753.32 |
| STJRefGen_Deser  | 17.087 μs | 0.3331 μs | 0.4670 μs |     57,536 | 17.164 μs |      27.00 |                      83.59 |                   4809286.07 |
| Utf8Json_Deser   | 17.964 μs | 0.5208 μs | 1.5356 μs |     56,252 | 17.515 μs |     100.00 |                      94.14 |                   5295708.08 |
| Newtonsoft_Deser | 31.774 μs | 0.7492 μs | 2.2091 μs |     32,246 | 31.854 μs |     100.00 |                     175.93 |                   5673077.26 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  5.654 μs | 0.1483 μs | 0.4372 μs |    180,864 |  5.552 μs |     100.00 |                      33.40 |                   6040984.18 |
| STJRefGen_Ser    |  7.858 μs | 0.2054 μs | 0.6058 μs |    122,608 |  7.639 μs |     100.00 |                      44.99 |                   5515565.44 |
| SpanJson_Ser     |  9.298 μs | 0.2396 μs | 0.7064 μs |    105,177 |  9.788 μs |     100.00 |                      51.75 |                   5443352.37 |
| Utf8Json_Ser     | 10.818 μs | 0.2601 μs | 0.7670 μs |     94,644 | 10.761 μs |     100.00 |                      56.28 |                   5326451.15 |
| Newtonsoft_Ser   | 18.342 μs | 0.3561 μs | 0.4239 μs |     53,987 | 18.362 μs |      21.00 |                      97.58 |                   5268169.52 |
