using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace c971_project.Extensions
{
    public static class TaskExtensions
    {
        public static void SafeFireAndForget(this Task task, bool returnToCallingContext = true, Action<Exception>? onException = null) // ← ADD ? after Action<Exception>
        {
            task.ContinueWith(t =>
            {
                if (t.IsFaulted && onException != null)
                    onException(t.Exception);

            }, returnToCallingContext ? TaskScheduler.FromCurrentSynchronizationContext() : TaskScheduler.Default);
        }
    }
}