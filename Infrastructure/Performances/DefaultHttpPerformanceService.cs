using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerformanceTests.Infrastructure.Performances
{
    public class DefaultHttpPerformanceService : IHttpPerformanceService
    {
        public static readonly List<int> StatusCodeList = new List<int>();

        public IEnumerable<HttpPerformanceInfo> GetPerformanceInfo()
        {
            var perfInfo = from s in StatusCodeList
                           group s by s into g
                           orderby g.Count() descending
                           select new HttpPerformanceInfo
                           {
                               StatusCode = g.Key,
                               Count = g.Count()
                           };

            return perfInfo;
        }

        public string GetPerformanceInfoString()
        {
            var sb = new StringBuilder();

            foreach (var perfInfo in GetPerformanceInfo())
            {
                sb.AppendLine(perfInfo.ToString());
            }

            return sb.ToString();
        }

        public void IncrementStatusCode(int code)
        {
            StatusCodeList.Add(code);
        }
    }
}
