
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace CMSuniVortex.Tasks
{
    public static class UnityWebRequestAsyncOperationExtensions
    {
        public static TaskAwaiter<UnityWebRequest> GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
            => asyncOp.ToTask().GetAwaiter();

        public static Task<UnityWebRequest> ToTask(this UnityWebRequestAsyncOperation asyncOp)
        {
            var source = new TaskCompletionSource<UnityWebRequest>();

            asyncOp.completed += operation =>
            {
                var webRequest = ((UnityWebRequestAsyncOperation) operation).webRequest;
                if (webRequest.result
                    is UnityWebRequest.Result.ConnectionError
                    or UnityWebRequest.Result.ProtocolError)
                {
                    source.SetException(new HttpRequestException(webRequest.error));
                }
                else
                {
                    source.SetResult(webRequest);
                }
            };

            return source.Task;
        }

        public static IEnumerator AsIEnumerator(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
            
            if (task.IsFaulted)
            {
                throw task.Exception ?? new System.Exception("Task faulted with no exception.");
            }
        }
        
        public static IEnumerator AsIEnumerator(this IEnumerable<Task> tasks)
        {
            var task = Task.WhenAll(tasks);
            yield return task.AsIEnumerator();
        }
    }
}