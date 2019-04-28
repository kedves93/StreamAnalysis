using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Exceptions
{
    public class InvalidRepositoryName : Exception
    {
        public InvalidRepositoryName()
        {
        }

        public InvalidRepositoryName(string message)
            : base(message)
        {
        }

        public InvalidRepositoryName(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
