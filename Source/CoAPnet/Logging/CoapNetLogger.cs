using System;
using System.Collections.Generic;
using System.Threading;

namespace CoAPnet.Logging
{
    public class CoapNetLogger
    {
        readonly List<ICoapNetLoggerSink> _sinks = new List<ICoapNetLoggerSink>();

        int _sinksCount;

        public void Trace(string source, string message, params object[] parameters)
        {
            Log(CoapNetLogMessageLevel.Trace, source, null, message, parameters);
        }

        public void Information(string source, string message, params object[] parameters)
        {
            Log(CoapNetLogMessageLevel.Information, source, null, message, parameters);
        }

        public void Warning(string source, string message, params object[] parameters)
        {
            Log(CoapNetLogMessageLevel.Warning, source, null, message, parameters);
        }

        public void Warning(string source, Exception exception, string message, params object[] parameters)
        {
            Log(CoapNetLogMessageLevel.Warning, source, exception, message, parameters);
        }

        public void Error(string source, string message, params object[] parameters)
        {
            Log(CoapNetLogMessageLevel.Error, source, null, message, parameters);
        }

        public void Error(string source, Exception exception, string message, params object[] parameters)
        {
            Log(CoapNetLogMessageLevel.Error, source, exception, message, parameters);
        }

        public void RegisterSink(ICoapNetLoggerSink sink)
        {
            if (sink is null)
            {
                throw new ArgumentNullException(nameof(sink));
            }

            lock (_sinks)
            {
                _sinks.Add(sink);

                Interlocked.Increment(ref _sinksCount);
            }
        }

        public void UnregisterSink(ICoapNetLoggerSink sink)
        {
            if (sink is null)
            {
                throw new ArgumentNullException(nameof(sink));
            }

            lock (_sinks)
            {
                _sinks.Remove(sink);

                Interlocked.Decrement(ref _sinksCount);
            }
        }

        void Log(CoapNetLogMessageLevel level, string source, Exception exception, string message, params object[] parameters)
        {
            if (_sinksCount == 0)
            {
                return; // Ensure that performance is not decreased when nothing is logged.
            }

            var logMessage = new CoapNetLogMessage
            {
                Timestamp = DateTime.UtcNow, // Use UTC for performance reasons.
                Source = source,
                Level = level,
                Exception = exception,
                Message = message,
                Parameters = parameters,
            };

            lock (_sinks)
            {
                foreach (var sink in _sinks)
                {
                    sink.ProcessLogMessage(logMessage);
                }
            }
        }
    }
}
