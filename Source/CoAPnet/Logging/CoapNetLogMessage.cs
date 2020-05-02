using System;

namespace CoAPnet.Logging
{
    public class CoapNetLogMessage
    {
        public DateTime Timestamp
        {
            get; set;
        }

        public CoapNetLogMessageLevel Level
        {
            get; set;
        }

        public string Source
        {
            get; set;
        }

        public Exception Exception
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        public object[] Parameters
        {
            get; set;
        }

        public int ThreadId
        {
            get; set;
        }
    }
}
