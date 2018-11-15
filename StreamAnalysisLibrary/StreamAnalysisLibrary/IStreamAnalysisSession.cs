using System;

namespace StreamAnalysisLibrary
{
    public interface IStreamAnalysisSession : IDisposable
    {
        /// <summary>
        /// Sends data to StreamAnalysis. The object must be serializable to XML.
        /// </summary>
        /// <param name="data"></param>
        void SendData(object data);
    }
}