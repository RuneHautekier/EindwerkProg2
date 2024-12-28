using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Model;
using FitnessEF.Exceptions;
using FitnessEF.Model;

namespace FitnessEF.Mappers
{
    public class MapProgram
    {
        public static Program MapToDomain(ProgramEF programEF)
        {
            try
            {
                Dictionary<int, Member> members = new Dictionary<int, Member>();
                foreach (MemberEF m in programEF.Members)
                {
                    Member member = MapMember.MapToDomain(m);
                    members.Add(member.Member_id, member);
                }
                return new Program(
                    programEF.programCode,
                    programEF.name,
                    programEF.target,
                    programEF.startdate,
                    programEF.max_members,
                    members
                );
            }
            catch (Exception ex)
            {
                throw new MapException("MapProgram - MapToDomain", ex);
            }
        }

        public static ProgramEF MapToDB(Program program)
        {
            try
            {
                List<MemberEF> members = new List<MemberEF>();
                foreach (Member m in program.Members.Values)
                {
                    MemberEF mEF = MapMember.MapToDB(m);
                    members.Add(mEF);
                }
                return new ProgramEF(
                    program.ProgramCode,
                    program.Name,
                    program.Target,
                    program.Startdate,
                    program.Max_members,
                    members
                );
            }
            catch (Exception ex)
            {
                throw new MapException("MapProgram - MapToDB", ex);
            }
        }
    }
}
