using System;

namespace WebApplication.Exceptions
{
    public class InexistentTaskDefinition : Exception
    {
        public InexistentTaskDefinition()
        {
        }

        public InexistentTaskDefinition(string message)
            : base(message)
        {
        }

        public InexistentTaskDefinition(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}