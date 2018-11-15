using Apache.NMS;
using System;

namespace StreamAnalysisLibrary
{
    public interface IStreamAnalysisConnection : IDisposable, IStartable, IStoppable
    {
        /// <summary>
        /// Creates a new session in order to start streaming data to StreamAnalysis.
        /// </summary>
        /// <returns></returns>
        IStreamAnalysisSession CreateStreamingSession();
    }
};