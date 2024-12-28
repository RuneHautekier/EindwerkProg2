using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Model;

namespace FitnessBL.Interfaces
{
    public interface IRunningSessionMainRepo
    {
        IEnumerable<Runningsession_main> GetRunningSessionsMain();
        Runningsession_main GetRunningSessionMainId(int id);
        IEnumerable<Runningsession_main> GetRunningSessionMember(Member member);
        Runningsession_main AddRunningSessionMain(Runningsession_main rsm);
        bool BestaatRunningSessionMainAl(Runningsession_main rsm);

        bool BestaatRunningSessionMain(Runningsession_main rsm);
        void UpdateRunningSessionMain(Runningsession_main rsm);
        void DeleteRunningSessionMain(Runningsession_main rsm);
    }
}
