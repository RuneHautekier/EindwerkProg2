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
        private DateTime datum;
        public DateTime Datum
        {
            get { return datum; }
            set
            {
                if (value > DateTime.Now.AddDays(7))
                {
                    throw new ReservatieException("Een reservatie kan maximaal 1 week op voorhand geplaatst worden!");
                }
                datum = value;
            }
        }
        public Toestel Toestel { get; set; }
        public Klant Klant { get; set; }
        public List<Tijdslot> Tijdslots { get; set;} = new List<Tijdslot>();

        public Reservatie(DateTime datum, Toestel toestel, Klant klant, List<Tijdslot> tijdslots)
        {
            Datum = datum;
            Toestel = toestel;
            Klant = klant;
            Tijdslots = tijdslots;
        }

        public Reservatie(int id, DateTime datum, Toestel toestel, Klant klant, List<Tijdslot> tijdslots)
        {
            Id = id;
            Datum = datum;
            Toestel = toestel;
            Klant = klant;
            Tijdslots = tijdslots;
        }

        public void voegTijdSlotToe(Tijdslot tijdslot)
        {
            if (Tijdslots.Count == 2)
            {
                throw new ReservatieException("Je kan maximaal 2 tijdsloten na elkaar reserveren");
            }
            else
            {
                if (Tijdslots.Contains(tijdslot))
                {
                    throw new ReservatieException("Dit Tijdslot is al reeds toegevoegd!");
                }
                else
                {
                    if (Tijdslots.Count != 0 && (tijdslot.Id + 1 != Tijdslots[0].Id && tijdslot.Id - 1 != Tijdslots[0].Id))
                    {
                        throw new ReservatieException("Je tijdsloten moeten op elkaar volgen dus!");
                    }
                    else
                    { 
                        Tijdslots.Add(tijdslot);
                    }
                }
            }
        }

        public void verwijderTijdslot(Tijdslot tijdslot)
        {
            if (!Tijdslots.Contains(tijdslot))
            {
                throw new ReservatieException("Dit tijdslot is niet gereserveerd door u dus kunt u hem niet verwijderen!");
            }
            else
            {
                Tijdslots.Remove(tijdslot);
            }     
        }
    }
}
