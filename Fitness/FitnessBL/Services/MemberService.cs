using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Exceptions;
using FitnessBL.Interfaces;
using FitnessBL.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public IEnumerable<Member> GetMemberNaam(string vn, string ln)
        {
            IEnumerable<Member> members = memberRepo.GetMemberNaam(vn, ln);
            if (members.Count() == 0)
                throw new ServiceException(
                    "MemberService - GetMemberNaam - Er is geen member met deze naam!"
                );
            return members;
        }

        public IEnumerable<TrainingSession> GetTrainingSessionsMember(Member member)
        {
            if (member == null)
                throw new ServiceException(
                    "MemberService - GetTrainingSessionsMember - member is null!"
                );

            if (!memberRepo.IsMemberId(member))
                throw new ServiceException(
                    "MemberService - GetTrainingSessionsMember - member bestaat niet met dit id!"
                );
            IEnumerable<TrainingSession> TrainingSessions = memberRepo.GetTrainingSessionsMember(
                member
            );
            if (TrainingSessions.Count() == 0)
                throw new ServiceException(
                    "MemberService - GetTrainingSessionsMember - Deze member heeft nog geen TrainingSessions!"
                );
            return TrainingSessions;
        }

        public IEnumerable<Program> GetProgramListMember(Member member)
        {
            if (member == null)
                throw new ServiceException("MemberService - GetProgramListMember - Member is null");
            if (!memberRepo.IsMemberId(member))
                throw new ServiceException(
                    "MemberService - GetProgramListMember - Er bestaat geen member met dit id!"
                );

            IEnumerable<Program> programList = new List<Program>();
            programList = memberRepo.GetProgramListMember(member);
            if (!programList.Any())
                throw new ServiceException(
                    "MemberService - GetProgramListMember - Deze member is nog voor geen enkel Program ingeschreven!"
                );
            return programList;
        }

        public IEnumerable<Reservation> GetReservationsMember(Member member)
        {
            if (member == null)
                throw new ServiceException("MemberService - GetProgramListMember - Member is null");
            if (!memberRepo.IsMemberId(member))
                throw new ServiceException(
                    "MemberService - GetProgramListMember - Er bestaat geen member met dit id!"
                );

            IEnumerable<Reservation> reservations = new List<Reservation>();
            reservations = memberRepo.GetReservationsMember(member);
            if (!reservations.Any())
                throw new ServiceException(
                    "MemberService - GetReservationsMember - Deze member heeft nog geen enkele reservations!"
                );

            return reservations;
        }

        public IEnumerable<TrainingSession> GetTrainingSessionsMemberInMaandInJaar(
            Member member,
            DateTime date
        )
        {
            if (member == null)
                throw new ServiceException(
                    "MemberService - TrainingSessionsMemberPerMaandInJaar - member is null!"
                );

            if (!memberRepo.IsMemberId(member))
                throw new ServiceException(
                    "MemberService - TrainingSessionsMemberPerMaandInJaar - member bestaat niet met dit id!"
                );
            IEnumerable<TrainingSession> TrainingSessions =
                memberRepo.GetTrainingSessionsMemberInMaandInJaar(member, date);
            if (TrainingSessions.Count() == 0)
                throw new ServiceException(
                    $"MemberService - TrainingSessionsMemberPerMaandInJaar - Deze member heeft geen TrainingSessions in maand {date.Month} in jaar {date.Year}!"
                );
            return TrainingSessions;
        }

        public Dictionary<int, int> GetTrainingSessionsMemberAantalPerMaandInJaar(
            Member member,
            DateTime date
        )
        {
            if (member == null)
                throw new ServiceException(
                    "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaar - member is null!"
                );

            if (!memberRepo.IsMemberId(member))
                throw new ServiceException(
                    "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaar - member bestaat niet met dit id!"
                );

            Dictionary<int, int> dic = memberRepo.GetTrainingSessionsMemberAantalPerMaandInJaar(
                member,
                date
            );

            if (dic.Keys.Count() == 0)
                throw new ServiceException(
                    $"MemberService - GetTrainingSessionsMemberAantalPerMaandInJaar - Deze member heeft geen TrainingSessions in maand {date.Month} jaar {date.Year}!"
                );
            return dic;
        }

        public Dictionary<
            string,
            Dictionary<int, int>
        > GetTrainingSessionsMemberAantalPerMaandInJaarMetType(Member member, DateTime date)
        {
            if (member == null)
                throw new ServiceException(
                    "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaarMetType - member is null!"
                );

            if (!memberRepo.IsMemberId(member))
                throw new ServiceException(
                    "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaarMetType - member bestaat niet met dit id!"
                );

            Dictionary<string, Dictionary<int, int>> dic =
                memberRepo.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(member, date);

            if (dic.Keys.Count() == 0)
                throw new ServiceException(
                    $"MemberService - GetTrainingSessionsMemberAantalPerMaandInJaar - Deze member heeft geen TrainingSessions in maand {date.Month} jaar {date.Year}!"
                );
            return dic;
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

        public int GetAantalGeboekteTijdsloten(Member member, DateTime date)
        {
            if (member == null)
                throw new ServiceException(
                    "MemberService - GetAantalGeboekteTijdsloten - member is null!"
                );

            if (!memberRepo.IsMemberId(member))
                throw new ServiceException(
                    "MemberService - GetAantalGeboekteTijdsloten - member bestaat niet met dit id!"
                );

            return memberRepo.GetAantalGeboekteTijdsloten(date, member);
        }
    }
}
