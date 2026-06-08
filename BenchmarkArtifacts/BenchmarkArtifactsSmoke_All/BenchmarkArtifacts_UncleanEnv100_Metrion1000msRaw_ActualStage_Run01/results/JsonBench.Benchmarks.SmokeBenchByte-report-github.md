```

BenchmarkDotNet v0.15.5-develop (2026-05-17), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 4.20GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   | 11.783 μs | 0.0643 μs | 0.0601 μs |     84,657 | 11.775 μs |      15.00 |                      64.65 |                   5473357.37 |
| STJSrcGen_Deser  | 15.674 μs | 0.2153 μs | 0.2014 μs |     64,144 | 15.568 μs |      15.00 |                      86.25 |                   5532401.53 |
| STJRefGen_Deser  | 15.966 μs | 0.3161 μs | 0.6805 μs |     64,224 | 15.585 μs |      56.00 |                      88.47 |                   5681911.18 |
| Utf8Json_Deser   | 18.276 μs | 0.1916 μs | 0.1793 μs |     54,990 | 18.214 μs |      15.00 |                     103.09 |                   5668656.99 |
| Newtonsoft_Deser | 28.513 μs | 0.2973 μs | 0.2781 μs |     35,142 | 28.476 μs |      15.00 |                     155.04 |                   5448495.09 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  5.631 μs | 0.0853 μs | 0.0798 μs |    178,688 |  5.597 μs |      15.00 |                      31.97 |                   5712050.19 |
| STJRefGen_Ser    |  7.839 μs | 0.1074 μs | 0.1005 μs |    127,552 |  7.809 μs |      15.00 |                      41.61 |                   5306896.60 |
| SpanJson_Ser     |  8.305 μs | 0.0563 μs | 0.0527 μs |    121,175 |  8.288 μs |      15.00 |                      47.08 |                   5704344.92 |
| Utf8Json_Ser     | 10.509 μs | 0.0698 μs | 0.0653 μs |     95,278 | 10.507 μs |      15.00 |                      55.13 |                   5252789.19 |
| Newtonsoft_Ser   | 16.674 μs | 0.3294 μs | 0.6876 μs |     60,174 | 16.494 μs |      53.00 |                      90.55 |                   5448808.99 |
