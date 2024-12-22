using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FitnessBL.Exceptions;
using Newtonsoft.Json;

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
        public Member Member { get; set; }

        [Newtonsoft.Json.JsonIgnore] // Negeer de originele Dictionary bij serialisatie
        public Dictionary<Time_slot, Equipment> TimeslotEquipment =
            new Dictionary<Time_slot, Equipment>();

        [JsonProperty("EquipmentPerTimeslot")] // Nieuwe JSON-weergave
        public List<object> TimeslotEquipmentSerialized
        {
            get
            {
                return TimeslotEquipment
                    .Select(kvp => new { TimeSlot = kvp.Key, Equipment = kvp.Value })
                    .ToList<object>();
            }
        }

        public Reservation(
            DateTime datum,
            Member klant,
            Dictionary<Time_slot, Equipment> timeslotEquipment
        )
        {
            Date = datum;
            Member = klant;
            TimeslotEquipment = timeslotEquipment;
        }

        public Reservation(
            int id,
            DateTime datum,
            Member klant,
            Dictionary<Time_slot, Equipment> timeslotEquipment
        )
        {
            Reservation_id = id;
            Date = datum;
            Member = klant;
            TimeslotEquipment = timeslotEquipment;
        }

        public void voegTijdSlotToe(Time_slot tijdslot, Equipment equipment)
        {
            if (TimeslotEquipment.Count == 2)
            {
                throw new ReservatieException("Je kan maximaal 2 tijdsloten na elkaar reserveren");
            }
            else
            {
                if (TimeslotEquipment.Keys.Contains(tijdslot))
                {
                    throw new ReservatieException("Dit Time_slot is al reeds toegevoegd!");
                }
                else
                {
                    if (
                        TimeslotEquipment.Count != 0
                        && (
                            tijdslot.Time_slot_id + 1
                                != TimeslotEquipment.Keys.ElementAt(0).Time_slot_id
                            && tijdslot.Time_slot_id - 1
                                != TimeslotEquipment.Keys.ElementAt(0).Time_slot_id
                        )
                    )
                    {
                        throw new ReservatieException("Je tijdsloten moeten op elkaar volgen dus!");
                    }
                    else
                    {
                        TimeslotEquipment.Add(tijdslot, equipment);
                    }
                }
            }
        }

        public void verwijderTijdslot(Time_slot tijdslot)
        {
            if (!TimeslotEquipment.Keys.Contains(tijdslot))
            {
                throw new ReservatieException(
                    "Dit tijdslot is niet gereserveerd door u dus kunt u hem niet verwijderen!"
                );
            }
            else
            {
                TimeslotEquipment.Remove(tijdslot);
            }
        }
    }
}
