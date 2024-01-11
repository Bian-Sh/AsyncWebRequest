using System;
using UnityEngine;
using System.Threading;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
public static class WebRequestExtension
{
    static SynchronizationContext context;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Install() => context = SynchronizationContext.Current;
    public static WebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp) => new(asyncOp);
    public struct WebRequestAwaiter : INotifyCompletion
    {
        Action continuation;
        readonly UnityWebRequestAsyncOperation asyncOp;
        public readonly bool IsCompleted => asyncOp.isDone;
        public readonly void GetResult() { }
        public WebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
        {
            this.asyncOp = asyncOp;
            continuation = null;
        }
        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
            if (asyncOp.isDone)
            {
                Post();
            }
            else
            {
                asyncOp.completed += OnAsyncOpComplete;
            }
        }
        private readonly void Post()
        {
            if (Thread.CurrentThread.ManagedThreadId == 1)
            {
                continuation?.Invoke();
            }
            else
            {
                context.Post(Callback, null);
            }
        }
        readonly void Callback(object state) => continuation?.Invoke();
        readonly void OnAsyncOpComplete(AsyncOperation obj)
        {
            asyncOp.completed -= OnAsyncOpComplete;
            Post();
        }
    }
}
