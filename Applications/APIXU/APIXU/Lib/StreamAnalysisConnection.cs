﻿using Apache.NMS;
using System;

namespace Apixu
{
    public class StreamAnalysisConnection : IStreamAnalysisConnection
    {
        /// <summary>
        /// Determines if the connection to Stream Analysis has started.
        /// </summary>
        public bool IsStarted { get; private set; }

        internal IConnection Connection { get; set; }

        internal StreamAnalysisConnection(IConnection connection)
        {
            Connection = connection;
            IsStarted = false;
        }

        /// <summary>
        /// Starts the connection to Stream Analysis.
        /// </summary>
        public void Start()
        {
            Connection.Start();
            IsStarted = Connection.IsStarted;
        }

        /// <summary>
        /// Stops the connection to Stream Analysis.
        /// </summary>
        public void Stop()
        {
            Connection.Stop();
            IsStarted = Connection.IsStarted;
        }

        /// <summary>
        /// Creates a new session in order to start streaming data to Stream Analysis.
        /// </summary>
        /// <returns></returns>
        public IStreamAnalysisSession CreateStreamingSession(string destination)
        {
            try
            {
                ISession session = Connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
                return new StreamAnalysisSession(destination, session);
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