
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CMSuniVortex
{
    public static class TaskSupport
    {
        public static Task WaitWhileAsync(Func<bool> condition, CancellationToken cancellationToken = default)
            => PlayerLoopWaiter.StartAsync(() => !condition.Invoke(), cancellationToken);
        
        public static Task WaitUntilAsync(Func<bool> condition, CancellationToken cancellationToken = default)
            => PlayerLoopWaiter.StartAsync(condition, cancellationToken);
    }
}