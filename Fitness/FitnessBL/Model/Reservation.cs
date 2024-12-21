using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Exceptions;

namespace FitnessBL.Model
{
    public class Reservation
    {
        public int Reservation_id { get; set; }
        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                if (value > DateTime.Now.AddDays(7))
                {
                    throw new ReservatieException(
                        "Een reservatie kan maximaal 1 week op voorhand geplaatst worden!"
                    );
                }
                date = value;
            }
        }
        public Equipment Equipment { get; set; }
        public Member Member { get; set; }
        public List<Time_slot> TimeSlots { get; set; } = new List<Time_slot>();

        public Reservation(
            DateTime datum,
            Equipment toestel,
            Member klant,
            List<Time_slot> tijdslots
        )
        {
            Date = datum;
            Equipment = toestel;
            Member = klant;
            TimeSlots = tijdslots;
        }

        public Reservation(
            int id,
            DateTime datum,
            Equipment toestel,
            Member klant,
            List<Time_slot> tijdslots
        )
        {
            Reservation_id = id;
            Date = datum;
            Equipment = toestel;
            Member = klant;
            TimeSlots = tijdslots;
        }

        public void voegTijdSlotToe(Time_slot tijdslot)
        {
            if (TimeSlots.Count == 2)
            {
                throw new ReservatieException("Je kan maximaal 2 tijdsloten na elkaar reserveren");
            }
            else
            {
                if (TimeSlots.Contains(tijdslot))
                {
                    throw new ReservatieException("Dit Time_slot is al reeds toegevoegd!");
                }
                else
                {
                    if (
                        TimeSlots.Count != 0
                        && (
                            tijdslot.Time_slot_id + 1 != TimeSlots[0].Time_slot_id
                            && tijdslot.Time_slot_id - 1 != TimeSlots[0].Time_slot_id
                        )
                    )
                    {
                        throw new ReservatieException("Je tijdsloten moeten op elkaar volgen dus!");
                    }
                    else
                    {
                        TimeSlots.Add(tijdslot);
                    }
                }
            }
        }

        public void verwijderTijdslot(Time_slot tijdslot)
        {
            if (!TimeSlots.Contains(tijdslot))
            {
                throw new ReservatieException(
                    "Dit tijdslot is niet gereserveerd door u dus kunt u hem niet verwijderen!"
                );
            }
            else
            {
                TimeSlots.Remove(tijdslot);
            }
        }
    }
}
