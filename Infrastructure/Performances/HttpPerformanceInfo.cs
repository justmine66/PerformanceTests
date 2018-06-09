namespace PerformanceTests.Infrastructure.Performances
{
    /// <summary>
    /// http 性能指标参数信息
    /// </summary>
    public class HttpPerformanceInfo
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// 计数
        /// </summary>
        public int Count { get; set; }

        public override string ToString()
        {
            return $"[StatusCode: {StatusCode}, Count: {Count}]";
        }
    }
}
