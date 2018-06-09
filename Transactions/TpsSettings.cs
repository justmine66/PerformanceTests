using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace PerformanceTests.Transactions
{
    public class TpsSettings
    {
        public string Protocol => AppConfig.Protocol;
        public string Host => AppConfig.Host;
        public string AccessToken => GetAccessToken();
        public string GetUrl(string path = null)
        {
            return $"{Protocol}://{Host}{path}";
        }

        #region 获取访问令牌

        private readonly string IdentityBasicAuthorizationUserName = "appkey";
        private readonly string IdentityBasicAuthorizationPassword = "appsecret";
        private readonly string IdenttiyUserName = "UserName";
        private readonly string IdenttiyPassword = "Password";
        private string GetAccessToken()
        {
            var basicAuth =
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(
                        $"{IdentityBasicAuthorizationUserName}:{IdentityBasicAuthorizationPassword}"));

            var req = WebRequest.CreateHttp($"{GetUrl()}/connect/token");
            //设置Basic授权
            req.Headers.Add("Authorization", $"Basic {basicAuth}");
            req.ContentType = "application/x-www-form-urlencoded";

            var data = Encoding.UTF8.GetBytes(
                $"grant_type=password&username={IdenttiyUserName}&password={IdenttiyPassword}");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            //写入请求数据
            var requestStream = req.GetRequestStream();
            requestStream.Write(data, 0, data.Length);

            var result = "";
            try
            {
                using (var respStream = req.GetResponse().GetResponseStream())
                {
                    var sr = new StreamReader(respStream ??
                                              throw new InvalidOperationException("Response stream is null"));
                    result = sr.ReadToEnd();
                }
            }
            catch (Exception)
            {
                throw;
            }

            var jsonData = JObject.Parse(result);
            if (jsonData.ContainsKey("error"))
            {
                var tip = jsonData.ContainsKey("error_description")
                    ? jsonData["error_description"].ToString()
                    : jsonData["error"].ToString();
                throw new Exception("Authorize Failed，Detail:" + tip);
            }

            return jsonData["access_token"].ToString();
        }

        #endregion
    }
}
