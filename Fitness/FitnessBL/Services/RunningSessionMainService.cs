using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Exceptions;
using FitnessBL.Interfaces;
using FitnessBL.Model;

namespace FitnessBL.Services
{
    public class RunningSessionMainService
    {
        private IRunningSessionMainRepo rsmRepo;

        public RunningSessionMainService(IRunningSessionMainRepo rsmRepo)
        {
            this.rsmRepo = rsmRepo;
        }

        public IEnumerable<Runningsession_main> GetRunningSessionsMain()
        {
            IEnumerable<Runningsession_main> rsms = rsmRepo.GetRunningSessionsMain();
            if (rsms.Count() == 0)
                throw new ServiceException(
                    "Er zitten nog geen RunningSessionsMain in de database!"
                );
            return rsms;
        }

        public Runningsession_main GetRunningSessionMainId(int id)
        {
            Runningsession_main rsm = rsmRepo.GetRunningSessionMainId(id);
            if (rsm == null)
                throw new ServiceException(
                    "RunningSessionMainService - GetRunningSessionMainId - Er is geen RunningSessionMain met dit id!"
                );
            return rsm;
        }

        public IEnumerable<Runningsession_main> GetRunningSessionMainMember(Member member)
        {
            IEnumerable<Runningsession_main> rsms = rsmRepo.GetRunningSessionMember(member);
            if (rsms == null)
                throw new ServiceException(
                    "RunningSessionMainService - GetRunningSessionMainMember - Deze Member heeft nog geen RunningSessions!"
                );
            return rsms;
        }

        public Runningsession_main AddRunningSessionMain(Runningsession_main rsm)
        {
            if (rsm == null)
                throw new ServiceException(
                    "RunningSessionMainService - AddRunningSessionMain - RunningSessionMain is null"
                );
            if (rsmRepo.BestaatRunningSessionMainAl(rsm))
                throw new ServiceException(
                    "RunningSessionMainService - AddRunningSessionMain - RunningSessionMain bestaat al (zelfde persoon, zelfde datum+tijdstip)!"
                );

            rsmRepo.AddRunningSessionMain(rsm);
            return rsm;
        }

        public Runningsession_main UpdateRunningSessionMain(Runningsession_main rsm)
        {
            if (rsm == null)
                throw new ServiceException(
                    "RunningSessionMainService - UpdateRunningSessionMain - RunningSession is null"
                );
            if (!rsmRepo.BestaatRunningSessionMain(rsm))
                throw new ServiceException(
                    "RunningSessionMainService - UpdateRunningSessionMain - Deze RunningSession bestaat niet!"
                );

            rsmRepo.UpdateRunningSessionMain(rsm);
            return rsm;
        }

        public void DeleteRunningSessionMain(Runningsession_main rsm)
        {
            if (!rsmRepo.BestaatRunningSessionMain(rsm))
                throw new ServiceException(
                    "RunningSessionMainService - DeleteRunningSessionMain - RunningSession bestaat niet met dit id!"
                );

            rsmRepo.DeleteRunningSessionMain(rsm);
        }
    }
}
