using System;
using System.Net.Http;

namespace SpartanHub.Core.Exceptions
{
    public class HaloApiException : Exception
    {
        public string RequestUrl { get; }
        public System.Net.HttpStatusCode StatusCode { get; }
        public string ResponseContent { get; }

        public HaloApiException(string requestUrl, HttpResponseMessage response)
            : base($"API请求失败: {response.StatusCode} - {requestUrl}")
        {
            RequestUrl = requestUrl;
            StatusCode = response.StatusCode;
            ResponseContent = response.Content?.ToString() ?? string.Empty;
        }
    }
}
