```

BenchmarkDotNet v0.15.5-develop (2026-05-16), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 0.80GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   |  5.743 μs | 0.1147 μs | 0.2899 μs |    147,168 |  5.581 μs |      75.00 |                     139.36 |                  20508625.65 |
| STJSrcGen_Deser  |  7.537 μs | 0.1501 μs | 0.4007 μs |    106,736 |  7.337 μs |      83.00 |                     184.32 |                  19673932.65 |
| STJRefGen_Deser  |  7.805 μs | 0.1603 μs | 0.4727 μs |    102,928 |  7.569 μs |     100.00 |                     192.32 |                  19795185.64 |
| Utf8Json_Deser   |  9.707 μs | 0.1941 μs | 0.4651 μs |     90,359 |  9.447 μs |      68.00 |                     237.51 |                  21461544.97 |
| Newtonsoft_Deser | 14.520 μs | 0.2884 μs | 0.7599 μs |     59,216 | 14.112 μs |      81.00 |                     351.52 |                  20815670.84 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.821 μs | 0.0561 μs | 0.1506 μs |    295,328 |  2.746 μs |      84.00 |                      70.82 |                  20914928.61 |
| STJRefGen_Ser    |  4.110 μs | 0.0817 μs | 0.2166 μs |    204,576 |  3.992 μs |      82.00 |                     101.52 |                  20767632.27 |
| SpanJson_Ser     |  4.488 μs | 0.0883 μs | 0.1881 μs |    202,944 |  4.386 μs |      55.00 |                     106.25 |                  21562618.59 |
| Utf8Json_Ser     |  5.324 μs | 0.1060 μs | 0.2773 μs |    166,032 |  5.192 μs |      80.00 |                     135.06 |                  22424824.13 |
| Newtonsoft_Ser   |  9.247 μs | 0.1849 μs | 0.4966 μs |     93,280 |  8.985 μs |      84.00 |                     227.45 |                  21216232.54 |
