using System;

namespace PerformanceTests.Infrastructure.Scheduling
{
    /// <summary>
    /// 调度服务接口
    /// </summary>
    public interface IScheduleService
    {
        /// <summary>
        /// 开启指定任务
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="action">操作</param>
        /// <param name="dueTime">初始延误时间，单位：秒</param>
        /// <param name="period">间隔时间，单位：秒</param>
        void StartTask(string name, Action action, int dueTime, int period);
        /// <summary>
        /// 停止指定任务
        /// </summary>
        /// <param name="name">任务名称</param>
        void StopTask(string name);
    }
}
