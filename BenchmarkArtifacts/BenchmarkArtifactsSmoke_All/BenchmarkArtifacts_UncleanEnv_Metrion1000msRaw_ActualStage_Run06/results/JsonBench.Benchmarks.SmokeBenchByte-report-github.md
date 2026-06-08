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
| SpanJson_Deser   |  5.717 μs | 0.1135 μs | 0.2763 μs |    180,912 |  5.568 μs |      70.00 |                     140.31 |                  25383834.43 |
| STJSrcGen_Deser  |  7.824 μs | 0.1555 μs | 0.3664 μs |    135,072 |  7.619 μs |      66.00 |                     197.95 |                  26738120.59 |
| STJRefGen_Deser  |  7.859 μs | 0.1565 μs | 0.3955 μs |    131,104 |  7.659 μs |      75.00 |                     192.59 |                  25248765.31 |
| Utf8Json_Deser   |  8.846 μs | 0.1755 μs | 0.4592 μs |     99,216 |  8.622 μs |      80.00 |                     214.32 |                  21263990.06 |
| Newtonsoft_Deser | 14.271 μs | 0.2840 μs | 0.7330 μs |     59,648 | 13.914 μs |      78.00 |                     357.56 |                  21327870.30 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.809 μs | 0.0561 μs | 0.1457 μs |    370,080 |  2.729 μs |      79.00 |                      71.76 |                  26556079.37 |
| STJRefGen_Ser    |  4.049 μs | 0.0805 μs | 0.2203 μs |    201,120 |  3.948 μs |      87.00 |                     101.83 |                  20480941.52 |
| SpanJson_Ser     |  4.479 μs | 0.0885 μs | 0.1961 μs |    203,008 |  4.389 μs |      59.00 |                     110.00 |                  22330374.81 |
| Utf8Json_Ser     |  5.313 μs | 0.1056 μs | 0.2727 μs |    163,984 |  5.182 μs |      78.00 |                     129.18 |                  21183328.45 |
| Newtonsoft_Ser   |  9.337 μs | 0.1903 μs | 0.5610 μs |     92,944 |  9.076 μs |     100.00 |                     230.04 |                  21380741.80 |
