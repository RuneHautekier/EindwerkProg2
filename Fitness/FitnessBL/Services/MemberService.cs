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

        public IEnumerable<Member> GetMembers()
        {
            try
            {
                return memberRepo.GetMembers();
            }
            catch (Exception ex)
            {
                throw new ServiceException("MemberService - GetMembers");
            }
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

        public Member GetMemberNaam(string vn, string ln)
        {
            try
            {
                return memberRepo.GetMemberNaam(vn, ln);
            }
            catch (Exception ex)
            {
                throw new ServiceException("GetMemberNaam", ex);
            }
        }

        public Member AddMember(Member member)
        {
            if (member == null)
                throw new ServiceException("AddMember - Member is null");
            if (memberRepo.IsMemberName(member.FirstName, member.LastName))
                throw new ServiceException("AddMember - Member bestaat al (zelfde naam)");
            memberRepo.AddMember(member);
            return member;
        }

        public Member UpdateMember(Member member)
        {
            try
            {
                if (member == null)
                    throw new ServiceException("UpdateMember - member is null");
                if (!memberRepo.IsMemberId(member.Member_id))
                    throw new ServiceException("UpdateMember - Member bestaat niet op id");

                memberRepo.UpdateMember(member);
                return member;
            }
            catch (Exception ex)
            {
                throw new ServiceException("UpdateMember", ex);
            }
        }

        public void DeleteMember(int id)
        {
            if (!memberRepo.IsMemberId(id))
                throw new ServiceException("MemberService - member bestaat niet op id");
            memberRepo.DeleteMember(id);
        }
    }
}
