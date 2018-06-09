using System;
using System.Threading.Tasks;

namespace PerformanceTests.Infrastructure.Parallelling
{
    /// <summary>
    /// 并行服务接口
    /// </summary>
    public interface IParallelService
    {
        /// <summary>
        /// 并行数
        /// </summary>
        int Parallels { get; }
        /// <summary>
        /// 吞吐量
        /// </summary>
        int Throughput { get; }
        /// <summary>
        /// 分发任务
        /// </summary>
        /// <param name="action">操作</param>
        Task DispatchAsync(Action action);
    }
}
