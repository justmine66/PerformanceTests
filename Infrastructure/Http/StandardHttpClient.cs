﻿using Newtonsoft.Json;
using PerformanceTests.Common;
using PerformanceTests.Infrastructure.Performances;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTests.Infrastructure.Http
{
    public class StandardHttpClient : IHttpClient
    {
        private static readonly HttpClient Client = new HttpClient();

        private readonly IHttpPerformanceService _httpPerfService;

        public StandardHttpClient(IHttpPerformanceService httpPerfService)
        {
            Guard.NotNull(httpPerfService, nameof(httpPerfService));

            _httpPerfService = new DefaultHttpPerformanceService();
        }

        /// <summary>
        /// Http Get 异步调用
        /// </summary>
        /// <param name="uri">源地址</param>
        /// <param name="authorizationToken">授权Token</param>
        /// <param name="authorizationMethod">授权方法</param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            Guard.NotNullOrEmpty(uri, nameof(uri));

            return await Helper.EatExceptionAsync<string>(async () =>
            {
                uri = NormalizeUri(uri);
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
                SetAuthorizationHeader(requestMessage, authorizationMethod, authorizationToken);

                var response = await Client.SendAsync(requestMessage);

                _httpPerfService.IncrementStatusCode((int)response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return await response.Content.ReadAsStringAsync();
            }, () => _httpPerfService.IncrementStatusCode(512));
        }

        /// <summary>
        /// Http Post 异步调用
        /// </summary>
        /// <param name="uri">源地址</param>
        /// <param name="item">数据项</param>
        /// <param name="authorizationToken">授权Token</param>
        /// <param name="mediaType">内容协议</param>
        /// <param name="authorizationMethod">授权方法</param>
        public async Task<HttpResponseMessage> PostAsync<T>(
            string uri,
            T item,
            string authorizationToken = null,
            string mediaType = "application/json",
            string authorizationMethod = "Bearer")
        {
            Guard.NotNullOrEmpty(uri, nameof(uri));

            return await Helper.EatExceptionAsync<HttpResponseMessage>(async () =>
            {
                uri = NormalizeUri(uri);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
                SetAuthorizationHeader(requestMessage, authorizationMethod, authorizationToken);
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, mediaType);

                var response = await Client.SendAsync(requestMessage);

                _httpPerfService.IncrementStatusCode((int)response.StatusCode);

                return response;
            }, () => _httpPerfService.IncrementStatusCode(512));
        }
        /// <summary>
        /// 设置授权头
        /// </summary>
        /// <param name="requestMessage">请求消息体</param>
        /// <param name="authorizationMethod">授权方法</param>
        /// <param name="authorizationToken">授权码</param>
        private void SetAuthorizationHeader(
            HttpRequestMessage requestMessage,
            string authorizationMethod,
            string authorizationToken)
        {
            if (!string.IsNullOrEmpty(authorizationToken))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }
        }
        /// <summary>
        /// 标准化地址
        /// </summary>
        /// <param name="uri">请求地址</param>
        private string NormalizeUri(string uri)
        {
            return uri?.Trim()?.ToLower();
        }
    }
}
