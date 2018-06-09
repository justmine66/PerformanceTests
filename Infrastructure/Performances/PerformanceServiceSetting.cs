using System;

namespace PerformanceTests.Infrastructure.Performances
{
    /// <summary>
    /// 性能统计分析配置信息
    /// </summary>
    public class PerformanceServiceSetting
    {
        /// <summary>
        /// 统计计数初次延误间隔，单位：秒
        /// </summary>
        public int StatInitializeDelaySeconds { get; set; }
        /// <summary>
        /// 统计计数间隔，单位：秒
        /// </summary>
        public int StatIntervalSeconds { get; set; }
        /// <summary>
        /// 是否自动记录日志
        /// </summary>
        public bool AutoLogging { get; set; }
        /// <summary>
        /// 获取日志上下文
        /// </summary>
        public Func<string> GetLogContextTextFunc { get; set; }
        /// <summary>
        /// 性能指标信息处理器
        /// </summary>
        public Action<PerformanceInfo> PerformanceInfoHandler { get; set; }
    }
}
