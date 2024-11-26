using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessBL.Exceptions
{
    public class ProgrammaException : Exception
    {
        public ProgrammaException(string? message) : base(message)
        {
        }
    }
}
