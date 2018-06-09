namespace PerformanceTests.Infrastructure.Performances
{
    /// <summary>
    /// 性能指标参数信息
    /// </summary>
    public class PerformanceInfo
    {
        /// <summary>
        /// 总量
        /// </summary>
        public long TotalCount { get; private set; }
        /// <summary>
        /// 吞吐量
        /// </summary>
        public long Throughput { get; private set; }
        /// <summary>
        /// 平均吞吐量
        /// </summary>
        public long AverageThroughput { get; private set; }
        /// <summary>
        /// The response time.
        /// </summary>
        public double Rt { get; private set; }
        /// <summary>
        /// The average response time.
        /// </summary>
        public double AverageRt { get; private set; }
        /// <summary>
        /// 初始化一个 <see cref="PerformanceInfo"/> 实例
        /// </summary>
        /// <param name="totalCount">总量</param>
        /// <param name="throughput">吞吐量</param>
        /// <param name="averageThroughput">平均吞吐量</param>
        /// <param name="rt">响应时间</param>
        /// <param name="averageRt">平均响应时间</param>
        public PerformanceInfo(long totalCount, long throughput, long averageThroughput, double rt, double averageRt)
        {
            this.TotalCount = totalCount;
            this.Throughput = throughput;
            this.AverageThroughput = averageThroughput;
            this.Rt = rt;
            this.AverageRt = averageRt;
        }
    }
}
