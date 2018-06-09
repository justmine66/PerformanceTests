using System.Collections.Generic;

namespace PerformanceTests.Infrastructure.Performances
{
    public interface IHttpPerformanceService
    {
        void IncrementStatusCode(int code);
        IEnumerable<HttpPerformanceInfo> GetPerformanceInfo();
        string GetPerformanceInfoString();
    }
}
