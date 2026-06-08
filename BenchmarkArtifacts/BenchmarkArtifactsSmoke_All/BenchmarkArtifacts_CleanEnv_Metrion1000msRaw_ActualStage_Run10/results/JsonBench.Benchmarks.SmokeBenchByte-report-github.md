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
| SpanJson_Deser   |  7.387 μs | 0.0815 μs | 0.0762 μs |    136,957 |      15.00 |                      53.29 |                   7298741.43 |
| STJSrcGen_Deser  |  9.823 μs | 0.1197 μs | 0.1119 μs |    102,560 |      15.00 |                     126.23 |                  12946087.30 |
| STJRefGen_Deser  | 10.032 μs | 0.1267 μs | 0.1186 μs |    100,384 |      15.00 |                     133.63 |                  13413987.26 |
| Utf8Json_Deser   | 10.678 μs | 0.1116 μs | 0.1044 μs |     94,509 |      15.00 |                     105.77 |                   9996647.34 |
| Newtonsoft_Deser | 18.624 μs | 0.2202 μs | 0.2059 μs |     54,272 |      15.00 |                     152.63 |                   8283312.49 |
|                  |           |           |           |            |            |                            |                              |
| STJSrcGen_Ser    |  3.632 μs | 0.0625 μs | 0.0584 μs |    269,200 |      15.00 |                      25.48 |                   6859173.97 |
| STJRefGen_Ser    |  4.756 μs | 0.0571 μs | 0.0534 μs |    212,160 |      15.00 |                      39.45 |                   8368780.78 |
| Utf8Json_Ser     |  6.735 μs | 0.0996 μs | 0.0932 μs |    147,621 |      15.00 |                      92.12 |                  13599071.51 |
| SpanJson_Ser     |  7.004 μs | 0.0887 μs | 0.0830 μs |    144,193 |      15.00 |                      82.70 |                  11925340.68 |
| Newtonsoft_Ser   | 12.122 μs | 0.1774 μs | 0.1659 μs |     83,696 |      15.00 |                     136.94 |                  11460967.19 |
