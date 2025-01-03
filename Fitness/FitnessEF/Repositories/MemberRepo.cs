using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Exceptions;
using FitnessBL.Interfaces;
using FitnessBL.Model;
using FitnessEF.Exceptions;
using FitnessEF.Mappers;
using FitnessEF.Model;
using Microsoft.EntityFrameworkCore;

namespace FitnessEF.Repositories
{
    public class MemberRepo : IMemberRepo
    {
        private FitnessContext ctx;

        public MemberRepo(string connectionString)
        {
            ctx = new FitnessContext(connectionString);
        }

        private void SaveAndClear()
        {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

        public IEnumerable<Member> GetMembers()
        {
            try
            {
                List<MemberEF> membersEF = ctx.members.Select(x => x).ToList();
                List<Member> members = new();
                foreach (MemberEF mEF in membersEF)
                {
                    members.Add(MapMember.MapToDomain(mEF));
                }
                return members;
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - GetMembers");
            }
        }

        public IEnumerable<Member> GetMemberNaam(string vn, string ln)
        {
            try
            {
                IEnumerable<MemberEF> memberEFs = ctx
                    .members.Where(x => x.first_name.Contains(vn))
                    .Where(x => x.last_name.Contains(ln))
                    .AsNoTracking()
                    .ToList();

                List<Member> members = new List<Member>();

                if (memberEFs == null)
                {
                    return new List<Member>();
                }
                else
                {
                    foreach (MemberEF mEF in memberEFs)
                    {
                        members.Add(MapMember.MapToDomain(mEF));
                    }

                    return members;
                }
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - GetMemberNaam", ex);
            }
        }

        public IEnumerable<TrainingSession> GetTrainingSessionsMember(Member member)
        {
            try
            {
                IEnumerable<CyclingSessionEF> cyclingSessionEFs = ctx
                    .cyclingsession.Where(c => c.member_id == member.Member_id)
                    .Include(m => m.member)
                    .AsNoTracking()
                    .ToList();

                List<TrainingSession> TrainingSessions = new List<TrainingSession>();
                foreach (CyclingSessionEF cEF in cyclingSessionEFs)
                {
                    Cyclingsession cs = new Cyclingsession(
                        cEF.cyclingsession_id,
                        cEF.date,
                        cEF.duration,
                        cEF.avg_watt,
                        cEF.max_watt,
                        cEF.avg_cadence,
                        cEF.max_cadence,
                        cEF.trainingtype,
                        cEF.comment,
                        MapMember.MapToDomain(cEF.member)
                    );
                    TrainingSessions.Add(cs);
                }

                IEnumerable<Runningsession_mainEF> rsmEFs = ctx
                    .runningsession_main.Where(c => c.member_id == member.Member_id)
                    .Include(m => m.Member)
                    .AsNoTracking()
                    .ToList();
                foreach (Runningsession_mainEF rsmEF in rsmEFs)
                {
                    Runningsession_main rsm = new Runningsession_main(
                        rsmEF.runningsession_id,
                        rsmEF.date,
                        rsmEF.duration,
                        rsmEF.avg_speed,
                        MapMember.MapToDomain(rsmEF.Member)
                    );

                    TrainingSessions.Add(rsm);
                }

                return TrainingSessions;
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - TrainingSessionsMember");
            }
        }

        public IEnumerable<Program> GetProgramListMember(Member member)
        {
            try
            {
                List<ProgramEF> programEFs = ctx
                    .program.Where(p => p.Members.Any(m => m.member_id == member.Member_id)) // Haal programma's op waarin lid 17 zit
                    .ToList();

                List<Program> programs = new List<Program>();
                foreach (ProgramEF programEF in programEFs)
                {
                    programs.Add(MapProgram.MapToDomain(programEF));
                }

                return programs;
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - GetProgramListMember");
            }
        }

        public IEnumerable<Reservation> GetReservationsMember(Member member)
        {
            try
            {
                List<ReservationEF> reservationEFs = ctx
                    .reservation.Where(p => p.member_id == member.Member_id)
                    .Include(m => m.Member)
                    .Include(ts => ts.Time_slot)
                    .Include(e => e.Equipment)
                    .AsNoTracking()
                    .ToList();

                if (reservationEFs.Count == 0)
                {
                    return new List<Reservation>();
                }

                // Stap 2: Groepeer de reserveringen per reservation_id
                List<IGrouping<int, ReservationEF>> groupedReservations = reservationEFs
                    .GroupBy(rs => rs.reservation_id) // Groepeer op basis van reservation_id
                    .ToList(); // Zet het resultaat om naar een lijst van groepen

                // Stap 3: Map de gegroepeerde reserveringen naar het Reservation-domeinmodel
                List<Reservation> reservations = new List<Reservation>();

                foreach (IGrouping<int, ReservationEF> group in groupedReservations)
                {
                    reservations.Add(MapReservation.MapToDomain(group.ToList())); // Map naar je domeinmodel
                }

                return reservations;
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - GetReservationsMember");
            }
        }

        public IEnumerable<TrainingSession> GetTrainingSessionsMemberInMaandInJaar(
            Member member,
            DateTime datum
        )
        {
            try
            {
                IEnumerable<CyclingSessionEF> cyclingSessionEFs = ctx
                    .cyclingsession.Where(c =>
                        c.member_id == member.Member_id
                        && c.date.Month == datum.Month
                        && c.date.Year == datum.Year
                    )
                    .Include(c => c.member)
                    .AsNoTracking()
                    .ToList();

                List<TrainingSession> TrainingSessions = new List<TrainingSession>();
                foreach (CyclingSessionEF cEF in cyclingSessionEFs)
                {
                    Cyclingsession cs = new Cyclingsession(
                        cEF.cyclingsession_id,
                        cEF.date,
                        cEF.duration,
                        cEF.avg_watt,
                        cEF.max_watt,
                        cEF.avg_cadence,
                        cEF.max_cadence,
                        cEF.trainingtype,
                        cEF.comment,
                        MapMember.MapToDomain(cEF.member)
                    );
                    TrainingSessions.Add(cs);
                }

                IEnumerable<Runningsession_mainEF> rsmEFs = ctx
                    .runningsession_main.Where(r =>
                        r.member_id == member.Member_id
                        && r.date.Month == datum.Month
                        && r.date.Year == datum.Year
                    )
                    .Include(m => m.Member)
                    .AsNoTracking()
                    .ToList();
                foreach (Runningsession_mainEF rsmEF in rsmEFs)
                {
                    Runningsession_main rsm = new Runningsession_main(
                        rsmEF.runningsession_id,
                        rsmEF.date,
                        rsmEF.duration,
                        rsmEF.avg_speed,
                        MapMember.MapToDomain(rsmEF.Member)
                    );

                    TrainingSessions.Add(rsm);
                }

                return TrainingSessions.OrderBy(ts => ts.Date).ToList();
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - TrainingSessionsMemberPerMaandEnDatum");
            }
        }

        public Dictionary<int, int> GetTrainingSessionsMemberAantalPerMaandInJaar(
            Member member,
            DateTime date
        )
        {
            try
            {
                Dictionary<int, int> totalCyclingSessionsPerMonth = ctx
                    .cyclingsession.Where(c =>
                        c.member_id == member.Member_id && c.date.Year == date.Year
                    )
                    .Include(m => m.member)
                    .GroupBy(c => c.date.Month)
                    .Select(g => new { Month = g.Key, TotalSessions = g.Count() })
                    .OrderBy(result => result.Month)
                    .ToDictionary(x => x.Month, x => x.TotalSessions);

                Dictionary<int, int> totalRunningSessionsPerMonth = ctx
                    .runningsession_main.Where(c =>
                        c.member_id == member.Member_id && c.date.Year == date.Year
                    )
                    .Include(m => m.Member)
                    .GroupBy(c => c.date.Month)
                    .Select(g => new { Month = g.Key, TotalSessions = g.Count() })
                    .OrderBy(result => result.Month)
                    .ToDictionary(x => x.Month, x => x.TotalSessions);

                // Combineer de resultaten
                Dictionary<int, int> combinedSessionsPerMonth = new Dictionary<int, int>();

                // Voeg cycling sessions toe aan de dictionary
                foreach (int month in totalCyclingSessionsPerMonth.Keys)
                {
                    combinedSessionsPerMonth[month] = totalCyclingSessionsPerMonth[month];
                }

                // Voeg running sessions toe aan de dictionary, of tel ze op als de maand al bestaat
                foreach (int month in totalRunningSessionsPerMonth.Keys)
                {
                    if (combinedSessionsPerMonth.ContainsKey(month))
                    {
                        combinedSessionsPerMonth[month] += totalRunningSessionsPerMonth[month];
                    }
                    else
                    {
                        combinedSessionsPerMonth[month] = totalRunningSessionsPerMonth[month];
                    }
                }

                return combinedSessionsPerMonth;
            }
            catch (Exception ex)
            {
                throw new RepoException(
                    "MemberRepo - GetTrainingSessionsMemberAantalPerMaandInJaar"
                );
            }
        }

        public Dictionary<
            string,
            Dictionary<int, int>
        > GetTrainingSessionsMemberAantalPerMaandInJaarMetType(Member member, DateTime date)
        {
            try
            {
                Dictionary<int, int> totalCyclingSessionsPerMonth = ctx
                    .cyclingsession.Where(c =>
                        c.member_id == member.Member_id && c.date.Year == date.Year
                    )
                    .Include(m => m.member)
                    .GroupBy(c => c.date.Month)
                    .Select(g => new { Month = g.Key, TotalSessions = g.Count() })
                    .OrderBy(result => result.Month)
                    .ToDictionary(x => x.Month, x => x.TotalSessions);

                Dictionary<int, int> totalRunningSessionsPerMonth = ctx
                    .runningsession_main.Where(c =>
                        c.member_id == member.Member_id && c.date.Year == date.Year
                    )
                    .Include(m => m.Member)
                    .GroupBy(c => c.date.Month)
                    .Select(g => new { Month = g.Key, TotalSessions = g.Count() })
                    .OrderBy(result => result.Month)
                    .ToDictionary(x => x.Month, x => x.TotalSessions);

                Dictionary<string, Dictionary<int, int>> SessiesPerMaandMetType =
                    new Dictionary<string, Dictionary<int, int>>();

                SessiesPerMaandMetType.Add("CyclingSessions", totalCyclingSessionsPerMonth);
                SessiesPerMaandMetType.Add("RunningSessions", totalRunningSessionsPerMonth);

                return SessiesPerMaandMetType;
            }
            catch (Exception ex)
            {
                throw new RepoException(
                    "MemberRepo - GetTrainingSessionsMemberAantalPerMaandInJaarMetType"
                );
            }
        }

        public Member GetMemberId(int id)
        {
            try
            {
                MemberEF memberEF = ctx
                    .members.Where(x => x.member_id == id)
                    .AsNoTracking()
                    .FirstOrDefault();

                if (memberEF == null)
                {
                    return null;
                }
                else
                {
                    return MapMember.MapToDomain(memberEF);
                }
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - GetMemberId", ex);
            }
        }

        public Member AddMember(Member member)
        {
            try
            {
                MemberEF m = MapMember.MapToDB(member);
                ctx.members.Add(m);
                SaveAndClear();
                member.Member_id = m.member_id;
                return member;
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - AddMember", ex);
            }
        }

        public bool IsMemberName(Member member)
        {
            try
            {
                return ctx.members.Any(x =>
                    x.first_name == member.FirstName && x.last_name == member.LastName
                );
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - IsMemberName");
            }
        }

        public bool IsMemberId(Member member)
        {
            try
            {
                return ctx.members.Any(x => x.member_id == member.Member_id);
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - IsMemberID", ex);
            }
        }

        public bool IsMemberEmail(Member member)
        {
            try
            {
                return ctx.members.Any(x => x.email == member.Email);
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - IsMemberEmail");
            }
        }

        public void UpdateMember(Member member)
        {
            try
            {
                ctx.members.Update(MapMember.MapToDB(member));
                SaveAndClear();
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - UpdateMember");
            }
        }

        public void DeleteMember(Member member)
        {
            try
            {
                MemberEF memberEF = ctx.members.FirstOrDefault(x =>
                    x.member_id == member.Member_id
                );
                ctx.members.Remove(memberEF);
                SaveAndClear();
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - DeleteMember");
            }
        }

        public int GetAantalGeboekteTijdsloten(DateTime date, Member member)
        {
            try
            {
                int aantal = ctx.reservation.Count(r =>
                    r.member_id == member.Member_id && r.date.Date == date.Date
                );

                return aantal;
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - GetAantalGeboekteTijdsloten");
            }
        }
    }
}
