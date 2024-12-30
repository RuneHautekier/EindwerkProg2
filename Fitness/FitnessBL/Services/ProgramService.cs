using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Exceptions;
using FitnessBL.Interfaces;
using FitnessBL.Model;

namespace FitnessBL.Services
{
    public class ProgramService
    {
        private IProgramRepo programRepo;

        public ProgramService(IProgramRepo programRepo)
        {
            this.programRepo = programRepo;
        }

        public Program GetProgramCode(string programCode)
        {
            Program program = programRepo.GetProgramCode(programCode);
            if (program == null)
                throw new ServiceException(
                    "ProgramService - GetProgramId - Er is geen Program met deze programCode!"
                );
            return program;
        }

        public Program AddProgram(Program program)
        {
            if (program == null)
                throw new ServiceException("ProgramService - AddProgram - Program is null");
            if (programRepo.BestaatProgram(program))
                throw new ServiceException(
                    "ProgramService - AddProgram - Dit programma bestaat al (zelfde programmaCode)"
                );
            programRepo.AddProgram(program);
            return program;
        }

        public Program UpdateProgram(Program program)
        {
            if (program == null)
                throw new ServiceException("ProgramService - UpdateProgram - Program is null!");
            if (!programRepo.BestaatProgram(program))
                throw new ServiceException(
                    "ProgramService - UpdateProgram - Program bestaat niet met deze programCode!"
                );

            programRepo.UpdateProgram(program);
            return program;
        }
    }
}
