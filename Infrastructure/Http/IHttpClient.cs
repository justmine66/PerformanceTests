using System.Net.Http;
using System.Threading.Tasks;

namespace PerformanceTests.Infrastructure.Http
{
    public interface IHttpClient
    {
        Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string mediaType = "application/json", string authorizationMethod = "Bearer");
    }
}
