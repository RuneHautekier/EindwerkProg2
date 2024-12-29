using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Interfaces;
using FitnessBL.Model;
using FitnessEF.Exceptions;
using FitnessEF.Mappers;
using FitnessEF.Model;
using Microsoft.EntityFrameworkCore;

namespace FitnessEF.Repositories
{
    public class ProgramRepo : IProgramRepo
    {
        private FitnessContext ctx;

        public ProgramRepo(string connectionString)
        {
            ctx = new FitnessContext(connectionString);
        }

        private void SaveAndClear()
        {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

        public Program GetProgramCode(string programCode)
        {
            try
            {
                ProgramEF pEF = ctx
                    .program.Where(x => x.programCode.Equals(programCode))
                    .AsNoTracking()
                    .FirstOrDefault();

                if (pEF == null)
                {
                    return null;
                }
                else
                {
                    return MapProgram.MapToDomain(pEF);
                }
            }
            catch (Exception ex)
            {
                throw new RepoException("ProgramRepo - GetProgramId", ex);
            }
        }

        public bool BestaatProgram(Program program)
        {
            try
            {
                return ctx.program.Any(p => p.programCode == program.ProgramCode);
            }
            catch (Exception ex)
            {
                throw new RepoException("ProgramRepo - bestaatProgram");
            }
        }

        public Program AddProgram(Program program)
        {
            try
            {
                ProgramEF pEF = MapProgram.MapToDB(program);
                ctx.program.Add(pEF);
                SaveAndClear();
                return program;
            }
            catch (Exception ex)
            {
                throw new RepoException("ProgramRepo - AddProgram", ex);
            }
        }

        public void UpdateProgram(Program program)
        {
            try
            {
                ctx.program.Update(MapProgram.MapToDB(program));
                SaveAndClear();
            }
            catch (Exception ex)
            {
                throw new RepoException("ProgramRepo - UpdateProgram");
            }
        }
    }
}
