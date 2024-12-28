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
        private IMemberRepo memberRepo;

        public ProgramService(IProgramRepo programRepo, IMemberRepo memberRepo)
        {
            this.programRepo = programRepo;
            this.memberRepo = memberRepo;
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

        public IEnumerable<Program> GetProgramListMember(Member member)
        {
            if (member == null)
                throw new ServiceException(
                    "ProgramService - GetProgramListMember - Member is null"
                );
            if (!memberRepo.IsMemberId(member))
                throw new ServiceException(
                    "ProgramService - GetProgramListMember - Er bestaat geen member met dit id!"
                );

            IEnumerable<Program> programList = new List<Program>();
            programList = programRepo.GetProgramListMember(member);
            if (!programList.Any())
                throw new ServiceException(
                    "ProgramService - GetProgramListMember - Deze member is nog voor geen enkel Program ingeschreven!"
                );
            return programList;
        }
    }
}
