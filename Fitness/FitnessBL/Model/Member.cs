using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FitnessBL.Enums;
using FitnessBL.Exceptions;

namespace FitnessBL.Model
{
    public class Member
    {
        public int Member_id { get; set; }

        private string firstName;

        public string FirstName
        {
            get { return firstName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new KlantException("De klant moet een firstName hebben!");
                }
                else
                {
                    firstName = value;
                }
            }
        }

        private string lastName;

        public string LastName
        {
            get { return lastName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new KlantException("De klant moet een lastName hebben!");
                }
                else
                {
                    lastName = value;
                }
            }
        }

        private string email;

        public string Email
        {
            get { return email; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new KlantException("De klant moet een emailadres hebben!");
                }
                else
                {
                    if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        throw new KlantException("Ongeldig e-mailadres!");
                    }
                    else
                    {
                        email = value;
                    }
                }
            }
        }

        private string address;

        public string Address
        {
            get { return address; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new KlantException("De klant moet een verblijfsplaats hebben!");
                }
                else
                {
                    address = value;
                }
            }
        }

        private DateTime birthday;

        public DateTime Birthday
        {
            get { return birthday; }
            set
            {
                if (value > DateTime.Now)
                {
                    throw new KlantException("De klant kan niet in de toekomst geboren zijn");
                }
                birthday = value;
            }
        }

        public List<string> Interests { get; set; } = new List<string>();

        private TypeKlant memberType;

        public TypeKlant MemberType
        {
            get { return memberType; }
            set
            {
                if (
                    value != TypeKlant.Bronze
                    && value != TypeKlant.Silver
                    && value != TypeKlant.Gold
                )
                {
                    throw new KlantException(
                        "De klant moet van het type Bronze, Silver of Gold zijn!"
                    );
                }
                else
                {
                    memberType = value;
                }
            }
        }

        public Member(
            int member_id,
            string firstName,
            string lastName,
            string emailadres,
            string verblijfsplaats,
            DateTime geboorteDatum,
            List<string> interesses,
            TypeKlant type
        )
        {
            Member_id = member_id;
            FirstName = firstName;
            LastName = lastName;
            Email = emailadres;
            Address = verblijfsplaats;
            Birthday = geboorteDatum;
            Interests = interesses;
            MemberType = type;
        }

        public Member(
            string firstName,
            string lastName,
            string emailadres,
            string verblijfsplaats,
            DateTime geboorteDatum,
            List<string> interesses,
            TypeKlant type
        )
        {
            FirstName = firstName;
            LastName = lastName;
            Email = emailadres;
            Address = verblijfsplaats;
            Birthday = geboorteDatum;
            Interests = interesses;
            MemberType = type;
        }

        public void VoegIntresseToe(string interesse)
        {
            string interesseStyled = interesse.Trim().ToLower();

            if (Interests.Contains(interesseStyled))
            {
                throw new KlantException($"{interesse} zit al in de lijst met interesses!");
            }
            else
            {
                Interests.Add(interesseStyled);
            }
        }

        public void VerwijderInteresse(string interesse)
        {
            string interesseStyled = interesse.Trim().ToLower();
            if (!Interests.Contains(interesseStyled))
            {
                throw new KlantException(
                    $"{interesseStyled} kan niet worden verwijderd uit de lijst van interesses aangezien deze er niet inzit!"
                );
            }
            else
            {
                Interests.Remove(interesseStyled);
            }
        }
    }
}
