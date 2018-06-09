using PerformanceTests.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PerformanceTests.Infrastructure.Scheduling
{
    /// <summary>
    /// 调度服务默认实现
    /// </summary>
    public class ScheduleService : IScheduleService
    {
        private readonly object _latchLock = new object();//闩锁
        private readonly Dictionary<string, TimerBasedTask> _taskDict;
        private readonly ILogger _logger;

        public ScheduleService(ILogger logger)
        {
            _logger = logger;
            _taskDict = new Dictionary<string, TimerBasedTask>();
        }

        /// <summary>
        /// Starts a task.
        /// </summary>
        /// <param name="name">the indentifier of task.</param>
        /// <param name="action">a <see cref="System.Action"/> delegate representing a method to be executed.</param>
        /// <param name="dueTime">the amount of time to delay before action is invoked,in milliseconds. </param>
        /// <param name="period">the time interval between invocations of action,in milliseconds.</param>
        public void StartTask(string name, Action action, int dueTime, int period)
        {
            lock (_latchLock)
            {
                if (_taskDict.ContainsKey(name)) return;
                var timer = new Timer(TaskCallback, name, Timeout.Infinite, Timeout.Infinite);
                _taskDict.Add(name, new TimerBasedTask()
                {
                    Name = name,
                    Action = action,
                    DueTime = dueTime,
                    Period = period,
                    Timer = timer,
                    Stopped = false
                });
                timer.Change(dueTime, period);
            }
        }

        /// <summary>
        /// Stops a task.
        /// </summary>
        /// <param name="name">the indentifier of task.</param>
        public void StopTask(string name)
        {
            lock (_latchLock)
            {
                if (_taskDict.ContainsKey(name))
                {
                    TimerBasedTask task = _taskDict[name];
                    task.Stopped = true;
                    task.Timer.Dispose();
                    _taskDict.Remove(name);
                }
            }
        }

        private void TaskCallback(object state)
        {
            var taskName = state as string;
            TimerBasedTask task;

            if (_taskDict.TryGetValue(taskName, out task))
            {
                try
                {
                    if (!task.Stopped)
                    {
                        task.Timer.Change(Timeout.Infinite, Timeout.Infinite);
                        task.Action();
                    }
                }
                catch (ObjectDisposedException) { }
                catch (Exception exc)
                {
                    _logger?.Error(string.Format("Task has exception, name: {0}, due: {1}, period: {2}", task.Name, task.DueTime, task.Period), exc);
                }
                finally
                {
                    try
                    {
                        if (!task.Stopped)
                        {
                            task.Timer.Change(task.DueTime, task.Period);
                        }
                    }
                    catch (ObjectDisposedException) { }
                    catch (Exception ex)
                    {
                        _logger?.Error(string.Format("Timer change has exception, name: {0}, due: {1}, period: {2}", task.Name, task.DueTime, task.Period), ex);
                    }
                }
            }
        }

        class TimerBasedTask
        {
            public string Name;
            public Action Action;
            public Timer Timer;
            public int DueTime;
            public int Period;
            public bool Stopped;
        }
    }
}
