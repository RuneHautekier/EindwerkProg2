using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessBL.Exceptions
{
    public class MemberException : Exception
    {
        public MemberException(string? message)
            : base(message) { }
    }
}
