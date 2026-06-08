```

BenchmarkDotNet v0.15.5-develop (2026-05-26), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 0.80GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.108
  [Host] : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   |  5.707 μs | 0.1136 μs | 0.2699 μs |    180,416 |  5.548 μs |      67.00 |                     146.27 |                  26389693.63 |
| STJSrcGen_Deser  |  7.625 μs | 0.1513 μs | 0.3384 μs |    134,832 |  7.432 μs |      60.00 |                     199.21 |                  26859708.15 |
| STJRefGen_Deser  |  7.696 μs | 0.1526 μs | 0.4229 μs |    109,680 |  7.469 μs |      89.00 |                     201.17 |                  22064504.94 |
| Utf8Json_Deser   |  8.175 μs | 0.1621 μs | 0.4355 μs |    106,960 |  7.940 μs |      84.00 |                     220.97 |                  23634816.97 |
| Newtonsoft_Deser | 15.202 μs | 0.3124 μs | 0.9211 μs |     56,816 | 14.712 μs |     100.00 |                     382.69 |                  21742868.28 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.810 μs | 0.0561 μs | 0.1408 μs |    363,328 |  2.728 μs |      74.00 |                      76.01 |                  27616127.49 |
| STJRefGen_Ser    |  3.914 μs | 0.0774 μs | 0.1928 μs |    261,712 |  3.804 μs |      73.00 |                     104.06 |                  27233061.18 |
| SpanJson_Ser     |  4.082 μs | 0.0815 μs | 0.1953 μs |    221,216 |  3.990 μs |      68.00 |                     105.96 |                  23439917.57 |
| Utf8Json_Ser     |  5.292 μs | 0.1043 μs | 0.2108 μs |    193,072 |  5.185 μs |      50.00 |                     132.74 |                  25628025.94 |
| Newtonsoft_Ser   |  9.189 μs | 0.1838 μs | 0.5213 μs |     93,520 |  8.904 μs |      93.00 |                     234.90 |                  21967542.59 |
