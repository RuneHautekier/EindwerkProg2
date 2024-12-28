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
            IEnumerable<Member> members = memberRepo.GetMembers();
            if (members.Count() == 0)
                throw new ServiceException("Er zitten nog geen members in de database!");
            return members;
        }

        public Member GetMemberId(int id)
        {
            Member member = memberRepo.GetMemberId(id);
            if (member == null)
                throw new ServiceException(
                    "MemberService - GetMemberId - Er is geen member met dit id!"
                );
            return member;
        }

        public Member GetMemberNaam(string vn, string ln)
        {
            Member member = memberRepo.GetMemberNaam(vn, ln);
            if (member == null)
                throw new ServiceException(
                    "MemberService - GetMemberNaam - Er is geen member met deze naam!"
                );
            return member;
        }

        public Member AddMember(Member member)
        {
            if (member == null)
                throw new ServiceException("MemberService - AddMember - Member is null");
            if (memberRepo.IsMemberName(member))
                throw new ServiceException(
                    "MemberService - AddMember - Member bestaat al (zelfde naam)!"
                );
            if (memberRepo.IsMemberEmail(member))
                throw new ServiceException(
                    "MemberService - AddMember - Dit email is al in gebruik!"
                );
            if (member.Birthday > DateTime.Now)
                throw new ServiceException(
                    "MemberService - AddMember - Je kan niet in de toekomst geboren zijn!"
                );
            memberRepo.AddMember(member);
            return member;
        }

        public Member UpdateMember(Member member)
        {
            if (member == null)
                throw new ServiceException("MemberService - UpdateMember - member is null!");
            if (!memberRepo.IsMemberId(member))
                throw new ServiceException(
                    "MemberService - UpdateMember - Member bestaat niet met dit id!"
                );

            memberRepo.UpdateMember(member);
            return member;
        }

        public void DeleteMember(Member member)
        {
            if (!memberRepo.IsMemberId(member))
                throw new ServiceException(
                    "MemberService - DeleteMember - member bestaat niet met dit id!"
                );

            memberRepo.DeleteMember(member);
        }
    }
}
