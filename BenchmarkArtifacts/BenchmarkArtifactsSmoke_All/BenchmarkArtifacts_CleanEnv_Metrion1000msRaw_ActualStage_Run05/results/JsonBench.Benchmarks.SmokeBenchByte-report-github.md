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
| SpanJson_Deser   |  7.336 μs | 0.0800 μs | 0.0748 μs |    137,481 |      15.00 |                     102.84 |                  14138919.88 |
| STJRefGen_Deser  |  9.827 μs | 0.0960 μs | 0.0898 μs |    102,544 |      15.00 |                     116.99 |                  11996487.43 |
| STJSrcGen_Deser  | 10.059 μs | 0.1654 μs | 0.1547 μs |    100,160 |      15.00 |                      65.78 |                   6588346.48 |
| Utf8Json_Deser   | 10.785 μs | 0.1166 μs | 0.1090 μs |     93,216 |      15.00 |                     143.01 |                  13330793.60 |
| Newtonsoft_Deser | 20.021 μs | 0.2574 μs | 0.2408 μs |     50,384 |      15.00 |                     277.41 |                  13976823.36 |
|                  |           |           |           |            |            |                            |                              |
| STJSrcGen_Ser    |  3.607 μs | 0.0440 μs | 0.0412 μs |    280,112 |      15.00 |                      40.64 |                  11383343.83 |
| STJRefGen_Ser    |  4.872 μs | 0.0573 μs | 0.0536 μs |    207,072 |      15.00 |                      69.00 |                  14288269.47 |
| Utf8Json_Ser     |  6.729 μs | 0.0679 μs | 0.0635 μs |    150,213 |      15.00 |                      82.73 |                  12427483.46 |
| SpanJson_Ser     |  7.055 μs | 0.0771 μs | 0.0721 μs |    143,916 |      15.00 |                     114.26 |                  16444410.92 |
| Newtonsoft_Ser   | 12.015 μs | 0.1722 μs | 0.1611 μs |     84,480 |      15.00 |                     168.36 |                  14223263.69 |
