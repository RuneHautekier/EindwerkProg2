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
        Member GetMemberId(int id);
        Member AddMember(Member member);
        bool IsMemberName(string vn, string ln);
        bool IsMemberId(int id);
        bool IsMemberEmail(string email);
        void UpdateMember(Member member);
        Member GetMemberNaam(string vn, string ln);
        void DeleteMember(int id);
    }
}
