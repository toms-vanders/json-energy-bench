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
| SpanJson_Deser   |  7.367 μs | 0.0826 μs | 0.0773 μs |    137,382 |      15.00 |                      94.24 |                  12946941.18 |
| STJRefGen_Deser  |  9.871 μs | 0.1244 μs | 0.1164 μs |    102,384 |      15.00 |                     124.10 |                  12705359.07 |
| STJSrcGen_Deser  |  9.876 μs | 0.1099 μs | 0.1028 μs |    101,024 |      15.00 |                     125.16 |                  12643910.16 |
| Utf8Json_Deser   | 13.573 μs | 0.1736 μs | 0.1624 μs |     73,859 |      15.00 |                     190.89 |                  14098685.11 |
| Newtonsoft_Deser | 20.394 μs | 0.2521 μs | 0.2358 μs |     49,552 |      15.00 |                     269.80 |                  13369224.99 |
|                  |           |           |           |            |            |                            |                              |
| STJSrcGen_Ser    |  3.498 μs | 0.0418 μs | 0.0391 μs |    288,656 |      15.00 |                      46.48 |                  13416097.62 |
| STJRefGen_Ser    |  4.852 μs | 0.0588 μs | 0.0550 μs |    208,048 |      15.00 |                      54.48 |                  11335449.70 |
| SpanJson_Ser     |  6.403 μs | 0.1246 μs | 0.1280 μs |    159,103 |      17.00 |                      83.59 |                  13299401.09 |
| Utf8Json_Ser     |  6.867 μs | 0.1072 μs | 0.1002 μs |    146,493 |      15.00 |                      85.32 |                  12498789.75 |
| Newtonsoft_Ser   | 12.047 μs | 0.1702 μs | 0.1592 μs |     84,064 |      15.00 |                     171.09 |                  14382812.92 |
