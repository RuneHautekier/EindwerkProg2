using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessBL.Exceptions
{
    public class CyclingSessionException : Exception
    {
        public CyclingSessionException(string? message)
            : base(message) { }

        public CyclingSessionException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
