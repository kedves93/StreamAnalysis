using Apache.NMS;
using System;

namespace StreamAnalysisLibrary
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

        public void Dispose()
        { }
    }
}