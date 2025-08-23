using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace c971_project.Extensions
{
    public static class TaskExtensions
    {
        public static void SafeFireAndForget(this Task task, bool returnToCallingContext = true, Action<Exception> onException = null)
        {
            task.ContinueWith(t =>
            {
                if (t.IsFaulted && onException != null)
                    onException(t.Exception);

            }, returnToCallingContext ? TaskScheduler.FromCurrentSynchronizationContext() : TaskScheduler.Default);
        }
    }
}
// custom extension method that provides a safe way to call async methods without
// awaiting them, while also handling any exceptions that might occur.