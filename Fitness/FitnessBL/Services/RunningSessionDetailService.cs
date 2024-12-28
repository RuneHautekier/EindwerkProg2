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
    public class RunningSessionDetailService
    {
        private IRunningSessionDetailRepo rsdRepo;
        private IRunningSessionMainRepo rsmRepo;

        public RunningSessionDetailService(
            IRunningSessionDetailRepo rsdRepo,
            IRunningSessionMainRepo rsmRepo
        )
        {
            this.rsdRepo = rsdRepo;
            this.rsmRepo = rsmRepo;
        }

        public IEnumerable<Runningsession_detail> GetRunningSessionsDetails()
        {
            IEnumerable<Runningsession_detail> rsds = rsdRepo.GetRunningSessionDetails();
            if (rsds.Count() == 0)
                throw new ServiceException(
                    "Er zitten nog geen RunningSessionsDetails in de database!"
                );
            return rsds;
        }

        public IEnumerable<Runningsession_detail> GetRunningSessionDetailsId(int id)
        {
            IEnumerable<Runningsession_detail> rsds = rsdRepo.GetRunningSessionDetailsId(id);
            if (rsds.Count() == 0)
                throw new ServiceException(
                    "RunningSessionDetailsService - GetRunningSessionDetailsId - Er is geen RunningSessionMain met dit id, dus ook geen RunningSessionDetails!"
                );
            return rsds;
        }

        public IEnumerable<Runningsession_detail> AddRunningSessionDetails(
            List<Runningsession_detail> rsds
        )
        {
            if (rsds == null)
                throw new ServiceException(
                    "RunningSessionDetailsService - AddRunningSessionDetails - RunningSessionDetails is null!"
                );
            if (rsds.Count() == 0)
                throw new ServiceException(
                    "RunningSessionDetailsService - AddRunningSessionDetails - Er zitten geen RunningSessionDetails in de lijst!"
                );
            if (rsds.Count() < 2)
                throw new ServiceException(
                    "RunningSessionDetailsService - AddRunningSessionDetails - Je moet minstens 2 Sequenties hebben!"
                );
            if (
                !rsds.All(r =>
                    r.MainSession.Runningsession_id == rsds.First().MainSession.Runningsession_id
                )
            )
                throw new ServiceException(
                    "RunningSessionDetailsService - AddRunningSessionDetails - De details moeten dezelfde MainSessieId hebben!"
                );
            if (!rsmRepo.BestaatRunningSessionMain(rsds.First().MainSession))
                throw new ServiceException(
                    "RunningSessionMainService - AddRunningSessionMain - RunningSessionMain bestaat niet dus kan het ook geen details hebben!"
                );

            rsdRepo.AddRunningSessionDetails(rsds);
            return rsds;
        }
    }
}
