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
    /// <summary>
    /// 正常下单事务
    /// </summary>
    public class CheckOutTps
    {
        private readonly string perfName = "CheckOutFlow";
        private readonly string perfKey = "CheckOutFlow.Count";

        private readonly IHttpClient _httpClient;
        private readonly IPerformanceService _perfService;
        private readonly IHttpPerformanceService _httpPerfService;
        private readonly IParallelService _parallelService;
        private readonly ILogger _logger;

        private readonly TpsSettings _settings;
        private readonly string _authorizationToken;

        /// <summary>
        /// 初始化一个 <see cref="CheckOutTps"/> 实例。
        /// </summary>
        /// <param name="parallels">并行数</param>
        /// <param name="throughput">吞吐量</param>
        public CheckOutTps(int parallels, int throughput)
        {
            Guard.Positive(parallels, nameof(parallels));
            Guard.Positive(throughput, nameof(throughput));

            _perfService = new DefaultPerformanceService(new ScheduleService(new ConsoleLogger()), new ConsoleLogger());
            _parallelService = new DefaultIParallelService(parallels, throughput);
            _httpPerfService = new DefaultHttpPerformanceService();
            _httpClient = new StandardHttpClient(_httpPerfService);
            _logger = new ConsoleLogger();
            _settings = new TpsSettings();
            _authorizationToken = _settings.AccessToken;
        }
        /// <summary>
        /// 下单核心流程
        /// </summary>
        /// <returns></returns>
        public async Task CheckOutFlowAsync()
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

               await AddCartAsync();
               await GetCartCheckOutAsync();
               await ModifyCartCheckOutAsync();
               await CheckOutAsync();

               _perfService.IncrementKeyCount(perfKey, DateTime.UtcNow.Subtract(start).TotalMilliseconds);
           });
        }

        /// <summary>
        /// 1 添加商品到购物车
        /// </summary>
        private async Task AddCartAsync()
        {
            await _httpClient.PostAsync(
                _settings.GetUrl("/api"),
                "",
                _authorizationToken);
        }
        /// <summary>
        /// 2 获取购物车结算信息
        /// </summary>
        private async Task GetCartCheckOutAsync()
        {
            await _httpClient.GetStringAsync(_settings.GetUrl("/api"), _authorizationToken);
        }
        /// <summary>
        /// 3 修改购物车结算信息
        /// </summary>
        private async Task ModifyCartCheckOutAsync()
        {
            await _httpClient.PostAsync(_settings.GetUrl("/api"), "", _authorizationToken);
        }
        /// <summary>
        /// 4 结算购物车
        /// </summary>
        private async Task CheckOutAsync()
        {
            await _httpClient.PostAsync(
                 _settings.GetUrl("/api"),
                 "",
                 _authorizationToken,
                 "application/x-www-form-urlencoded");
        }
    }
}
