using System;
using System.Collections.Generic;
using System.Net;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;

namespace YourGame.Server.Common.Net
{
    public delegate void HttpRequestQueueCallback(
        HttpRequestQueueResultCode result, AsyncHttpRequest request, object userState);

    public enum HttpRequestQueueResultCode
    {
        Success,
        RequestTimeout,
        QueueTimeout,
        Offline,
        QueueFull,
        Error
    }

    public enum HttpRequestQueueState
    {
        Running,
        Connecting,
        Reconnecting,
        Offline
    }

    public class HttpRequestQueue : IDisposable
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly IFiber fiber;

        private readonly LinkedList<QueuedRequest> queue = new LinkedList<QueuedRequest>();

        public readonly TimeSpan QueueTimeout = TimeSpan.FromSeconds(5);

        private DateTime nextReconnectTime;

        public HttpRequestQueue()
        {
            QueueState = HttpRequestQueueState.Connecting;
            MaxConcurrentRequests = 1;
            MaxQueuedRequests = 100;

            fiber = new PoolFiber();
            fiber.Start();
        }

        public HttpRequestQueue(IFiber fiber)
        {
            QueueState = HttpRequestQueueState.Connecting;
            MaxConcurrentRequests = 1;
            MaxQueuedRequests = 100;
            ReconnectInterval = TimeSpan.FromMinutes(1);

            this.fiber = fiber;
            this.fiber.Start();
        }

        public int QueuedRequestCount
        {
            get { return queue.Count; }
        }

        public HttpRequestQueueState QueueState { get; private set; }

        public int RunningRequestsCount { get; private set; }

        public int MaxConcurrentRequests ;

        public int MaxQueuedRequests ;

        public int MaxTimedOutRequests ;

        public int TimedOutRequests { get; private set; }

        public TimeSpan ReconnectInterval ;

        /// <summary>
        ///     Releases the unmanaged resources and disposes of the managed
        ///     resources used by the <see cref="HttpRequestQueue" /> .
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HttpRequestQueue()
        {
            Dispose(false);
        }

        public void Enqueue(string requestUri, HttpRequestQueueCallback callback, object state)
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(requestUri);
            fiber.Enqueue(() => ExecuteRequest(webRequest, callback, state));
        }

        public void Enqueue(HttpWebRequest webRequest, HttpRequestQueueCallback callback, object state)
        {
            fiber.Enqueue(() => ExecuteRequest(webRequest, callback, state));
        }

        private void ExecuteRequest(HttpWebRequest webRequest, HttpRequestQueueCallback callback, object state)
        {
            if (queue.Count > MaxQueuedRequests)
            {
                callback(HttpRequestQueueResultCode.QueueFull, null, state);
                return;
            }

            var request = new QueuedRequest {Request = webRequest, Callback = callback, PostData = null, State = state};

            switch (QueueState)
            {
                case HttpRequestQueueState.Connecting:
                    ExecuteRequestConnecting(request);
                    break;

                case HttpRequestQueueState.Reconnecting:
                    ExecuteRequestReconnecting(request);
                    break;

                case HttpRequestQueueState.Running:
                    ExecuteRequestOnline(request);
                    break;

                case HttpRequestQueueState.Offline:
                    ExecuteRequestOffline(request);
                    break;
            }
        }

        private void ExecuteHttpRequest(QueuedRequest request)
        {
            try
            {
                RunningRequestsCount++;
                var asyncHttpRequest = new AsyncHttpRequest(request.Request, WebRequestCallback, request);
                if (request.PostData == null)
                {
                    asyncHttpRequest.GetAsync();
                }
                else
                {
                    asyncHttpRequest.PostAsync(request.PostData);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void ExecuteRequestOnline(QueuedRequest request)
        {
            if (RunningRequestsCount >= MaxConcurrentRequests)
            {
                queue.AddLast(request);
                return;
            }

            ExecuteHttpRequest(request);
        }

        private void ExecuteRequestConnecting(QueuedRequest request)
        {
            if (RunningRequestsCount < MaxConcurrentRequests)
            {
                ExecuteHttpRequest(request);
                return;
            }

            queue.AddLast(request);
        }

        private void ExecuteRequestReconnecting(QueuedRequest request)
        {
            if (RunningRequestsCount < 1)
            {
                ExecuteHttpRequest(request);
                return;
            }

            request.Callback(HttpRequestQueueResultCode.Offline, null, request.State);
        }

        private void ExecuteRequestOffline(QueuedRequest request)
        {
            if (DateTime.UtcNow >= nextReconnectTime)
            {
                QueueState = HttpRequestQueueState.Reconnecting;
                ExecuteHttpRequest(request);
                return;
            }

            request.Callback(HttpRequestQueueResultCode.Offline, null, request.State);
        }

        private void WebRequestCallback(AsyncHttpRequest request)
        {
            fiber.Enqueue(() => ProcessWebResponse(request));
        }

        private void ProcessWebResponse(AsyncHttpRequest request)
        {
            RunningRequestsCount--;

            switch (QueueState)
            {
                case HttpRequestQueueState.Connecting:
                    ProcessResponseConnecting(request);
                    break;

                case HttpRequestQueueState.Running:
                    ProcessResponseRunning(request);
                    break;

                case HttpRequestQueueState.Reconnecting:
                    ProcessResponseReConnecting(request);
                    break;

                case HttpRequestQueueState.Offline:
                    ProcessResponseOffline(request);
                    break;
            }

            ProcessQueuedItems();
        }

        private void ProcessResponseConnecting(AsyncHttpRequest request)
        {
            var queuedRequest = (QueuedRequest) request.State;

            switch (request.WebStatus)
            {
                case WebExceptionStatus.Success:
                    QueueState = HttpRequestQueueState.Running;
                    DecrementTimedOutCount();
                    queuedRequest.Callback(HttpRequestQueueResultCode.Success, request, queuedRequest.State);
                    break;

                case WebExceptionStatus.Timeout:
                    IncrementTimedOutCount();
                    queuedRequest.Callback(HttpRequestQueueResultCode.RequestTimeout, request, queuedRequest.State);
                    break;

                default:
                    SetOffline();
                    queuedRequest.Callback(HttpRequestQueueResultCode.Error, request, queuedRequest.State);
                    break;
            }
        }

        private void ProcessResponseReConnecting(AsyncHttpRequest request)
        {
            var queuedRequest = (QueuedRequest) request.State;

            switch (request.WebStatus)
            {
                case WebExceptionStatus.Success:
                    QueueState = HttpRequestQueueState.Running;
                    queuedRequest.Callback(HttpRequestQueueResultCode.Success, request, queuedRequest.State);
                    break;

                case WebExceptionStatus.Timeout:
                    SetOffline();
                    queuedRequest.Callback(HttpRequestQueueResultCode.RequestTimeout, request, queuedRequest.State);
                    break;

                default:
                    SetOffline();
                    queuedRequest.Callback(HttpRequestQueueResultCode.Error, request, queuedRequest.State);
                    break;
            }
        }

        private void ProcessResponseRunning(AsyncHttpRequest request)
        {
            var queuedRequest = (QueuedRequest) request.State;

            switch (request.WebStatus)
            {
                case WebExceptionStatus.Success:
                    DecrementTimedOutCount();
                    queuedRequest.Callback(HttpRequestQueueResultCode.Success, request, queuedRequest.State);
                    break;

                case WebExceptionStatus.Timeout:
                    IncrementTimedOutCount();
                    queuedRequest.Callback(HttpRequestQueueResultCode.RequestTimeout, request, queuedRequest.State);
                    break;

                default:
                    SetOffline();
                    queuedRequest.Callback(HttpRequestQueueResultCode.Error, request, queuedRequest.State);
                    break;
            }
        }

        private void ProcessResponseOffline(AsyncHttpRequest request)
        {
            var queuedRequest = (QueuedRequest) request.State;
            queuedRequest.Callback(HttpRequestQueueResultCode.Offline, null, queuedRequest.State);
        }

        private void ProcessQueuedItems()
        {
            if (queue.Count == 0)
            {
                return;
            }

            // remove all request that timed out already
            var now = DateTime.UtcNow;
            var maxRequestDate = now.Subtract(QueueTimeout);
            while (queue.Count > 0)
            {
                var nextRequest = queue.First.Value;
                if (nextRequest.CreateDate > maxRequestDate)
                {
                    break;
                }

                nextRequest.Callback(HttpRequestQueueResultCode.QueueTimeout, null, nextRequest.State);
                queue.RemoveFirst();
            }

            // execute requests until max concurrent requests are reached
            while (queue.Count > 0 && RunningRequestsCount < MaxConcurrentRequests)
            {
                var nextRequest = queue.First.Value;
                queue.RemoveFirst();
                ExecuteHttpRequest(nextRequest);
            }
        }

        private void SetOffline()
        {
            if (QueueState == HttpRequestQueueState.Offline)
            {
                return;
            }

            QueueState = HttpRequestQueueState.Offline;
            nextReconnectTime = DateTime.UtcNow.Add(ReconnectInterval);

            if (log.IsInfoEnabled)
            {
                log.InfoFormat("Request queue has been set offline");
            }

            foreach (var item in queue)
            {
                item.Callback(HttpRequestQueueResultCode.Offline, null, item.State);
            }

            queue.Clear();
        }

        private void DecrementTimedOutCount()
        {
            if (MaxTimedOutRequests > 0 && TimedOutRequests > 0)
            {
                TimedOutRequests--;
            }
        }

        private void IncrementTimedOutCount()
        {
            if (MaxTimedOutRequests <= 0)
            {
                return;
            }

            TimedOutRequests++;
            if (TimedOutRequests >= MaxTimedOutRequests)
            {
                SetOffline();
            }
        }

        /// <summary>
        ///     Releases the unmanaged resources used by the
        ///     <see cref="HttpRequestQueue" /> and optionally disposes of the
        ///     managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <see langword="true" /> to release both managed and unmanaged
        ///     resources; <see langword="false" /> to releases only unmanaged
        ///     resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing == false)
            {
                return;
            }

            fiber.Dispose();
        }

        private class QueuedRequest
        {
            public readonly DateTime CreateDate = DateTime.UtcNow;

            public HttpWebRequest Request ;

            public HttpRequestQueueCallback Callback ;

            public byte[] PostData ;

            public object State ;
        }
    }
}