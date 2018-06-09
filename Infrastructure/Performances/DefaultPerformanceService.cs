using PerformanceTests.Common;
using PerformanceTests.Infrastructure.Logging;
using PerformanceTests.Infrastructure.Scheduling;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PerformanceTests.Infrastructure.Performances
{
    /// <summary>
    /// 性能服务默认实现
    /// </summary>
    public class DefaultPerformanceService : IPerformanceService
    {
        private string _name;
        private PerformanceServiceSetting _setting;
        private string _taskName;

        private readonly ILogger _logger;
        private readonly IScheduleService _scheduleService;
        private readonly ConcurrentDictionary<string, CountInfo> _countInfoDict;
        /// <summary>
        /// 初始化一个 <see cref="DefaultPerformanceService"/> 实例
        /// </summary>
        /// <param name="scheduleService">调度服务</param>
        /// <param name="logger">日志组件</param>
        public DefaultPerformanceService(IScheduleService scheduleService, ILogger logger)
        {
            _scheduleService = scheduleService;
            _logger = logger;
            _countInfoDict = new ConcurrentDictionary<string, CountInfo>();
        }
        public string Name => _name;
        public PerformanceServiceSetting Setting => _setting;
        public PerformanceInfo GetKeyPerformanceInfo(string key)
        {
            if (_countInfoDict.TryGetValue(key, out CountInfo countInfo))
            {
                return countInfo.GetCurrentPerformanceInfo();
            }
            return null;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="setting">配置信息</param>
        public IPerformanceService Initialize(string name, PerformanceServiceSetting setting = null)
        {
            Guard.NotNullOrEmpty(name, "name");

            if (setting == null)
            {
                _setting = new PerformanceServiceSetting
                {
                    AutoLogging = true,
                    StatInitializeDelaySeconds = 1,
                    StatIntervalSeconds = 1
                };
            }
            else
            {
                _setting = setting;
            }

            Guard.Positive(_setting.StatInitializeDelaySeconds, nameof(_setting.StatInitializeDelaySeconds));
            Guard.Positive(_setting.StatIntervalSeconds, nameof(_setting.StatIntervalSeconds));

            _name = name;
            _taskName = name + ".Task";

            return this;
        }
        /// <summary>
        /// 启动计数
        /// </summary>
        public void Start()
        {
            if (string.IsNullOrWhiteSpace(_taskName))
            {
                throw new Exception(string.Format("Please initialize the {0} before starting it.", nameof(DefaultPerformanceService)));
            }

            _scheduleService.StartTask(_taskName, () =>
            {
                foreach (var entry in _countInfoDict)
                {
                    entry.Value?.Calculate();
                }
            }, _setting.StatInitializeDelaySeconds * 1000, _setting.StatIntervalSeconds * 1000);
        }
        /// <summary>
        /// 停止计数
        /// </summary>
        public void Stop()
        {
            if (string.IsNullOrEmpty(_taskName))
            {
                return;
            }

            _scheduleService.StopTask(_taskName);
        }
        /// <summary>
        /// 新增计数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="rtMilliseconds">单位响应时间</param>
        public void IncrementKeyCount(string key, double rtMilliseconds)
        {
            _countInfoDict.AddOrUpdate(key,
            x => new CountInfo(this, 1, rtMilliseconds),
            (x, y) =>
            {
                y.IncrementTotalCount(rtMilliseconds);
                return y;
            });
        }
        /// <summary>
        /// 更新计数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="count">单位计数</param>
        /// <param name="rtMilliseconds">单位响应时间</param>
        public void UpdateKeyCount(string key, long count, double rtMilliseconds)
        {
            _countInfoDict.AddOrUpdate(key,
            x => new CountInfo(this, count, rtMilliseconds),
            (x, y) =>
            {
                y.UpdateTotalCount(count, rtMilliseconds);
                return y;
            });
        }
        /// <summary>
        /// 统计计数信息
        /// </summary>
        public class CountInfo
        {
            private readonly DefaultPerformanceService _service;

            private long _totalCount;
            private long _previousCount;
            private long _throughput;
            private long _averageThroughput;
            private long _throughputCalculateCount;

            private long _rtCount;
            private long _totalRtTime;
            private long _rtTime;
            private double _rt;
            private double _averageRt;
            private long _rtCalculateCount;
            /// <summary>
            /// 初始化一个 <see cref="CountInfo"/> 实例
            /// </summary>
            /// <param name="service">性能服务</param>
            /// <param name="initialCount">初始计数</param>
            /// <param name="rtMilliseconds">响应时间，单位：毫秒</param>
            public CountInfo(DefaultPerformanceService service, long initialCount, double rtMilliseconds)
            {
                _service = service;
                _totalCount = initialCount;
                _rtCount = initialCount;
                Interlocked.Add(ref _rtTime, (long)(rtMilliseconds * 1000));
                Interlocked.Add(ref _totalRtTime, (long)(rtMilliseconds * 1000));
            }
            /// <summary>
            /// 新增单位计数
            /// </summary>
            /// <param name="rtMilliseconds">单位响应时间，单位：毫秒</param>
            public void IncrementTotalCount(double rtMilliseconds)
            {
                Interlocked.Increment(ref _totalCount);
                Interlocked.Increment(ref _rtCount);
                Interlocked.Add(ref _rtTime, (long)(rtMilliseconds * 1000));
                Interlocked.Add(ref _totalRtTime, (long)(rtMilliseconds * 1000));
            }
            /// <summary>
            /// 更新单位计数
            /// </summary>
            /// <param name="count">单位计数</param>
            /// <param name="rtMilliseconds">单位响应时间</param>
            public void UpdateTotalCount(long count, double rtMilliseconds)
            {
                _totalCount = count;
                _rtCount = count;
                Interlocked.Add(ref _rtTime, (long)(rtMilliseconds * 1000));
                Interlocked.Add(ref _totalRtTime, (long)(rtMilliseconds * 1000));
            }
            /// <summary>
            /// 单位计数
            /// </summary>
            public void Calculate()
            {
                //001 度量指标
                CalculateThroughput();
                CalculateRt();
                //002 日志
                if (_service._setting.AutoLogging)
                {
                    var contextText = string.Empty;
                    if (_service._setting.GetLogContextTextFunc != null)
                    {
                        contextText = _service._setting.GetLogContextTextFunc();
                    }
                    if (!string.IsNullOrWhiteSpace(contextText))
                    {
                        contextText += ", ";
                    }
                    _service._logger.InfoFormat("{0}, {1}totalCount: {2}, throughput: {3}, averageThrughput: {4}, rt: {5:F3}ms, averageRT: {6:F3}ms", _service._name, contextText, _totalCount, _throughput, _averageThroughput, _rt, _averageRt);
                }
                //003 预警
                if (_service._setting.PerformanceInfoHandler != null)
                {
                    try
                    {
                        _service._setting.PerformanceInfoHandler(GetCurrentPerformanceInfo());
                    }
                    catch (Exception ex)
                    {
                        _service._logger.Error("PerformanceInfo handler execution has exception.", ex);
                    }
                }
            }
            /// <summary>
            /// 获取当前性能指标信息
            /// </summary>
            /// <returns></returns>
            public PerformanceInfo GetCurrentPerformanceInfo()
            {
                //传入计算出来的度量指标即可
                return new PerformanceInfo(TotalCount, Throughput, AverageThroughput, RT, AverageRT);
            }
            /// <summary>
            /// 总吞吐量
            /// </summary>
            public long TotalCount
            {
                get { return _totalCount; }
            }
            /// <summary>
            /// 单位吞吐量
            /// </summary>
            public long Throughput
            {
                get { return _throughput; }
            }
            /// <summary>
            /// 平均吞吐量
            /// </summary>
            public long AverageThroughput
            {
                get { return _averageThroughput; }
            }
            /// <summary>
            /// 单位响应时间
            /// </summary>
            public double RT
            {
                get { return _rt; }
            }
            /// <summary>
            /// 平均响应时间
            /// </summary>
            public double AverageRT
            {
                get { return _averageRt; }
            }
            /// <summary>
            /// 计算吞吐量
            /// <remarks>
            /// 单位吞吐量 = 总吞吐量 - 前单位吞吐量
            /// 平均吞吐量 = 总吞吐量 / 单位吞吐量总计数
            /// </remarks>
            /// </summary>
            private void CalculateThroughput()
            {
                var totalCount = _totalCount;
                _throughput = totalCount - _previousCount;
                _previousCount = totalCount;

                if (_throughput > 0)
                {
                    _throughputCalculateCount++;
                    _averageThroughput = totalCount / _throughputCalculateCount;
                }
            }
            /// <summary>
            /// 计算响应时间
            /// <remarks>
            /// 单位响应时间 = 单位响应总时间 / 响应时间单位计数
            /// 平均响应时间 = 总响应时间 / 响应时间总计数
            /// </remarks>
            /// </summary>
            private void CalculateRt()
            {
                var rtCount = _rtCount;
                var rtTime = _rtTime;
                var totalRtTime = _totalRtTime;

                if (rtCount > 0)
                {
                    _rt = ((double)rtTime / 1000) / rtCount;
                    _rtCalculateCount += rtCount;
                    _averageRt = ((double)totalRtTime / 1000) / _rtCalculateCount;
                }

                _rtCount = 0L;
                _rtTime = 0L;
            }
        }
    }
}
