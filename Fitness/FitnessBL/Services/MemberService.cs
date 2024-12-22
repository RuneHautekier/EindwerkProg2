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
    public class MemberService
    {
        private IMemberRepo memberRepo;

        public MemberService(IMemberRepo memberRepo)
        {
            this.memberRepo = memberRepo;
        }

        public Member GetMemberId(int id)
        {
            try
            {
                return memberRepo.GetMemberId(id);
            }
            catch (Exception ex)
            {
                throw new ServiceException("MemberService - GetMemberId");
            }
        }
    }
}
