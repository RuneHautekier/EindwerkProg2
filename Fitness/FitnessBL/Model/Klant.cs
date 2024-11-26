using FitnessBL.Enums;
using FitnessBL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessBL.Model
{
    public class Klant
    {
		public int? Id { get; set; }

		private string voornaam;

		public string Voornaam
		{
			get { return voornaam; }
			set 
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new KlantException("De klant moet een voornaam hebben!");
				}
				else
				{ 
					voornaam = value; 
				}
			}
		}

        private string achternaam;

        public string Achternaam
        {
            get { return achternaam; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new KlantException("De klant moet een achternaam hebben!");
                }
                else
                {
                    achternaam = value;
                }
            }
        }

        private string emailadres;

        public string Emailadres
        {
            get { return emailadres; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new KlantException("De klant moet een emailadres hebben!");
                }
                else
                {
                    emailadres = value;
                }
            }
        }

        private string verblijfsplaats;

        public string Verblijfsplaats
        {
            get { return verblijfsplaats; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new KlantException("De klant moet een verblijfsplaats hebben!");
                }
                else
                {
                    verblijfsplaats = value;
                }
            }
        }

        private DateTime geboortedatum;

        public DateTime GeboorteDatum
        {
            get { return geboortedatum; }
            set 
            {
                if (value > DateTime.Now)
                {
                    throw new KlantException("De klant kan niet in de toekomst geboren zijn");
                }
                geboortedatum = value; 
            }
        }

        List<string> Interesses { get; set; }

        private TypeKlant type;

        public TypeKlant Type
        {
            get { return type; }
            set 
            {
                if (value != TypeKlant.Bronze || value != TypeKlant.Silver || value != TypeKlant.Gold)
                {
                    throw new KlantException("De klant moet van het type Bronze, Silver of Gold zijn!");
                }
                else
                { 
                    type = value; 
                }
            }
        }

        
        public Klant(string voornaam, string achternaam, string emailadres, string verblijfsplaats, DateTime geboorteDatum, List<string>? interesses, TypeKlant type)
        {
            Voornaam = voornaam;
            Achternaam = achternaam;
            Emailadres = emailadres;
            Verblijfsplaats = verblijfsplaats;
            GeboorteDatum = geboorteDatum;
            Interesses = interesses;
            Type = type;
        }

        public Klant(int id, string voornaam, string achternaam, string emailadres, string verblijfsplaats, DateTime geboorteDatum, List<string>? interesses, TypeKlant type)
        {
            Id = id;
            Voornaam = voornaam;
            Achternaam = achternaam;
            Emailadres = emailadres;
            Verblijfsplaats = verblijfsplaats;
            GeboorteDatum = geboorteDatum;
            Interesses = interesses;
            Type = type;
        }
    }
}
