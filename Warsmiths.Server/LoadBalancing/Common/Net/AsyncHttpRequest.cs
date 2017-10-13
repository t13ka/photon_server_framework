using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Warsmiths.Server.Common.Net
{
    public class AsyncHttpRequest
    {
        private readonly Action<AsyncHttpRequest> callBackAction;
        public readonly HttpWebRequest WebRequest;

        private IAsyncResult asyncResult;

        private byte[] postContent;

        private byte[] readBuffer;

        private int status;

        private RegisteredWaitHandle timeoutWaitHandle;

        private Stream webResponseStream;

        public AsyncHttpRequest(HttpWebRequest webRequest, Action<AsyncHttpRequest> callBack)
            : this(webRequest, callBack, null)
        {
        }

        public AsyncHttpRequest(HttpWebRequest webRequest, Action<AsyncHttpRequest> callBack, object state)
        {
            WebRequest = webRequest;
            callBackAction = callBack;
            State = state;

            ReadBufferSize = 1024;
        }

        public int ReadBufferSize ;

        public HttpWebResponse WebResponse { get; private set; }

        public MemoryStream ResponseStream { get; private set; }

        public object State ;

        public AsyncHttpRequestStatus Status
        {
            get { return (AsyncHttpRequestStatus) status; }
        }

        public WebExceptionStatus WebStatus { get; private set; }

        /// <summary>
        ///     Gets the Exception that caused the request to end prematurely. If
        ///     the request completed successfully, this will return null.
        /// </summary>
        public Exception Exception { get; private set; }

        public void GetAsync()
        {
            asyncResult = WebRequest.BeginGetResponse(GetResponseCallBack, null);
            timeoutWaitHandle = ThreadPool.UnsafeRegisterWaitForSingleObject(asyncResult.AsyncWaitHandle,
                TimeoutCallback, null, WebRequest.Timeout, true);
        }

        public void PostAsync(byte[] content)
        {
            postContent = content;
            asyncResult = WebRequest.BeginGetRequestStream(GetRequestStreamCallback, null);
            timeoutWaitHandle = ThreadPool.UnsafeRegisterWaitForSingleObject(asyncResult.AsyncWaitHandle,
                TimeoutCallback, null, WebRequest.Timeout, true);
        }

        public bool Cancel()
        {
            if (!TrySetStatus(AsyncHttpRequestStatus.Running, AsyncHttpRequestStatus.Canceled))
            {
                return false;
            }

            WebRequest.Abort();
            UnregisterTimeoutWaitHandle();
            WebStatus = WebExceptionStatus.RequestCanceled;
            return true;
        }

        private void GetRequestStreamCallback(IAsyncResult ar)
        {
            try
            {
                if (!TrySetStatus(AsyncHttpRequestStatus.Running, AsyncHttpRequestStatus.Running))
                {
                    return;
                }

                UnregisterTimeoutWaitHandle();

                var requestStream = WebRequest.EndGetRequestStream(ar);
                requestStream.Write(postContent, 0, postContent.Length);
                requestStream.Flush();
                requestStream.Close();
                requestStream.Dispose();

                asyncResult = WebRequest.BeginGetResponse(GetResponseCallBack, null);
                timeoutWaitHandle = ThreadPool.UnsafeRegisterWaitForSingleObject(asyncResult.AsyncWaitHandle,
                    TimeoutCallback, null, WebRequest.Timeout, true);
            }
            catch (Exception ex)
            {
                SetException(ex);
            }
        }

        private void GetResponseCallBack(IAsyncResult ar)
        {
            try
            {
                if (!TrySetStatus(AsyncHttpRequestStatus.Running, AsyncHttpRequestStatus.Completed))
                {
                    return;
                }

                UnregisterTimeoutWaitHandle();

                WebResponse = (HttpWebResponse) WebRequest.EndGetResponse(ar);
                WebStatus = WebExceptionStatus.Success;

                webResponseStream = WebResponse.GetResponseStream();

                var len = WebResponse.ContentLength == -1 ? ReadBufferSize : (int) WebResponse.ContentLength;
                ResponseStream = new MemoryStream(len);
                readBuffer = new byte[ReadBufferSize];
                webResponseStream.BeginRead(readBuffer, 0, readBuffer.Length, ReadCallback, null);
            }
            catch (Exception ex)
            {
                SetException(ex);
            }
        }

        private void UnregisterTimeoutWaitHandle()
        {
            if (timeoutWaitHandle != null)
            {
                timeoutWaitHandle.Unregister(asyncResult.AsyncWaitHandle);
                timeoutWaitHandle = null;
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                var bytesRead = webResponseStream.EndRead(ar);
                if (bytesRead <= 0)
                {
                    ResponseStream.Position = 0;
                    EndRequest();
                    return;
                }

                ResponseStream.Write(readBuffer, 0, bytesRead);
                webResponseStream.BeginRead(readBuffer, 0, readBuffer.Length, ReadCallback, null);
            }
            catch (Exception ex)
            {
                SetException(ex);
            }
        }

        private void TimeoutCallback(object state, bool timedOut)
        {
            try
            {
                if (timedOut == false)
                {
                    return;
                }

                if (!TrySetStatus(AsyncHttpRequestStatus.Running, AsyncHttpRequestStatus.Faulted))
                {
                    return;
                }

                WebRequest.Abort();
                SetException(new WebException("The operation has timed out", WebExceptionStatus.Timeout));
            }
            catch (Exception ex)
            {
                SetException(ex);
            }
        }

        private void SetStatus(AsyncHttpRequestStatus newStatus)
        {
            Interlocked.Exchange(ref status, (int) newStatus);
        }

        private bool TrySetStatus(AsyncHttpRequestStatus oldStatus, AsyncHttpRequestStatus newStatus)
        {
            return Interlocked.CompareExchange(ref status, (int) newStatus, (int) oldStatus) == (int) oldStatus;
        }

        private void SetException(Exception ex)
        {
            SetStatus(AsyncHttpRequestStatus.Faulted);
            Exception = ex;
            WebResponse = null;
            ResponseStream = null;

            var webException = ex as WebException;
            if (webException != null)
            {
                WebStatus = webException.Status;
            }
            else
            {
                WebStatus = WebExceptionStatus.UnknownError;
            }

            EndRequest();
        }

        private void EndRequest()
        {
            Cleanup();
            callBackAction(this);
        }

        private void Cleanup()
        {
            if (webResponseStream != null)
            {
                webResponseStream.Dispose();
                webResponseStream = null;
            }

            if (WebResponse != null)
            {
                WebResponse.Close();
            }
        }
    }
}