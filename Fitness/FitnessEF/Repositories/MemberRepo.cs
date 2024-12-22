using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
