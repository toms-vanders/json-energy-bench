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
| SpanJson_Deser   |  8.473 μs | 0.2383 μs | 0.7026 μs |    100,442 |  8.266 μs |      100.0 |                      75.80 |                   7613867.48 |
| STJRefGen_Deser  | 11.384 μs | 0.3358 μs | 0.9901 μs |     73,408 | 10.968 μs |      100.0 |                     100.59 |                   7384267.37 |
| STJSrcGen_Deser  | 11.496 μs | 0.3141 μs | 0.9262 μs |     74,464 | 11.172 μs |      100.0 |                      99.95 |                   7442724.91 |
| Utf8Json_Deser   | 12.044 μs | 0.3223 μs | 0.9504 μs |     71,271 | 11.806 μs |      100.0 |                     107.53 |                   7664033.16 |
| Newtonsoft_Deser | 21.082 μs | 0.5951 μs | 1.7547 μs |     41,840 | 20.664 μs |      100.0 |                     183.98 |                   7697741.94 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  4.158 μs | 0.1364 μs | 0.4023 μs |    204,704 |  4.029 μs |      100.0 |                      36.79 |                   7531493.02 |
| STJRefGen_Ser    |  5.851 μs | 0.2242 μs | 0.6611 μs |    126,304 |  5.603 μs |      100.0 |                      51.06 |                   6448918.07 |
| SpanJson_Ser     |  6.332 μs | 0.1591 μs | 0.4690 μs |    135,242 |  6.195 μs |      100.0 |                      55.78 |                   7544315.56 |
| Utf8Json_Ser     |  7.802 μs | 0.2324 μs | 0.6854 μs |    110,520 |  7.559 μs |      100.0 |                      67.93 |                   7507730.81 |
| Newtonsoft_Ser   | 13.012 μs | 0.3996 μs | 1.1782 μs |     68,144 | 12.554 μs |      100.0 |                     112.02 |                   7633326.04 |
