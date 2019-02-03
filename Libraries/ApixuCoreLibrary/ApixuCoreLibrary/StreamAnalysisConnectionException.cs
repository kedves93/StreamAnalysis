using System;

namespace ApixuCoreLibrary
{
    [Serializable]
    public class StreamAnalysisConnectionException : Exception
    {
        public StreamAnalysisConnectionException()
        { }

        public StreamAnalysisConnectionException(string message) : base(message)
        { }

        public StreamAnalysisConnectionException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}