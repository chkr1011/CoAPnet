namespace CoAPnet.Logging
{
    public interface ICoapNetLoggerSink
    {
        void ProcessLogMessage(CoapNetLogMessage logMessage);
    }
}
