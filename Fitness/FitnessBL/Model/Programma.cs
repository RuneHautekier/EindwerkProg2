using FitnessBL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessBL.Model
{
    public class Programma
    {
        public int? Id { get; set; }

		private string naam;

		public string Naam
		{
			get { return naam; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new ProgrammaException("Het programma moet een naam hebben!");
				}
				else
				{ 
					naam = value; 
				}
			}
		}

		private string doelpubliek;

		public string Doelpubliek
		{
			get { return doelpubliek; }
			set 
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new ProgrammaException("Het doelpubliek moet ingevuld zijn!");
				}
				else
				{ 
					doelpubliek = value; 
				}
			}
		}

		public DateTime StartDatum { get; set; }

		private int maxAantal;

		public int MaxAantal
		{
			get { return maxAantal; }
			set 
			{
				if (value <= 0)
				{
					throw new ProgrammaException("Het maximaal aantal leden moet meer dan 0 zijn!");
				}
				else
				{ 
					maxAantal = value; 
				}
			}
		}

		public List<Klant> Klanten = new List<Klant>();

        public Programma(int? id, string naam, string doelpubliek, DateTime startDatum, int maxAantal, List<Klant> klanten)
        {
            Id = id;
            Naam = naam;
            Doelpubliek = doelpubliek;
            StartDatum = startDatum;
            MaxAantal = maxAantal;
            Klanten = klanten;
        }
    }
}
