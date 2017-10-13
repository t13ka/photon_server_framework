using System;
using System.Net;

namespace Warsmiths.Server.Common.Net
{
    public static class HttpWebRequestExtensions
    {
        public static AsyncHttpRequest GetAsync(this HttpWebRequest webRequest, Action<AsyncHttpRequest> callBack)
        {
            var asyncRequest = new AsyncHttpRequest(webRequest, callBack);
            asyncRequest.GetAsync();
            return asyncRequest;
        }

        public static AsyncHttpRequest PostAsync(this HttpWebRequest webRequest, Action<AsyncHttpRequest> callBack)
        {
            var asyncRequest = new AsyncHttpRequest(webRequest, callBack);
            asyncRequest.GetAsync();
            return asyncRequest;
        }
    }
}
