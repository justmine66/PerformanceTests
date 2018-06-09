using PerformanceTests.Common;
using System;
using System.Threading.Tasks;

namespace PerformanceTests.Infrastructure.Parallelling
{
    /// <summary>
    /// 并行服务默认实现
    /// </summary>
    public class DefaultIParallelService : IParallelService
    {
        private readonly int _parallels;
        private readonly int _throughput;

        private readonly object _latchLock = new object();//闩锁

        /// <summary>
        /// 初始化一个 <see cref="DefaultIParallelService"/> 实例。
        /// </summary>
        /// <param name="parallels">并行数</param>
        /// <param name="throughput">吞吐量</param>
        public DefaultIParallelService(int parallels, int throughput)
        {
            Guard.Positive(parallels, nameof(parallels));
            Guard.Positive(throughput, nameof(throughput));

            _parallels = parallels;
            _throughput = throughput;
        }

        public int Parallels => _parallels;

        public int Throughput => _throughput;

        /// <summary>
        /// 分发任务
        /// <remarks>
        /// 为了跑完所有吞吐量，分发算法如下: 
        ///    刚好取整：假如并行度为3，吞吐量为12，每组线程串行数为4。
        ///    取整有余：假如并行度为3，吞吐量为10，第一、二组线程串行数为3，第三组线程串行数为4。
        /// </remarks>
        /// </summary>
        /// <param name="action">操作</param>
        public Task DispatchAsync(Action action)
        {
            Guard.NotNull(action, nameof(action));

            int unitNum = _throughput / _parallels;
            int remainder = _throughput % _parallels;
            if (remainder == 0)
            {
                Parallel.For(0, _parallels, (i) =>
                {
                    for (int j = 0; j < unitNum; j++)
                    {
                        lock (_latchLock)
                        {
                            action();
                        }
                    }
                });
            }
            else
            {
                Parallel.For(0, _parallels, (i) =>
                {
                    if (i < _parallels - 1)
                    {
                        for (int j = 0; j < unitNum; j++)
                        {
                            lock (_latchLock)
                            {
                                action();
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < unitNum + remainder; j++)
                        {
                            lock (_latchLock)
                            {
                                action();
                            }
                        }
                    }
                });
            }

            return Task.CompletedTask; ;
        }
    }
}
