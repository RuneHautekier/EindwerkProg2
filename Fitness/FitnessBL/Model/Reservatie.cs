using FitnessBL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessBL.Model
{
    public class Reservatie
    {
        public int Id { get; set; }
        public Klant Klant { get; set; }
        public DateTime Datum { get; set; }

        public Toestel Toestel { get; set; }
        public List<Tijdslot> Tijdslots { get; set;} = new List<Tijdslot>();



    }
}
