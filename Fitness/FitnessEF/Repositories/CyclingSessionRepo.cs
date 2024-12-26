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
    public class CyclingSessionRepo : ICyclingSessionRepo
    {
        private FitnessContext ctx;

        public CyclingSessionRepo(string connectionString)
        {
            ctx = new FitnessContext(connectionString);
        }

        private void SaveAndClear()
        {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

        public IEnumerable<Cyclingsession> GetCyclingSessions()
        {
            try
            {
                List<CyclingSessionEF> csEFs = ctx
                    .cyclingsession.Select(x => x)
                    .Include(m => m.member)
                    .ToList();

                List<Cyclingsession> cyclingsessions = new();
                foreach (CyclingSessionEF csEF in csEFs)
                {
                    cyclingsessions.Add(MapCyclingSession.MapToDomain(csEF));
                }
                return cyclingsessions;
            }
            catch (Exception ex)
            {
                throw new RepoException("CyclingSessionRepo - GetCyclingSessions");
            }
        }

        public Cyclingsession GetCyclingSessionId(int id)
        {
            try
            {
                CyclingSessionEF csEF = ctx
                    .cyclingsession.Where(x => x.cyclingsession_id == id)
                    .Include(m => m.member)
                    .AsNoTracking()
                    .FirstOrDefault();

                if (csEF == null)
                {
                    return null;
                }
                else
                {
                    return MapCyclingSession.MapToDomain(csEF);
                }
            }
            catch (Exception ex)
            {
                throw new RepoException("CyclingSessionRepo - GetCyclingSessionId");
            }
        }

        public List<Cyclingsession> GetCyclingSessionViaMember(Member member)
        {
            try
            {
                MemberEF memberEF = ctx
                    .members.Where(x => x.first_name == member.FirstName)
                    .Where(x => x.last_name == member.LastName)
                    .AsNoTracking()
                    .FirstOrDefault();

                if (memberEF == null)
                {
                    return null;
                }

                List<CyclingSessionEF> csEFs = ctx
                    .cyclingsession.Where(x => x.member_id == memberEF.member_id)
                    .Include(m => m.member)
                    .AsNoTracking()
                    .ToList();

                if (csEFs.Count() == 0)
                {
                    return null;
                }

                List<Cyclingsession> css = new List<Cyclingsession>();
                foreach (CyclingSessionEF csEF in csEFs)
                {
                    css.Add(MapCyclingSession.MapToDomain(csEF));
                }
                return css;
            }
            catch (Exception ex)
            {
                throw new RepoException("CyclingSessionRepo - GetCyclingSessionId");
            }
        }

        public Cyclingsession AddCyclingSession(Cyclingsession cyclingsession)
        {
            try
            {
                CyclingSessionEF csEF = MapCyclingSession.MapToDB(cyclingsession);
                ctx.cyclingsession.Add(csEF);
                SaveAndClear();
                cyclingsession.Cyclingsession_id = csEF.cyclingsession_id;
                return cyclingsession;
            }
            catch (Exception ex)
            {
                throw new RepoException("CyclingSessionRepo - AddCyclingSession");
            }
        }

        public bool BestaatCyclingSessionAl(Cyclingsession cyclingsession)
        {
            try
            {
                return ctx.cyclingsession.Any(x =>
                    x.date == cyclingsession.Date
                    && x.duration == cyclingsession.Duration
                    && x.member_id == cyclingsession.Member.Member_id
                );
            }
            catch (Exception ex)
            {
                throw new RepoException("CyclingSessionRepo - BestaatCyclingSessionAl");
            }
        }

        public void UpdateCyclingSession(Cyclingsession cs)
        {
            try
            {
                ctx.cyclingsession.Update(MapCyclingSession.MapToDB(cs));
                SaveAndClear();
            }
            catch (Exception ex)
            {
                throw new RepoException("CyclingSessionRepo - UpdateCyclingSession");
            }
        }

        public bool IsCyclingSessionId(int id)
        {
            try
            {
                return ctx.cyclingsession.Any(x => x.cyclingsession_id == id);
            }
            catch (Exception ex)
            {
                throw new RepoException("CyclingSessionRepo - IsCyclingSessionId");
            }
        }

        public void DeleteCyclingSession(int id)
        {
            try
            {
                CyclingSessionEF csEF = ctx.cyclingsession.FirstOrDefault(x =>
                    x.cyclingsession_id == id
                );
                ctx.cyclingsession.Remove(csEF);
                SaveAndClear();
            }
            catch (Exception ex)
            {
                throw new RepoException("CyclingSessionRepo - DeleteCyclingSession");
            }
        }
    }
}
