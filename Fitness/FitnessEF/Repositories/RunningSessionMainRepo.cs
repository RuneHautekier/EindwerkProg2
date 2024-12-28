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
    public class RunningSessionMainRepo : IRunningSessionMainRepo
    {
        private FitnessContext ctx;

        public RunningSessionMainRepo(string connectionString)
        {
            ctx = new FitnessContext(connectionString);
        }

        private void SaveAndClear()
        {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

        public IEnumerable<Runningsession_main> GetRunningSessionsMain()
        {
            try
            {
                List<Runningsession_mainEF> rsmEFs = ctx
                    .runningsession_main.Select(x => x)
                    .Include(m => m.Member)
                    .ToList();
                List<Runningsession_main> rsms = new();
                foreach (Runningsession_mainEF rsmEF in rsmEFs)
                {
                    rsms.Add(MapRunningSessionMain.MapToDomain(rsmEF));
                }
                return rsms;
            }
            catch (Exception ex)
            {
                throw new RepoException("RunningSessionMainRepo - GetRunningSessionsMain");
            }
        }

        public Runningsession_main GetRunningSessionMainId(int id)
        {
            try
            {
                Runningsession_mainEF rsmEF = ctx
                    .runningsession_main.Where(x => x.runningsession_id == id)
                    .Include(m => m.Member)
                    .AsNoTracking()
                    .FirstOrDefault();

                if (rsmEF == null)
                {
                    return null;
                }
                else
                {
                    return MapRunningSessionMain.MapToDomain(rsmEF);
                }
            }
            catch (Exception ex)
            {
                throw new RepoException("RunningSessionMainRepo - GetRunningSessionMainId");
            }
        }

        public IEnumerable<Runningsession_main> GetRunningSessionMember(Member member)
        {
            try
            {
                List<Runningsession_mainEF> rsmEFs = ctx
                    .runningsession_main.Where(x => x.member_id == member.Member_id)
                    .Include(m => m.Member)
                    .ToList();

                if (rsmEFs.Count == 0)
                {
                    return null;
                }

                List<Runningsession_main> rsms = new();
                foreach (Runningsession_mainEF rsmEF in rsmEFs)
                {
                    rsms.Add(MapRunningSessionMain.MapToDomain(rsmEF));
                }
                return rsms;
            }
            catch (Exception ex)
            {
                throw new RepoException("RunningSessionMainRepo - GetRunningSessionMainId");
            }
        }

        public Runningsession_main AddRunningSessionMain(Runningsession_main rsm)
        {
            try
            {
                Runningsession_mainEF rsmEF = MapRunningSessionMain.MapToDB(rsm);
                ctx.runningsession_main.Add(rsmEF);
                SaveAndClear();
                rsm.Runningsession_id = rsmEF.runningsession_id;
                return rsm;
            }
            catch (Exception ex)
            {
                throw new RepoException("RunningSessionMainRepo - AddRunningSessionMain", ex);
            }
        }

        public bool BestaatRunningSessionMain(Runningsession_main rsm)
        {
            try
            {
                return ctx.runningsession_main.Any(x =>
                    x.runningsession_id == rsm.Runningsession_id
                );
            }
            catch (Exception ex)
            {
                throw new RepoException("RunningSessionMainRepo - BestaatRunningSessionMainAl");
            }
        }

        public bool BestaatRunningSessionMainAl(Runningsession_main rsm)
        {
            try
            {
                return ctx.runningsession_main.Any(x =>
                    x.member_id == rsm.Member.Member_id && x.date == rsm.Date
                );
            }
            catch (Exception ex)
            {
                throw new RepoException("RunningSessionMainRepo - BestaatRunningSessionMainAl");
            }
        }

        public void UpdateRunningSessionMain(Runningsession_main rsm)
        {
            try
            {
                ctx.runningsession_main.Update(MapRunningSessionMain.MapToDB(rsm));
                SaveAndClear();
            }
            catch (Exception ex)
            {
                throw new RepoException("RunningSessionMainRepo - UpdateRunningSessionMain");
            }
        }

        public void DeleteRunningSessionMain(Runningsession_main rsm)
        {
            try
            {
                Runningsession_mainEF rsmEF = ctx.runningsession_main.FirstOrDefault(x =>
                    x.runningsession_id == rsm.Runningsession_id
                );
                ctx.runningsession_main.Remove(rsmEF);
                SaveAndClear();
            }
            catch (Exception ex)
            {
                throw new RepoException("RunningSessionMainRepo - DeleteRunningSessionMain");
            }
        }
    }
}
