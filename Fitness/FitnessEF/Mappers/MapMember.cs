using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Enums;
using FitnessBL.Model;
using FitnessEF.Exceptions;
using FitnessEF.Model;

namespace FitnessEF.Mappers
{
    public class MapMember
    {
        public static Member MapToDomain(MemberEF memberEF)
        {
            try
            {
                return new Member(
                    memberEF.member_id,
                    memberEF.first_name,
                    memberEF.last_name,
                    memberEF.email,
                    memberEF.address,
                    memberEF.birthday,
                    StringToList(memberEF.interests),
                    StringToEnum(memberEF.membertype)
                );
            }
            catch (Exception ex)
            {
                throw new MapException("MapMember - MapToDomain", ex);
            }
        }

        public static MemberEF MapToDB(Member member)
        {
            try
            {
                return new MemberEF(
                    member.Member_id,
                    member.FirstName,
                    member.LastName,
                    member.Email,
                    member.Address,
                    member.Birthday,
                    ListToString(member.Interests),
                    EnumToString(member.MemberType)
                );
            }
            catch (Exception ex)
            {
                throw new MapException("MapMember - MapToDB", ex);
            }
        }

        // De lijst van intresses uit domeinlaag mappen naar string voor database
        public static string ListToString(List<string> intrests)
        {
            if (intrests.Count == 0 || intrests == null)
            {
                return null;
            }
            else
            {
                return string.Join(", ", intrests);
            }
        }

        // De lijst van strings uit de database mappen naar een List voor de domeinlaag.
        public static List<string> StringToList(string str)
        {
            List<string> list = new List<string>();
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }
            else
            {
                list = str.Split(',').ToList();
                return list;
            }
        }

        // De string van het typeklant uit de database omzetten naar een een enum
        public static TypeKlant StringToEnum(string str)
        {
            if (Enum.TryParse(str, out TypeKlant typeKlant))
            {
                return typeKlant;
            }
            throw new MapException("Invalid Enum Type");
        }

        // De enum van het type omzetten naar een string voor in de database
        public static string EnumToString(TypeKlant typeKlant)
        {
            return typeKlant.ToString();
        }
    }
}
