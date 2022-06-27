using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.Extensions
{
    public static class TaskExtensions
    {
        public static async void AwaitInOtherContext(this Task task, bool returnToCallingContext, Action<Exception> onException = null)
        {
            try
            {
                await task.ConfigureAwait(returnToCallingContext);
            }
            catch (Exception exception)
            {
                if (onException != null)
                    onException(exception);
                else
                {
                    Debug.WriteLine($"Unhandled exception in {nameof(AwaitInOtherContext)}: {exception.Message}");
                    throw;
                }
            }
        }
    }
}
