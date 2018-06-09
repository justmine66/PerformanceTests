namespace PerformanceTests.Infrastructure.Performances
{
    /// <summary>
    /// 性能服务接口
    /// </summary>
    public interface IPerformanceService
    {
        string Name { get; }
        PerformanceServiceSetting Setting { get; }
        IPerformanceService Initialize(string name, PerformanceServiceSetting setting = null);
        void Start();
        void Stop();
        void IncrementKeyCount(string key, double rtMilliseconds);
        void UpdateKeyCount(string key, long count, double rtMilliseconds);
        PerformanceInfo GetKeyPerformanceInfo(string key);
    }
}
