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
    public class CyclingSessionService
    {
        private ICyclingSessionRepo cyclingSessionRepo;
        private IMemberRepo memberRepo;

        public CyclingSessionService(ICyclingSessionRepo cyclingSessionRepo, IMemberRepo memberRepo)
        {
            this.cyclingSessionRepo = cyclingSessionRepo;
            this.memberRepo = memberRepo;
        }

        public IEnumerable<Cyclingsession> GetCyclingSessions()
        {
            IEnumerable<Cyclingsession> cyclingsessions = cyclingSessionRepo.GetCyclingSessions();
            if (cyclingsessions.Count() == 0)
                throw new ServiceException("Er zitten geen cyclingsessions in de database!");
            return cyclingsessions;
        }

        public Cyclingsession GetCyclingSessionId(int id)
        {
            Cyclingsession cs = cyclingSessionRepo.GetCyclingSessionId(id);
            if (cs == null)
                throw new ServiceException(
                    "CyclingSessionRepo - GetCyclingSessionId - Er is geen CyclingSession met dit id!"
                );
            return cs;
        }

        public List<Cyclingsession> GetCyclingsessionViaMember(Member member)
        {
            if (member == null)
                throw new ServiceException(
                    "CyclingSessionRepo - GetCyclingsessionViaMember - Deze member bestaat niet!"
                );
            List<Cyclingsession> css = cyclingSessionRepo.GetCyclingSessionViaMember(member);
            if (css == null)
                throw new ServiceException(
                    "CyclingSessionRepo - GetCyclingsessionViaMember - Deze member heeft nog geen cyclingsessions!"
                );
            return css;
        }

        public Cyclingsession AddCyclingSession(Cyclingsession cs)
        {
            if (cs == null)
                throw new ServiceException(
                    "CyclingSessionRepo - AddCyclingSession - Cyclingsession is null!"
                );
            if (cyclingSessionRepo.BestaatCyclingSessionAl(cs))
                throw new ServiceException(
                    "CyclingSessionRepo - AddCyclingSession - Deze CyclingSession zit al in de database!"
                );
            cyclingSessionRepo.AddCyclingSession(cs);
            return cs;
        }

        public Cyclingsession UpdateCyclingSession(Cyclingsession cs)
        {
            if (cs == null)
                throw new ServiceException(
                    "CyclingSessionRepo - UpdateCyclingSession - CyclingSession is null"
                );
            if (!cyclingSessionRepo.IsCyclingSessionId(cs.Cyclingsession_id))
                throw new ServiceException(
                    "CyclingSessionRepo - UpdateCyclingSession - Cyclingsession bestaat niet met dit id"
                );

            cyclingSessionRepo.UpdateCyclingSession(cs);
            return cs;
        }

        public void DeleteCyclingSession(int id)
        {
            if (!cyclingSessionRepo.IsCyclingSessionId(id))
                throw new ServiceException(
                    "CyclingSessionRepo - DeleteCyclingSession - CyclingSession bestaat niet met dit id!"
                );

            cyclingSessionRepo.DeleteCyclingSession(id);
        }
    }
}
