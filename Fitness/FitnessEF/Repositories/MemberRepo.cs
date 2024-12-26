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

        public Member GetMemberNaam(string vn, string ln)
        {
            try
            {
                MemberEF memberEF = ctx
                    .members.Where(x => x.first_name == vn)
                    .Where(x => x.last_name == ln)
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

        public bool IsMemberName(string vn, string ln)
        {
            try
            {
                return ctx.members.Any(x => x.first_name == vn && x.last_name == ln);
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - IsMemberName");
            }
        }

        public bool IsMemberId(int id)
        {
            try
            {
                return ctx.members.Any(x => x.member_id == id);
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - IsMemberID", ex);
            }
        }

        public bool IsMemberEmail(string email)
        {
            try
            {
                return ctx.members.Any(x => x.email == email);
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - IsMemberName");
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

        public void DeleteMember(int id)
        {
            try
            {
                MemberEF memberEF = ctx.members.FirstOrDefault(x => x.member_id == id);
                ctx.members.Remove(memberEF);
                SaveAndClear();
            }
            catch (Exception ex)
            {
                throw new RepoException("MemberRepo - DeleteRepo");
            }
        }
    }
}
