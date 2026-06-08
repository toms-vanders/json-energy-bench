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
| SpanJson_Deser   | 11.882 μs | 0.2912 μs | 0.8587 μs |     86,041 | 11.679 μs |     100.00 |                      66.95 |                   5760118.56 |
| STJSrcGen_Deser  | 15.329 μs | 0.3599 μs | 1.0611 μs |     63,456 | 15.082 μs |     100.00 |                      92.22 |                   5851879.67 |
| STJRefGen_Deser  | 15.382 μs | 0.3074 μs | 0.8671 μs |     60,688 | 15.127 μs |      92.00 |                      92.70 |                   5626042.44 |
| Utf8Json_Deser   | 17.012 μs | 0.3824 μs | 1.1276 μs |     50,156 | 16.716 μs |     100.00 |                     102.48 |                   5139854.63 |
| Newtonsoft_Deser | 31.633 μs | 0.2797 μs | 0.2616 μs |     35,298 | 31.697 μs |      15.00 |                     174.98 |                   6176446.76 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  5.594 μs | 0.1541 μs | 0.4543 μs |    179,872 |  5.522 μs |     100.00 |                      31.42 |                   5650819.57 |
| STJRefGen_Ser    |  8.010 μs | 0.1840 μs | 0.5425 μs |    115,472 |  7.836 μs |     100.00 |                      43.53 |                   5027052.54 |
| SpanJson_Ser     |  8.558 μs | 0.1686 μs | 0.3288 μs |    109,150 |  8.496 μs |      47.00 |                      49.56 |                   5409942.21 |
| Utf8Json_Ser     | 10.857 μs | 0.2299 μs | 0.6777 μs |     84,611 | 10.667 μs |     100.00 |                      63.66 |                   5386319.12 |
| Newtonsoft_Ser   | 16.180 μs | 0.3217 μs | 0.5883 μs |     57,391 | 16.081 μs |      42.00 |                      95.00 |                   5452338.50 |
