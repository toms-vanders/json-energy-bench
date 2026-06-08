```

BenchmarkDotNet v0.15.5-develop (2026-05-26), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Coffee Lake), 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.108
  [Host] : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  Affinity=00000100  
IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   |  7.351 μs | 0.1302 μs | 0.1218 μs |    137,519 |      15.00 |                      65.01 |                   8940278.70 |
| STJSrcGen_Deser  |  9.935 μs | 0.1812 μs | 0.1695 μs |    101,696 |      15.00 |                     112.38 |                  11428544.07 |
| STJRefGen_Deser  | 10.170 μs | 0.1296 μs | 0.1212 μs |     99,232 |      15.00 |                      80.78 |                   8015746.85 |
| Utf8Json_Deser   | 12.675 μs | 0.1301 μs | 0.1217 μs |     79,487 |      15.00 |                     163.82 |                  13021667.30 |
| Newtonsoft_Deser | 20.093 μs | 0.2355 μs | 0.2203 μs |     50,384 |      15.00 |                     192.04 |                   9675687.89 |
|                  |           |           |           |            |            |                            |                              |
| STJSrcGen_Ser    |  3.604 μs | 0.0457 μs | 0.0428 μs |    279,744 |      15.00 |                      44.20 |                  12365823.90 |
| STJRefGen_Ser    |  4.724 μs | 0.0505 μs | 0.0473 μs |    213,552 |      15.00 |                      49.12 |                  10489915.49 |
| SpanJson_Ser     |  6.299 μs | 0.0967 μs | 0.0904 μs |    161,268 |      15.00 |                      89.86 |                  14491174.77 |
| Utf8Json_Ser     |  6.728 μs | 0.0833 μs | 0.0779 μs |    146,746 |      15.00 |                     114.97 |                  16871192.09 |
| Newtonsoft_Ser   | 12.034 μs | 0.1757 μs | 0.1644 μs |     84,336 |      15.00 |                     156.22 |                  13175045.82 |
