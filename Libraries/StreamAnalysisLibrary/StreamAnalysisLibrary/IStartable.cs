﻿namespace StreamAnalysisLibrary
{
    public interface IStartable
    {
        bool IsStarted { get; }

        void Start();
    }
}