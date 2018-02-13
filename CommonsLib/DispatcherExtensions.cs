using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CommonsLib
{
    public static class DispatcherExtensions
    {
        public static TaskScheduler ToTaskSchedulerAsync(this Dispatcher dispatcher)
        {
            var result = TaskScheduler.Current;
            dispatcher.Invoke(
                DispatcherPriority.Background,
                new Action(() => result = TaskScheduler.FromCurrentSynchronizationContext()));

            return result;
        }
    }
}