using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Exceptions;

namespace FitnessBL.Model
{
    public class Program
    {
        public string ProgramCode { get; set; }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ProgramException("Het programma moet een naam hebben!");
                }
                else
                {
                    name = value;
                }
            }
        }

        private string target;

        public string Target
        {
            get { return target; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ProgramException("Het doelpubliek moet ingevuld zijn!");
                }
                else
                {
                    target = value;
                }
            }
        }

        public DateTime Startdate { get; set; }

        private int max_members;

        public int Max_members
        {
            get { return max_members; }
            set
            {
                if (value <= 0)
                {
                    throw new ProgramException("Het maximaal aantal leden moet meer dan 0 zijn!");
                }
                else
                {
                    max_members = value;
                }
            }
        }
        public Dictionary<int, Member> Members = new Dictionary<int, Member>();

        public Program(
            string naam,
            string doelpubliek,
            DateTime startDatum,
            int maxAantal,
            Dictionary<int, Member> klanten
        )
        {
            Name = naam;
            Target = doelpubliek;
            Startdate = startDatum;
            Max_members = maxAantal;
            Members = klanten;
        }

        public Program(
            string programCode,
            string naam,
            string doelpubliek,
            DateTime startDatum,
            int maxAantal,
            Dictionary<int, Member> klanten
        )
        {
            ProgramCode = programCode;
            Name = naam;
            Target = doelpubliek;
            Startdate = startDatum;
            Max_members = maxAantal;
            Members = klanten;
        }

        public void SchrijfKlantIn(Member klant)
        {
            if (Members.Count + 1 > Max_members)
            {
                throw new ProgramException(
                    "Het maximum aantal klanten is al bereikt voor dit programma!"
                );
            }
            else
            {
                if (Members.ContainsKey(klant.Member_id))
                {
                    throw new ProgramException("Deze klant is al ingeschreven voor het programma!");
                }
                else
                {
                    Members.Add(klant.Member_id, klant);
                }
            }
        }

        public void SchrijfKlantUit(Member klant)
        {
            if (!Members.ContainsKey(klant.Member_id))
            {
                throw new ProgramException(
                    "Deze klant is niet ingeschreven voor dit programma dus kan hij niet worden uitgeschreven!"
                );
            }
            else
            {
                Members.Remove(klant.Member_id);
            }
        }
    }
}
