using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Model;

namespace FitnessBL.Interfaces
{
    public interface ICyclingSessionRepo
    {
        IEnumerable<Cyclingsession> GetCyclingSessions();
        Cyclingsession GetCyclingSessionId(int id);
        List<Cyclingsession> GetCyclingSessionViaMember(Member member);
        Cyclingsession AddCyclingSession(Cyclingsession cyclingsession);
        bool BestaatCyclingSessionAl(Cyclingsession cyclingsession);
        void UpdateCyclingSession(Cyclingsession cyclingsession);
        bool IsCyclingSessionId(int id);
        void DeleteCyclingSession(int id);
    }
}
