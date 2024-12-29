using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Model;

namespace FitnessBL.Interfaces
{
    public interface IProgramRepo
    {
        Program GetProgramCode(string programCode);
        bool BestaatProgram(Program program);
        Program AddProgram(Program program);
        void UpdateProgram(Program program);
    }
}
