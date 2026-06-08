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
| SpanJson_Deser   |  5.659 μs | 0.1126 μs | 0.2698 μs |    181,872 |  5.500 μs |      68.00 |                     145.72 |                  26502985.99 |
| STJSrcGen_Deser  |  7.660 μs | 0.1516 μs | 0.3603 μs |    135,440 |  7.464 μs |      67.00 |                     206.74 |                  28000439.79 |
| STJRefGen_Deser  |  7.696 μs | 0.1520 μs | 0.3672 μs |    134,464 |  7.490 μs |      69.00 |                     200.97 |                  27023776.94 |
| Utf8Json_Deser   |  8.679 μs | 0.1719 μs | 0.3433 μs |    101,072 |  8.483 μs |      49.00 |                     227.94 |                  23038411.72 |
| Newtonsoft_Deser | 14.448 μs | 0.2880 μs | 0.8171 μs |     59,392 | 14.023 μs |      93.00 |                     370.56 |                  22008447.86 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.782 μs | 0.0555 μs | 0.1382 μs |    366,304 |  2.696 μs |      73.00 |                      74.36 |                  27239024.06 |
| STJRefGen_Ser    |  4.072 μs | 0.0809 μs | 0.1706 μs |    253,424 |  3.971 μs |      54.00 |                     104.00 |                  26356569.15 |
| SpanJson_Ser     |  4.075 μs | 0.0814 μs | 0.2157 μs |    209,408 |  3.969 μs |      82.00 |                     106.60 |                  22323934.42 |
| Utf8Json_Ser     |  5.297 μs | 0.1056 μs | 0.2468 μs |    163,512 |  5.162 μs |      65.00 |                     133.39 |                  21810284.25 |
| Newtonsoft_Ser   |  9.167 μs | 0.1888 μs | 0.5567 μs |     93,904 |  8.849 μs |     100.00 |                     233.48 |                  21924339.84 |
