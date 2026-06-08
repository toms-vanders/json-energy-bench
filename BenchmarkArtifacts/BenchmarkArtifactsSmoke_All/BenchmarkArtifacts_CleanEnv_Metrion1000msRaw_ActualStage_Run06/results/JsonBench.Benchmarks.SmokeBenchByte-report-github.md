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
| SpanJson_Deser   |  7.451 μs | 0.1476 μs | 0.2466 μs |     44,308 |      36.00 |                     115.31 |                   5109153.45 |
| STJSrcGen_Deser  |  9.957 μs | 0.1024 μs | 0.0958 μs |    100,576 |      15.00 |                      81.36 |                   8182595.34 |
| STJRefGen_Deser  | 10.160 μs | 0.1067 μs | 0.0998 μs |     99,472 |      15.00 |                     142.11 |                  14135926.96 |
| Utf8Json_Deser   | 11.464 μs | 0.1151 μs | 0.1077 μs |     87,815 |      15.00 |                     130.76 |                  11482348.23 |
| Newtonsoft_Deser | 20.198 μs | 0.2494 μs | 0.2332 μs |     50,080 |      15.00 |                     235.34 |                  11785776.02 |
|                  |           |           |           |            |            |                            |                              |
| STJSrcGen_Ser    |  3.637 μs | 0.0466 μs | 0.0436 μs |    276,800 |      15.00 |                      42.75 |                  11833392.13 |
| STJRefGen_Ser    |  4.786 μs | 0.0592 μs | 0.0554 μs |    210,336 |      15.00 |                      40.59 |                   8537807.99 |
| Utf8Json_Ser     |  6.737 μs | 0.1098 μs | 0.1027 μs |    146,894 |      15.00 |                      99.07 |                  14552163.84 |
| SpanJson_Ser     |  7.156 μs | 0.0962 μs | 0.0900 μs |    140,989 |      15.00 |                      97.58 |                  13758090.34 |
| Newtonsoft_Ser   | 11.965 μs | 0.1795 μs | 0.1679 μs |     85,152 |      15.00 |                     152.29 |                  12968043.07 |
