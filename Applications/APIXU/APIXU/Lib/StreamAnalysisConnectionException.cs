﻿using System;

namespace Apixu
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