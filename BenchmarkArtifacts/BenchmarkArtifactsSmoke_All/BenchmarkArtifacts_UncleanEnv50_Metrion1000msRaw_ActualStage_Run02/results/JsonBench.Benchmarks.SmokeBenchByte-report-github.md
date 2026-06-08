```

BenchmarkDotNet v0.15.5-develop (2026-05-17), Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Core i9-9900K CPU 3.60GHz (Max: 4.60GHz) (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.107
  [Host] : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  Energy : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=Energy  OutlierMode=DontRemove  IterationTime=1s  

```
| Method           | Mean      | Error     | StdDev    | Operations | Median    | Iterations | Metrion CPU Energy (uJ/op) | Metrion CPU Energy (uJ/iter) |
|----------------- |----------:|----------:|----------:|-----------:|----------:|-----------:|---------------------------:|-----------------------------:|
| SpanJson_Deser   |  8.498 μs | 0.2550 μs | 0.7520 μs |     99,051 |  8.256 μs |      100.0 |                      73.78 |                   7308259.53 |
| STJRefGen_Deser  | 11.289 μs | 0.3137 μs | 0.9250 μs |     73,200 | 10.928 μs |      100.0 |                      98.87 |                   7237333.41 |
| STJSrcGen_Deser  | 11.321 μs | 0.3348 μs | 0.9871 μs |     79,008 | 11.069 μs |      100.0 |                      98.55 |                   7785922.39 |
| Utf8Json_Deser   | 12.359 μs | 0.3601 μs | 1.0618 μs |     66,100 | 11.995 μs |      100.0 |                     107.61 |                   7112864.62 |
| Newtonsoft_Deser | 22.815 μs | 0.6912 μs | 2.0379 μs |     37,824 | 22.066 μs |      100.0 |                     199.32 |                   7538935.70 |
|                  |           |           |           |            |           |            |                            |                              |
| STJSrcGen_Ser    |  4.171 μs | 0.1375 μs | 0.4055 μs |    224,192 |  4.079 μs |      100.0 |                      36.44 |                   8168756.64 |
| STJRefGen_Ser    |  5.873 μs | 0.1652 μs | 0.4870 μs |    142,624 |  5.759 μs |      100.0 |                      49.67 |                   7083702.76 |
| SpanJson_Ser     |  6.585 μs | 0.1403 μs | 0.4137 μs |    153,831 |  6.451 μs |      100.0 |                      57.76 |                   8885752.73 |
| Utf8Json_Ser     |  7.811 μs | 0.2255 μs | 0.6649 μs |    110,816 |  7.634 μs |      100.0 |                      68.75 |                   7618299.59 |
| Newtonsoft_Ser   | 12.979 μs | 0.3831 μs | 1.1296 μs |     66,992 | 12.644 μs |      100.0 |                     113.24 |                   7586065.59 |
