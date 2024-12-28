using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessBL.Exceptions
{
    public class RunningSessionDetailException : Exception
    {
        public RunningSessionDetailException(string? message)
            : base(message) { }
    }
}
