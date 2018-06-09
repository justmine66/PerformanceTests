# PerformanceTests
一个CSharp写的性能/压测工具，支持dotnet2.1

## 事务测试(tps)
![](tps.png)

## 接口测试(qps)
![](qps.png)

## 启动命令
dotnet PerformanceTests.dll -p 4 -t 2000
>-p|--parallels 并行数，-t|--throughput 吞吐量

