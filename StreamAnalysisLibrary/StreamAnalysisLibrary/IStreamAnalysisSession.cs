using System;

namespace StreamAnalysisLibrary
{
    public interface IStreamAnalysisSession : IDisposable
    {
        void SendData(object data);
    }
}