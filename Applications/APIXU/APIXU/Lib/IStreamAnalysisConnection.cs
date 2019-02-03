using System;

namespace Apixu
{
    public interface IStreamAnalysisConnection : IDisposable, IStartable, IStoppable
    {
        /// <summary>
        /// Creates a new session in order to start streaming data to Stream Analysis.
        /// </summary>
        /// <returns></returns>
        IStreamAnalysisSession CreateStreamingSession();
    }
};