using CoAPnet.Protocol;
using System;
using System.Collections.Concurrent;

namespace CoAPnet.MessageDispatcher
{
    public sealed class CoapMessageDispatcher
    {
        readonly ConcurrentDictionary<ushort, CoapMessageAwaiter> _awaiters = new ConcurrentDictionary<ushort, CoapMessageAwaiter>();

        public void Dispatch(Exception exception)
        {
            if (exception is null) throw new ArgumentNullException(nameof(exception));

            foreach (var awaiter in _awaiters)
            {
                awaiter.Value.Fail(exception);
            }

            _awaiters.Clear();
        }

        public void Dispatch(CoapMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (_awaiters.TryRemove(message.Id, out var awaiter))
            {
                awaiter.Complete(message);
                return;
            }

            // TODO: Process messages which are sent out of request/reply pattern.
        }

        public void Reset()
        {
            foreach (var awaiter in _awaiters)
            {
                awaiter.Value.Cancel();
            }

            _awaiters.Clear();
        }

        public CoapMessageAwaiter AddAwaiter(ushort messageId)
        {
            var awaiter = new CoapMessageAwaiter(messageId, this);

            if (!_awaiters.TryAdd(messageId, awaiter))
            {
                throw new InvalidOperationException($"The message dispatcher already has an awaiter for message with ID {messageId}.");
            }

            return awaiter;
        }

        public void RemoveAwaiter(ushort messageId)
        {
            _awaiters.TryRemove(messageId, out _);
        }
    }
}