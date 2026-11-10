
## Environment
OS: Ubuntu 24.04.5 LTS \
Framework: 9.0.22 \
Processors: 4 \
Machine Name: codeserver \
64-bit Process: True \
64-bit OS: True 
## 100000
|Тип|Stopwatch.Elapsed|
|---|---|
| LINQ | 00:00:00.0092913 |
| simple foreach | 00:00:00.0005282 |
| parallel List<Thread> | 00:00:00.0928466 |
| PLINQ | 00:00:00.1489243 |
## 1000000
|Тип|Stopwatch.Elapsed|
|---|---|
| LINQ | 00:00:00.0089690 |
| simple foreach | 00:00:00.0038063 |
| parallel List<Thread> | 00:00:00.0580034 |
| PLINQ | 00:00:00.0120899 |
## 10000000
|Тип|Stopwatch.Elapsed|
|---|---|
| LINQ | 00:00:00.0773595 |
| simple foreach | 00:00:00.0395555 |
| parallel List<Thread> | 00:00:00.1023213 |
| PLINQ | 00:00:00.0558705 |