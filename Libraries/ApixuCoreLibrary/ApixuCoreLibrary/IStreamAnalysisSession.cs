using System;

namespace ApixuCoreLibrary
{
    public interface IStreamAnalysisSession : IDisposable
    {
        /// <summary>
        /// Sends data to Stream Analysis. The object must be serializable to XML.
        /// </summary>
        /// <param name="data"></param>
        void SendData(object data);

        /// <summary>
        /// Subscribes to the given stream and sends its data to Stream Analysis. The object must be serializable to XML.
        /// </summary>
        /// <param name="stream"></param>
        void StreamData(IObservable<object> stream);
    }
}