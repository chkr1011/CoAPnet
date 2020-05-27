using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Internal
{
    public static class ParallelTask
    {
        public static void Start(Action action, CancellationToken cancellationToken)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            Task.Run(action, cancellationToken);
        }

        public static void Start(Func<Task> function, CancellationToken cancellationToken)
        {
            if (function is null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            Task.Run(function, cancellationToken);
        }

        public static void StartLongRunning(Func<Task> function, CancellationToken cancellationToken)
        {
            if (function is null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            Task.Factory.StartNew(function, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}
