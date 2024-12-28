using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Model;

namespace FitnessBL.Interfaces
{
    public interface IMemberRepo
    {
        IEnumerable<Member> GetMembers();
        IEnumerable<TrainingSession> TrainingSessionsMember(Member member);
        Member GetMemberId(int id);
        Member AddMember(Member member);
        bool IsMemberName(Member member);
        bool IsMemberId(Member member);
        bool IsMemberEmail(Member member);
        void UpdateMember(Member member);
        void DeleteMember(Member member);
        int GetAantalGeboekteTijdsloten(DateTime date, Member member);
    }
}
