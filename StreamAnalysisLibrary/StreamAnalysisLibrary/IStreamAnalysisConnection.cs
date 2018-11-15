using Apache.NMS;
using System;

namespace StreamAnalysisLibrary
{
    public interface IStreamAnalysisConnection : IDisposable, IStartable, IStoppable
    {
        IStreamAnalysisSession CreateStreamingSession();
    }
};