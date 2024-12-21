using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessEF.Exceptions
{
    public class MapKlantException : Exception
    {
        public MapKlantException(string? message)
            : base(message) { }

        public MapKlantException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
