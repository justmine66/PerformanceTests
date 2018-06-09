using McMaster.Extensions.CommandLineUtils;
using PerformanceTests.Common;
using PerformanceTests.Infrastructure.Performances;
using PerformanceTests.Transactions;
using System;
using System.Threading.Tasks;

namespace PerformanceTests
{
    class Program
    {
        private readonly IHttpPerformanceService _httpPerfService = new DefaultHttpPerformanceService();

        public static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;
            var app = new CommandLineApplication();
            var parallels = app.Option("-p|--parallels <cpu>", "并行数", CommandOptionType.SingleValue);
            var throughput = app.Option("-t|--throughput <throughput>", "吞吐量", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                var cpus = parallels.HasValue()
                    ? parallels.Value()
                    : "4";
                var tpses = throughput.HasValue()
                    ? throughput.Value()
                    : "2000";

                if (int.TryParse(cpus, out int parallelsInt) &&
                    int.TryParse(tpses, out int throughputInt))
                {
                    Guard.Positive(parallelsInt, nameof(parallelsInt));
                    Guard.Positive(throughputInt, nameof(throughputInt));

                    MainAsync(SelectTpsKey(), parallelsInt, throughputInt).GetAwaiter().GetResult();

                    Console.Read();
                }

                return 0;
            });

            return app.Execute(args);
        }

        private static async Task MainAsync(int tpsKey, int parallels, int throughput)
        {
            switch (tpsKey)
            {
                case 1:
                    Console.WriteLine($"事务: 正常下单流程, 并行数: {parallels}, 吞吐量: {throughput} 性能测试!!!");
                    await new CheckOutTps(parallels, throughput).CheckOutFlowAsync();
                    break;
                case 2:
                    Console.WriteLine($"事务: 获取APP版本信息, 并行数: {parallels}, 吞吐量: {throughput} 性能测试!!!");
                    await new CommonTps(parallels, throughput).GetVersionAsync();
                    break;
                case 3:
                    Console.WriteLine($"事务: 获取语言, 并行数: {parallels}, 吞吐量: {throughput} 性能测试!!!");
                    await new CommonTps(parallels, throughput).GetRestLanguageAsync();
                    break;
                case 4:
                    Console.WriteLine($"事务: 产品评论, 并行数: {parallels}, 吞吐量: {throughput} 性能测试!!!");
                    await new CommonTps(parallels, throughput).GetCatalogReviewAsync();
                    break;
                default:
                    Console.Write($"未匹配到任何事务{Environment.NewLine}");
                    MainAsync(SelectTpsKey(), parallels, throughput).GetAwaiter().GetResult();
                    break;
            }
        }

        private static int SelectTpsKey()
        {
            return Prompt.GetInt(@"选择事务，输入数字下标： " +
                                       $"{Environment.NewLine}1 - 正常下单流程" +
                                       $"{Environment.NewLine}2 - 获取APP版本信息" +
                                       $"{Environment.NewLine}3 - 获取语言" +
                                       $"{Environment.NewLine}4 - 产品评论"
                                 );
        }
        static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("{0:yyyy-MM-dd HH:mm:ss.fff} 捕获到未处理的异常, 异常: {1}", DateTime.Now, e.ExceptionObject.ToString());
        }
    }
}
