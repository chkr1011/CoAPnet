using CoAPnet.Exceptions;
using CoAPnet.Protocol;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.MessageDispatcher
{
    public sealed class CoapMessageAwaiter : IDisposable
    {
        readonly TaskCompletionSource<CoapMessage> _taskCompletionSource;
        readonly ushort _messageId;
        readonly CoapMessageDispatcher _owningMessageDispatcher;

        public CoapMessageAwaiter(ushort messageId, CoapMessageDispatcher owningMessageDispatcher)
        {
            _messageId = messageId;
            _owningMessageDispatcher = owningMessageDispatcher ?? throw new ArgumentNullException(nameof(owningMessageDispatcher));
#if NET452
            _taskCompletionSource = new TaskCompletionSource<CoapMessage>();
#else
            _taskCompletionSource = new TaskCompletionSource<CoapMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
#endif
        }

        public async Task<CoapMessage> WaitOneAsync(TimeSpan timeout)
        {
            using (var timeoutToken = new CancellationTokenSource(timeout))
            {
                timeoutToken.Token.Register(() => Fail(new CoapCommunicationTimedOutException()));

                return await _taskCompletionSource.Task.ConfigureAwait(false);
            }
        }

        public void Complete(CoapMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

#if NET452
            // To prevent deadlocks it is required to call the _TrySetResult_ method
            // from a new thread because the awaiting code will not(!) be executed in
            // a new thread automatically (due to await). Furthermore _this_ thread will
            // do it. But _this_ thread is also reading incoming packets -> deadlock.
            // NET452 does not support RunContinuationsAsynchronously
            Task.Run(() => _taskCompletionSource.TrySetResult(message));
#else
            _taskCompletionSource.TrySetResult(message);
#endif
        }

        public void Fail(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));

#if NET452
            // To prevent deadlocks it is required to call the _TrySetResult_ method
            // from a new thread because the awaiting code will not(!) be executed in
            // a new thread automatically (due to await). Furthermore _this_ thread will
            // do it. But _this_ thread is also reading incoming packets -> deadlock.
            // NET452 does not support RunContinuationsAsynchronously
            Task.Run(() => _taskCompletionSource.TrySetException(exception));
#else
            _taskCompletionSource.TrySetException(exception);
#endif
        }

        public void Cancel()
        {
#if NET452
            // To prevent deadlocks it is required to call the _TrySetResult_ method
            // from a new thread because the awaiting code will not(!) be executed in
            // a new thread automatically (due to await). Furthermore _this_ thread will
            // do it. But _this_ thread is also reading incoming packets -> deadlock.
            // NET452 does not support RunContinuationsAsynchronously
            Task.Run(() => _taskCompletionSource.TrySetCanceled());
#else
            _taskCompletionSource.TrySetCanceled();
#endif
        }

        public void Dispose()
        {
            _owningMessageDispatcher.AddAwaiter(_messageId);
        }
    }
}