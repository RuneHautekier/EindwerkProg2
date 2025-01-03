using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessWPF.Excepitons
{
    public class FitnessClientException : Exception
    {
        public FitnessClientException(string? message, Exception ex)
            : base(message) { }
    }
}
