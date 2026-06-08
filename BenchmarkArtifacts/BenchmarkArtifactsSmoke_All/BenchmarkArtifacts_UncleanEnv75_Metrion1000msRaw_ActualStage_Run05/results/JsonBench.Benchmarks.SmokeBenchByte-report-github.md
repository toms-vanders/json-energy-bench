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
| SpanJson_Deser   | 11.680 μs | 0.2792 μs | 0.8232 μs |     86,365 | 11.342 μs |     100.00 |                      60.47 |                   5222725.19 |
| STJSrcGen_Deser  | 15.202 μs | 0.3448 μs | 1.0168 μs |     64,256 | 14.860 μs |     100.00 |                      90.31 |                   5802733.74 |
| STJRefGen_Deser  | 16.432 μs | 0.4103 μs | 1.2098 μs |     63,216 | 17.222 μs |     100.00 |                      79.71 |                   5039190.37 |
| Utf8Json_Deser   | 18.499 μs | 0.5436 μs | 1.6028 μs |     55,990 | 19.305 μs |     100.00 |                     100.73 |                   5640061.96 |
| Newtonsoft_Deser | 30.319 μs | 0.6048 μs | 1.3019 μs |     31,737 | 30.170 μs |      56.00 |                     178.34 |                   5660065.48 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  5.625 μs | 0.1363 μs | 0.4019 μs |    181,296 |  5.534 μs |     100.00 |                      32.98 |                   5979389.55 |
| STJRefGen_Ser    |  7.811 μs | 0.1670 μs | 0.4924 μs |    121,024 |  7.733 μs |     100.00 |                      45.58 |                   5516827.56 |
| SpanJson_Ser     |  8.081 μs | 0.1826 μs | 0.5383 μs |    117,258 |  7.877 μs |     100.00 |                      48.77 |                   5718395.93 |
| Utf8Json_Ser     | 11.694 μs | 0.2312 μs | 0.2374 μs |     95,052 | 11.735 μs |      17.00 |                      54.74 |                   5203269.00 |
| Newtonsoft_Ser   | 16.492 μs | 0.3949 μs | 1.1644 μs |     57,112 | 16.139 μs |     100.00 |                      96.60 |                   5516864.00 |
