using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Internal
{
    public static class ParallelTask
    {
        public static void Run(Action action, CancellationToken cancellationToken)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));

            Task.Run(action, cancellationToken);
        }

        public static void Run(Func<Task> function, CancellationToken cancellationToken)
        {
            if (function is null) throw new ArgumentNullException(nameof(function));

            Task.Run(function);
        }
    }
}
