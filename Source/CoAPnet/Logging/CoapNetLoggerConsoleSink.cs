using System;

namespace CoAPnet.Logging
{
    public sealed class CoapNetLoggerConsoleSink : ICoapNetLoggerSink
    {
        public void ProcessLogMessage(CoapNetLogMessage logMessage)
        {
            if (logMessage is null)
            {
                throw new ArgumentNullException(nameof(logMessage));
            }

            var formattedMessage = logMessage.Message;
            if (logMessage.Parameters != null)
            {
                formattedMessage = string.Format(logMessage.Message, logMessage.Parameters);
            }

            Console.WriteLine($"[{logMessage.Timestamp}] [{logMessage.ThreadId}] [{logMessage.Level}] [{formattedMessage}]");
            if (logMessage.Exception != null)
            {
                Console.WriteLine("[\r\n" + logMessage.Exception + "\r\n]");
            }
        }
    }
}