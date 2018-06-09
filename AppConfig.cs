using System.Configuration;

namespace PerformanceTests
{
    public class AppConfig
    {
        public static readonly string Protocol = ConfigurationManager.AppSettings["Protocol"];
        public static readonly string Host = ConfigurationManager.AppSettings["Host"];
    }
}
