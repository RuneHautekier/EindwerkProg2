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
        IEnumerable<Member> GetMemberNaam(string vn, string ln);

        IEnumerable<TrainingSession> GetTrainingSessionsMember(Member member);
        IEnumerable<Program> GetProgramListMember(Member member);
        IEnumerable<Reservation> GetReservationsMember(Member member);
        IEnumerable<TrainingSession> GetTrainingSessionsMemberInMaandInJaar(
            Member member,
            DateTime date
        );

        Dictionary<int, int> GetTrainingSessionsMemberAantalPerMaandInJaar(
            Member member,
            DateTime date
        );

        Dictionary<
            string,
            Dictionary<int, int>
        > GetTrainingSessionsMemberAantalPerMaandInJaarMetType(Member member, DateTime date);

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
