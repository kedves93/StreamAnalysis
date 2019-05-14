using System;

namespace WebApplication.Exceptions
{
    public class InexistentCluster : Exception
    {
        public InexistentCluster()
        {
        }

        public InexistentCluster(string message)
            : base(message)
        {
        }

        public InexistentCluster(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}