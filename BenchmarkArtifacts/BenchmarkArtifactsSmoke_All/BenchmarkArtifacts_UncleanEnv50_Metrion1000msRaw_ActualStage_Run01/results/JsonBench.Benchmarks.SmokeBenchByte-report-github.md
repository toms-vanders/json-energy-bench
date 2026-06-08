```

BenchmarkDotNet v0.15.5-develop (2026-05-17), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 0.80GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   |  8.544 μs | 0.2368 μs | 0.6983 μs |    102,105 |  8.384 μs |      100.0 |                      74.85 |                   7642180.17 |
| STJRefGen_Deser  | 11.405 μs | 0.3113 μs | 0.9178 μs |     76,208 | 11.107 μs |      100.0 |                      98.18 |                   7481979.74 |
| STJSrcGen_Deser  | 11.516 μs | 0.3585 μs | 1.0570 μs |     76,976 | 11.179 μs |      100.0 |                      97.59 |                   7511777.11 |
| Utf8Json_Deser   | 12.751 μs | 0.3016 μs | 0.8893 μs |     69,074 | 12.486 μs |      100.0 |                     110.58 |                   7638040.67 |
| Newtonsoft_Deser | 22.817 μs | 0.5682 μs | 1.6753 μs |     36,880 | 22.229 μs |      100.0 |                     202.09 |                   7452906.59 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  4.120 μs | 0.1213 μs | 0.3577 μs |    210,976 |  4.053 μs |      100.0 |                      35.49 |                   7487576.67 |
| STJRefGen_Ser    |  5.901 μs | 0.1597 μs | 0.4709 μs |    147,920 |  5.792 μs |      100.0 |                      53.60 |                   7928444.53 |
| SpanJson_Ser     |  6.431 μs | 0.1680 μs | 0.4953 μs |    145,256 |  6.347 μs |      100.0 |                      54.66 |                   7939123.59 |
| Utf8Json_Ser     |  7.722 μs | 0.2402 μs | 0.7083 μs |    111,557 |  7.510 μs |      100.0 |                      67.81 |                   7565210.23 |
| Newtonsoft_Ser   | 13.215 μs | 0.4070 μs | 1.1999 μs |     67,152 | 12.803 μs |      100.0 |                     113.87 |                   7646767.64 |
