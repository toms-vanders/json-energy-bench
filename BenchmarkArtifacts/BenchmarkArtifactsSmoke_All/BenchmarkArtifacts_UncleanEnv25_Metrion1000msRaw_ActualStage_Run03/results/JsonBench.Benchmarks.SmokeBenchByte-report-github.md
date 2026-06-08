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
| SpanJson_Deser   |  6.255 μs | 0.1619 μs | 0.4773 μs |    171,747 |  6.216 μs |      100.0 |                      93.21 |                  16008449.45 |
| STJSrcGen_Deser  |  8.249 μs | 0.2324 μs | 0.6854 μs |    130,288 |  7.901 μs |      100.0 |                     121.26 |                  15798533.57 |
| STJRefGen_Deser  |  8.484 μs | 0.2317 μs | 0.6831 μs |    124,672 |  8.342 μs |      100.0 |                     126.47 |                  15766872.06 |
| Utf8Json_Deser   |  9.353 μs | 0.2790 μs | 0.8227 μs |     86,324 |  9.259 μs |      100.0 |                     136.51 |                  11784360.28 |
| Newtonsoft_Deser | 16.743 μs | 0.5016 μs | 1.4791 μs |     47,696 | 16.553 μs |      100.0 |                     246.82 |                  11772172.45 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  3.079 μs | 0.0933 μs | 0.2750 μs |    256,816 |  3.020 μs |      100.0 |                      45.51 |                  11688598.29 |
| STJRefGen_Ser    |  4.376 μs | 0.1244 μs | 0.3668 μs |    170,768 |  4.286 μs |      100.0 |                      63.83 |                  10899968.74 |
| SpanJson_Ser     |  4.724 μs | 0.1284 μs | 0.3786 μs |    188,512 |  4.643 μs |      100.0 |                      67.87 |                  12794639.77 |
| Utf8Json_Ser     |  5.804 μs | 0.1490 μs | 0.4394 μs |    139,525 |  5.847 μs |      100.0 |                      84.80 |                  11832322.57 |
| Newtonsoft_Ser   |  9.841 μs | 0.2540 μs | 0.7490 μs |     78,352 |  9.715 μs |      100.0 |                     142.10 |                  11133619.43 |
