
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CMSuniVortex.Tasks
{
    public static class TaskExtension
    {
        public static void SafeContinueWith<TResult>(
            this Task<TResult> @this,
            Action<Task<TResult>> continuationAction,
            CancellationToken cancellationToken = default)
        {
            var context = SynchronizationContext.Current;
            @this.ConfigureAwait(false)
                .GetAwaiter()
                .OnCompleted(() =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (context != null
                        && SynchronizationContext.Current != context)
                    {
                        context.Post(state => continuationAction(@this), null);
                    }
                    else
                    {
                        continuationAction(@this);
                    }
                });
        }
        
        public static void SafeContinueWith(
            this Task @this,
            Action<Task> continuationAction,
            CancellationToken cancellationToken = default)
        {
            var context = SynchronizationContext.Current;
            @this.ConfigureAwait(false)
                .GetAwaiter()
                .OnCompleted(() =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (context != null
                        && SynchronizationContext.Current != context)
                    {
                        context.Post(state => continuationAction(@this), null);
                    }
                    else
                    {
                        continuationAction(@this);
                    }
                });
        }
        
        public static void SafeCancelAndDispose(this CancellationTokenSource @this)
        {
            if (@this == default)
            {
                return;
            }
            
            try
            {
                if (!@this.IsCancellationRequested)
                {
                    @this.Cancel();
                }
                @this.Dispose();
            }
            catch
            {
                // Ignore
            }
        }
    }
}