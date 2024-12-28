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
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.EntityFrameworkCore;

namespace FitnessEF.Repositories
{
    public class RunningSessionDetailRepo : IRunningSessionDetailRepo
    {
        private FitnessContext ctx;

        public RunningSessionDetailRepo(string connectionString)
        {
            ctx = new FitnessContext(connectionString);
        }

        private void SaveAndClear()
        {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

        public IEnumerable<Runningsession_detail> GetRunningSessionDetails()
        {
            try
            {
                List<Runningsession_detailEF> rsdEFs = ctx
                    .runningsession_detail.Select(x => x)
                    .Include(r => r.MainSession)
                    .ThenInclude(m => m.Member)
                    .ToList();
                List<Runningsession_detail> rsds = new();
                foreach (Runningsession_detailEF rsdEF in rsdEFs)
                {
                    rsds.Add(MapRunningSessionDetail.MapToDomain(rsdEF));
                }
                return rsds;
            }
            catch (Exception ex)
            {
                throw new RepoException("RunningSessionDetailRepo - GetRunningSessionsDetails");
            }
        }

        public IEnumerable<Runningsession_detail> GetRunningSessionDetailsId(int id)
        {
            try
            {
                IEnumerable<Runningsession_detailEF> rsdEFs = ctx
                    .runningsession_detail.Where(x => x.runningsession_id == id)
                    .Include(r => r.MainSession)
                    .ThenInclude(m => m.Member)
                    .AsNoTracking()
                    .ToList();

                if (rsdEFs == null)
                {
                    return null;
                }

                List<Runningsession_detail> rsds = new List<Runningsession_detail>();
                foreach (Runningsession_detailEF rsdEF in rsdEFs)
                {
                    Runningsession_detail rsd = new Runningsession_detail(
                        MapRunningSessionMain.MapToDomain(rsdEF.MainSession),
                        rsdEF.seq_nr,
                        rsdEF.interval_time,
                        rsdEF.interval_speed
                    );
                    rsds.Add(rsd);
                }

                return rsds;
            }
            catch (Exception ex)
            {
                throw new RepoException("RunningSessionDetailRepo - GetRunningSessionDetailsId");
            }
        }

        public List<Runningsession_detail> AddRunningSessionDetails(
            List<Runningsession_detail> rsds
        )
        {
            try
            {
                foreach (Runningsession_detail rsd in rsds)
                {
                    Runningsession_detailEF rsdEF = MapRunningSessionDetail.MapToDB(rsd);
                    ctx.runningsession_detail.Add(rsdEF);
                    SaveAndClear();
                }

                return rsds;
            }
            catch (Exception ex)
            {
                throw new RepoException("RunningSessionDetailsRepo - AddRunningSessionDetails", ex);
            }
        }
    }
}
