using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Enums;
using FitnessBL.Model;
using FitnessEF.Exceptions;
using FitnessEF.Model;

namespace FitnessEF.Mappers
{
    public class MapKlant
    {
        public static Member MapToDomain(MemberEF klantEF)
        {
            try
            {
                return new Member(
                    klantEF.member_id,
                    klantEF.first_name,
                    klantEF.last_name,
                    klantEF.email,
                    klantEF.address,
                    klantEF.birthday,
                    StringToList(klantEF.interests),
                    StringToEnum(klantEF.membertype)
                );
            }
            catch (Exception ex)
            {
                throw new MapKlantException("MapGebruiker - MapToDomain", ex);
            }
        }

        public static MemberEF MapToDB(Member klant)
        {
            try
            {
                return new MemberEF(
                    klant.Member_id,
                    klant.FirstName,
                    klant.LastName,
                    klant.Email,
                    klant.Address,
                    klant.Birthday,
                    ListToString(klant.Interests),
                    EnumToString(klant.MemberType)
                );
            }
            catch (Exception ex)
            {
                throw new MapKlantException("MapGebruiker - MapToDB", ex);
            }
        }

        // De lijst van intresses uit domeinlaag mappen naar string voor database
        public static string ListToString(List<string> intresses)
        {
            if (intresses.Count == 0 || intresses == null)
            {
                return null;
            }
            else
            {
                return string.Join(", ", intresses);
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
            throw new MapKlantException("Invalid Enum Type");
        }

        // De enum van het type omzetten naar een string voor in de database
        public static string EnumToString(TypeKlant typeKlant)
        {
            return typeKlant.ToString();
        }
    }
}
