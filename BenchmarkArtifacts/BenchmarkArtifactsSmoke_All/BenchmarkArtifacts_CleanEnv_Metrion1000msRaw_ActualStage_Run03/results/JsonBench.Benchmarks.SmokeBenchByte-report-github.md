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
| SpanJson_Deser   |  7.358 μs | 0.0855 μs | 0.0800 μs |    137,465 |      15.00 |                      83.76 |                  11513584.09 |
| STJRefGen_Deser  |  9.953 μs | 0.1301 μs | 0.1217 μs |    101,536 |      15.00 |                      82.18 |                   8344519.61 |
| STJSrcGen_Deser  | 10.000 μs | 0.1028 μs | 0.0961 μs |    100,960 |      15.00 |                     129.17 |                  13040927.81 |
| Utf8Json_Deser   | 12.368 μs | 0.2414 μs | 0.2479 μs |     80,916 |      17.00 |                      87.12 |                   7049555.74 |
| Newtonsoft_Deser | 18.560 μs | 0.2360 μs | 0.2208 μs |     54,400 |      15.00 |                     241.48 |                  13136490.33 |
|                  |           |           |           |            |            |                            |                              |
| STJSrcGen_Ser    |  3.742 μs | 0.0613 μs | 0.0573 μs |    269,856 |      15.00 |                      25.73 |                   6944497.00 |
| STJRefGen_Ser    |  4.573 μs | 0.0772 μs | 0.0722 μs |    220,832 |      15.00 |                      63.10 |                  13935209.39 |
| SpanJson_Ser     |  6.190 μs | 0.1129 μs | 0.1056 μs |    162,545 |      15.00 |                      62.19 |                  10108367.75 |
| Utf8Json_Ser     |  6.697 μs | 0.1103 μs | 0.1032 μs |    147,799 |      15.00 |                      69.53 |                  10276463.76 |
| Newtonsoft_Ser   | 12.020 μs | 0.1733 μs | 0.1621 μs |     84,256 |      15.00 |                      81.74 |                   6887160.75 |
