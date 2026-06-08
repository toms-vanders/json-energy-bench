```

BenchmarkDotNet v0.15.5-develop (2026-05-17), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 4.20GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   | 12.075 μs | 0.3053 μs | 0.9003 μs |     86,864 | 12.486 μs |     100.00 |                      67.51 |                   5864319.99 |
| STJSrcGen_Deser  | 15.445 μs | 0.3892 μs | 1.1474 μs |     64,400 | 15.099 μs |     100.00 |                      88.56 |                   5703107.17 |
| STJRefGen_Deser  | 17.419 μs | 0.1507 μs | 0.1410 μs |     57,120 | 17.411 μs |      15.00 |                      84.45 |                   4823674.52 |
| Utf8Json_Deser   | 19.056 μs | 0.4567 μs | 1.3466 μs |     53,709 | 18.614 μs |     100.00 |                      87.64 |                   4706831.13 |
| Newtonsoft_Deser | 30.938 μs | 0.7558 μs | 2.2285 μs |     31,866 | 30.380 μs |     100.00 |                     185.01 |                   5895557.08 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  5.488 μs | 0.1360 μs | 0.4009 μs |    174,768 |  5.396 μs |     100.00 |                      31.62 |                   5526277.51 |
| STJRefGen_Ser    |  8.062 μs | 0.1930 μs | 0.5691 μs |    120,336 |  8.126 μs |     100.00 |                      42.49 |                   5112717.81 |
| SpanJson_Ser     |  8.904 μs | 0.2244 μs | 0.6618 μs |    107,364 |  8.708 μs |     100.00 |                      49.69 |                   5334634.10 |
| Utf8Json_Ser     | 11.616 μs | 0.1951 μs | 0.1825 μs |     97,256 | 11.651 μs |      15.00 |                      63.51 |                   6177104.62 |
| Newtonsoft_Ser   | 18.362 μs | 0.3658 μs | 0.4354 μs |     60,355 | 18.412 μs |      21.00 |                      92.88 |                   5605771.05 |
