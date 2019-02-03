using Apache.NMS;
using System;

namespace ApixuCoreLibrary
{
    public class StreamAnalysisSession : IStreamAnalysisSession
    {
        private const string DESTINATION = "queue://StreamAnalysisQueue";

        internal ISession Session { get; set; }

        internal IDestination Destination { get; set; }

        internal IMessageProducer Producer { get; set; }

        internal StreamAnalysisSession(ISession session)
        {
            Session = session;
            try
            {
                Destination = Session.GetDestination(DESTINATION);
                Producer = Session.CreateProducer(Destination);
            }
            catch (Exception ex)
            {
                throw new StreamAnalysisConnectionException($"Failed creating the producer {ex.Message}.");
            }
        }

        /// <summary>
        /// Sends data to Stream Analysis. The object must be serializable to XML.
        /// </summary>
        /// <param name="data"></param>
        public void SendData(object data)
        {
            try
            {
                Producer.Send(data);
            }
            catch (Exception ex)
            {
                throw new StreamAnalysisSendingFailedException($"Failed sending the data: {ex.Message}.");
            }
        }

        /// <summary>
        /// Subscribes to the given stream and sends its data to Stream Analysis. The object must be serializable to XML.
        /// </summary>
        /// <param name="stream"></param>
        public void StreamData(IObservable<object> stream)
        {
            stream.Subscribe(onNext: data => SendData(data), onError: error => throw new StreamAnalysisSendingFailedException($"The stream ecountered an error: {error.Message}"));
        }

        public void Dispose()
        { }
    }
}