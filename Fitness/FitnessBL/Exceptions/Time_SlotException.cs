using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessBL.Exceptions
{
    public class Time_SlotException : Exception
    {
        public Time_SlotException(string? message)
            : base(message) { }
    }
}
