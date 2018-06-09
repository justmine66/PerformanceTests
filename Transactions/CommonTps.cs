using PerformanceTests.Common;
using PerformanceTests.Infrastructure.Http;
using PerformanceTests.Infrastructure.Logging;
using PerformanceTests.Infrastructure.Parallelling;
using PerformanceTests.Infrastructure.Performances;
using PerformanceTests.Infrastructure.Scheduling;
using System;
using System.Threading.Tasks;

namespace PerformanceTests.Transactions
{
    public class CommonTps
    {
        private readonly string perfName = "CommonTps";
        private readonly string perfKey = "CommonTps.Count";

        private readonly IHttpClient _httpClient;
        private readonly IPerformanceService _perfService;
        private readonly IHttpPerformanceService _httpPerfService;
        private readonly IParallelService _parallelService;
        private readonly ILogger _logger;

        private readonly TpsSettings _settings;

        /// <summary>
        /// 初始化一个 <see cref="CheckOutTps"/> 实例。
        /// </summary>
        /// <param name="parallels">并行数</param>
        /// <param name="throughput">吞吐量</param>
        public CommonTps(int parallels, int throughput)
        {
            Guard.Positive(parallels, nameof(parallels));
            Guard.Positive(throughput, nameof(throughput));

            _perfService = new DefaultPerformanceService(new ScheduleService(new ConsoleLogger()), new ConsoleLogger());
            _parallelService = new DefaultIParallelService(parallels, throughput);
            _httpPerfService = new DefaultHttpPerformanceService();
            _httpClient = new StandardHttpClient(_httpPerfService);
            _logger = new ConsoleLogger();
            _settings = new TpsSettings();
        }

        public async Task GetVersionAsync()
        {
            _perfService
                .Initialize(perfName, new PerformanceServiceSetting
                {
                    AutoLogging = true,
                    StatInitializeDelaySeconds = 1,
                    StatIntervalSeconds = 1,
                    PerformanceInfoHandler = (metrics) =>
                    {
                        if (metrics.TotalCount == _parallelService.Throughput)
                        {
                            var perfInfo = _httpPerfService.GetPerformanceInfoString();
                            _logger.Info(perfInfo);
                            _perfService.Stop();
                        }
                    }
                })
                .Start();

            await _parallelService.DispatchAsync(async () =>
           {
               var start = DateTime.UtcNow;

               await VersionAsync();

               _perfService.IncrementKeyCount(perfKey, DateTime.UtcNow.Subtract(start).TotalMilliseconds);
           });
        }
        public async Task GetRestLanguageAsync()
        {
            _perfService
                .Initialize(perfName, new PerformanceServiceSetting
                {
                    AutoLogging = true,
                    StatInitializeDelaySeconds = 1,
                    StatIntervalSeconds = 1,
                    PerformanceInfoHandler = (metrics) =>
                    {
                        if (metrics.TotalCount == _parallelService.Throughput)
                        {
                            var perfInfo = _httpPerfService.GetPerformanceInfoString();
                            _logger.Info(perfInfo);
                            _perfService.Stop();
                        }
                    }
                })
                .Start();

            await _parallelService.DispatchAsync(async () =>
           {
               var start = DateTime.UtcNow;

               await RestLanguageAsync();

               _perfService.IncrementKeyCount(perfKey, DateTime.UtcNow.Subtract(start).TotalMilliseconds);
           });
        }
        public async Task GetCatalogReviewAsync()
        {
            _perfService
                .Initialize(perfName, new PerformanceServiceSetting
                {
                    AutoLogging = true,
                    StatInitializeDelaySeconds = 1,
                    StatIntervalSeconds = 1,
                    PerformanceInfoHandler = (metrics) =>
                    {
                        if (metrics.TotalCount == _parallelService.Throughput)
                        {
                            var perfInfo = _httpPerfService.GetPerformanceInfoString();
                            _logger.Info(perfInfo);
                            _perfService.Stop();
                        }
                    }
                })
                .Start();

            await _parallelService.DispatchAsync(async () =>
            {
                var start = DateTime.UtcNow;

                await CatalogReviewAsync();

                _perfService.IncrementKeyCount(perfKey, DateTime.UtcNow.Subtract(start).TotalMilliseconds);
            });
        }

        /// <summary>
        /// 获取版本信息
        /// </summary>
        private async Task VersionAsync()
        {
            await _httpClient.GetStringAsync(_settings.GetUrl("/api/common"));
        }
        /// <summary>
        /// 获取语言
        /// </summary>
        private async Task RestLanguageAsync()
        {
            await _httpClient.GetStringAsync(_settings.GetUrl("/api/common"));
        }
        /// <summary>
        /// 产品评论
        /// </summary>
        private async Task CatalogReviewAsync()
        {
            await _httpClient.GetStringAsync(
                _settings.GetUrl("/api/catalog"));
        }
    }
}
