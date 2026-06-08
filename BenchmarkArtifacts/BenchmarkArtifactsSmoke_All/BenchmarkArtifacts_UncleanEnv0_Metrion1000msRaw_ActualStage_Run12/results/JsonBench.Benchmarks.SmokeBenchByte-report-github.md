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
| SpanJson_Deser   |  5.713 μs | 0.1130 μs | 0.2815 μs |    180,288 |  5.546 μs |      73.00 |                     142.45 |                  25681632.69 |
| STJSrcGen_Deser  |  7.629 μs | 0.1512 μs | 0.3534 μs |    134,512 |  7.426 μs |      65.00 |                     200.86 |                  27017415.85 |
| STJRefGen_Deser  |  7.700 μs | 0.1535 μs | 0.3989 μs |    134,144 |  7.494 μs |      79.00 |                     205.68 |                  27590760.10 |
| Utf8Json_Deser   |  9.308 μs | 0.1842 μs | 0.4159 μs |     94,048 |  9.083 μs |      61.00 |                     234.62 |                  22065143.64 |
| Newtonsoft_Deser | 15.546 μs | 0.3231 μs | 0.9528 μs |     55,456 | 15.042 μs |     100.00 |                     398.89 |                  22120621.84 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  2.808 μs | 0.0561 μs | 0.1334 μs |    367,264 |  2.726 μs |      67.00 |                      72.98 |                  26802131.68 |
| STJRefGen_Ser    |  4.055 μs | 0.0809 μs | 0.1777 μs |    251,584 |  3.954 μs |      58.00 |                     105.40 |                  26516738.39 |
| SpanJson_Ser     |  4.665 μs | 0.0933 μs | 0.1968 μs |    213,824 |  4.564 μs |      54.00 |                     119.51 |                  25554511.09 |
| Utf8Json_Ser     |  5.254 μs | 0.1050 μs | 0.2496 μs |    166,960 |  5.124 μs |      67.00 |                     132.94 |                  22194887.16 |
| Newtonsoft_Ser   |  9.268 μs | 0.1841 μs | 0.5428 μs |     92,960 |  8.977 μs |     100.00 |                     238.45 |                  22165988.48 |
