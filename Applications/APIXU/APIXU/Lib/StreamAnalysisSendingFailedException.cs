﻿using System;

namespace Apixu
{
    [Serializable]
    public class StreamAnalysisSendingFailedException : Exception
    {
        public StreamAnalysisSendingFailedException()
        { }

        public StreamAnalysisSendingFailedException(string message) : base(message)
        { }

        public StreamAnalysisSendingFailedException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}