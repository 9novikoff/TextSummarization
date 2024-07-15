```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i5-10300H CPU 2.50GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


```
| Method            | Mean        | Error      | StdDev     |
|------------------ |------------:|-----------:|-----------:|
| LuhnSummarize     |    11.84 ms |   0.310 ms |   0.909 ms |
| TextRankSummarize |    99.03 ms |   2.325 ms |   6.633 ms |
| LexRankSummarize  |   151.20 ms |   2.787 ms |   3.423 ms |
| LsaSummarize      | 9,366.28 ms | 138.727 ms | 129.765 ms |
