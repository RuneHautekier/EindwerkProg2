using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessEF.Exceptions
{
    public class RepoException : Exception
    {
        public RepoException(string? message)
            : base(message) { }

        public RepoException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
