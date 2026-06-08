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
| SpanJson_Deser   |  5.703 μs | 0.1135 μs | 0.2806 μs |    173,680 |  5.546 μs |      72.00 |                     151.07 |                  26237975.19 |
| STJRefGen_Deser  |  7.753 μs | 0.1536 μs | 0.3739 μs |    132,864 |  7.547 μs |      70.00 |                     200.19 |                  26597466.24 |
| STJSrcGen_Deser  |  7.857 μs | 0.1565 μs | 0.3838 μs |    132,800 |  7.618 μs |      71.00 |                     204.03 |                  27095085.44 |
| Utf8Json_Deser   |  9.043 μs | 0.1807 μs | 0.4567 μs |     96,464 |  8.796 μs |      75.00 |                     237.31 |                  22891835.94 |
| Newtonsoft_Deser | 15.443 μs | 0.3083 μs | 0.8283 μs |     55,376 | 15.052 μs |      84.00 |                     408.84 |                  22639706.54 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.826 μs | 0.0565 μs | 0.1321 μs |    367,648 |  2.756 μs |      65.00 |                      73.92 |                  27175356.10 |
| STJRefGen_Ser    |  3.995 μs | 0.0794 μs | 0.1856 μs |    254,816 |  3.899 μs |      65.00 |                     107.63 |                  27425590.52 |
| SpanJson_Ser     |  4.268 μs | 0.0850 μs | 0.2133 μs |    210,880 |  4.181 μs |      74.00 |                     107.90 |                  22753112.05 |
| Utf8Json_Ser     |  5.258 μs | 0.1041 μs | 0.2196 μs |    193,696 |  5.138 μs |      54.00 |                     130.92 |                  25358257.49 |
| Newtonsoft_Ser   |  9.188 μs | 0.1825 μs | 0.4904 μs |    110,512 |  8.907 μs |      84.00 |                     236.14 |                  26096502.96 |
