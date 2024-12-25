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
        Member GetMemberId(int id);
        Member AddMember(Member member);
        bool IsMemberName(string vn, string ln);
    }
}
