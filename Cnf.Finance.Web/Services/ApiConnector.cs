using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Cnf.Finance.Web.Services
{
    public class ApiConnector : IApiConnector
    {
        private readonly string _baseUrl;
        public ApiConnector(IOptionsSnapshot<AppSettings> options) =>
            _baseUrl = options.Value.ApiBaseUrl;

        /// <summary>
        /// 枚举可用的Http Method，没有列举在此枚举中的Http Method是不支持的。
        /// </summary>
        public enum HttpMethod
        {
            Delete, Get, Post, Put
        }

        static string GetMethodName(HttpMethod method) =>
            method.ToString();

        /// <summary>
        /// 定义本api实现支持的mime内容种类，没有列举在此常数表中的mime种类可能不被完全支持。
        /// </summary>
        public static class ContentType
        {
            public const string NULL = "";
            public const string JSON = "application/json";
            //public const string PLAINTEXT = "text/plain";
            //public const string HTML = "text/html";
            //public const string XMLDATA = "application/xml";
            //public const string XML = "text/xml";
            //public const string STEAM = "application/octet-stream";
        }

        private static HttpWebRequest CreateWebRequest(string fullUrl, HttpMethod method, string contentType, string postData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUrl);
            request.ServerCertificateValidationCallback = (s, c, e, t) => true;
            if (fullUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                request.ServerCertificateValidationCallback = (s, c, e, t) => true;
            }

            if (method switch
            {
                HttpMethod.Post => true,
                HttpMethod.Put => true,
                _ => false
            })
            {
                request.Method = GetMethodName(method);
                request.ContentType = contentType;
                byte[] buffer = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = buffer.Length;
                using (Stream post = request.GetRequestStream())
                {
                    post.Write(buffer, 0, buffer.Length);
                    post.Close();
                }
            }
            else
            {
                request.Method = GetMethodName(method);
            }
            return request;
        }

        private static string InvokeApi(string fullUrl, HttpMethod method, string contentType, string postData)
        {
            var request = CreateWebRequest(fullUrl, method, contentType, postData);
            using WebResponse response = request.GetResponse();
            using StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            return reader.ReadToEnd().ToString();
        }

        async static Task<string> InvokeApiAsync(string fullUrl, HttpMethod method, string contentType, string postData)
        {
            var request = CreateWebRequest(fullUrl, method, contentType, postData);

            using WebResponse response = await request.GetResponseAsync();
            using StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

        string BuildUrl(string route, string queryString="")=>
            _baseUrl.TrimEnd('/') + '/' + route.TrimStart('/').TrimEnd('/') + 
            (string.IsNullOrWhiteSpace(queryString) ? "" : ("?" + queryString.TrimStart('?')));

        public async Task<TReturn> HttpGetAsync<TReturn>(string route, string queryString = "")
        {
            string result = await InvokeApiAsync(BuildUrl(route, queryString), HttpMethod.Get, ContentType.NULL, "");
            if (string.IsNullOrEmpty(result))
                return default;

            return JsonConvert.DeserializeObject<TReturn>(result);
        }

        public async Task<TReturn> HttpDeleteAsync<TReturn>(string route, string queryString = "")
        {
            string result = await InvokeApiAsync(BuildUrl(route, queryString), HttpMethod.Delete, ContentType.NULL, "");
            if (string.IsNullOrEmpty(result))
                return default;
            
            return JsonConvert.DeserializeObject<TReturn>(result);
        }

        public async Task<TReturn> HttpPostAsync<T, TReturn>(string route, T data = default)
        {
            string result = await InvokeApiAsync(BuildUrl(route, ""), HttpMethod.Post, ContentType.JSON, JsonConvert.SerializeObject(data));
            if (string.IsNullOrEmpty(result))
                return default;

            return JsonConvert.DeserializeObject<TReturn>(result);
        }

        public async Task<TReturn> HttpPutAsync<T, TReturn>(string route, T data = default)
        {
            string result = await InvokeApiAsync(BuildUrl(route, ""), HttpMethod.Put, ContentType.JSON, JsonConvert.SerializeObject(data));
            if (string.IsNullOrEmpty(result))
                return default;
            return JsonConvert.DeserializeObject<TReturn>(result);
        }

    }
}
