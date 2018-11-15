using Apache.NMS;
using System;

namespace StreamAnalysisLibrary
{
    public class StreamAnalysisConnection : IStreamAnalysisConnection
    {
        public bool IsStarted { get; private set; }

        internal IConnection Connection { get; set; }

        internal StreamAnalysisConnection(IConnection connection)
        {
            Connection = connection;
            IsStarted = false;
        }

        public void Start()
        {
            Connection.Start();
            IsStarted = Connection.IsStarted;
        }

        public void Stop()
        {
            Connection.Stop();
            IsStarted = Connection.IsStarted;
        }

        public IStreamAnalysisSession CreateStreamingSession()
        {
            try
            {
                ISession session = this.Connection.CreateSession();
                return new StreamAnalysisSession(session);
            }
            catch (Exception ex)
            {
                throw new StreamAnalysisConnectionException($"Creating the session failed {ex.Message}.");
            }
        }

        public void Dispose()
        { }
    }
}