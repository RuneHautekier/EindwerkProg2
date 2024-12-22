using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Model;
using FitnessEF.Exceptions;
using FitnessEF.Model;

namespace FitnessEF.Mappers
{
    public class MapReservation
    {
        public static Reservation MapToDomain(List<ReservationEF> reservationEFs)
        {
            try
            {
                ReservationEF rsEF = reservationEFs[0];

                Dictionary<Time_slot, Equipment> dic = new Dictionary<Time_slot, Equipment>();
                foreach (ReservationEF rs in reservationEFs)
                {
                    dic.Add(
                        MapTime_slot.MapToDomain(rs.Time_slot),
                        MapEquipment.MapToDomain(rs.Equipment)
                    );
                }

                return new Reservation(
                    rsEF.reservation_id,
                    rsEF.date,
                    MapMember.MapToDomain(rsEF.Member),
                    dic
                );
            }
            catch (Exception ex)
            {
                throw new MapException("MapMember - MapToDomain", ex);
            }
        }

        public static List<ReservationEF> MapToEF(Reservation reservation)
        {
            try
            {
                List<ReservationEF> rsEFs = new List<ReservationEF>();

                // Doorloop elk tijdslot en bijbehorende equipment in de Dictionary van de Reservation
                foreach (
                    KeyValuePair<
                        Time_slot,
                        Equipment
                    > timeslotEquipment in reservation.TimeslotEquipment
                )
                {
                    // Maak een nieuwe ReservationEF voor elke combinatie van Time_slot en Equipment
                    ReservationEF rsEF = new ReservationEF
                    {
                        reservation_id = reservation.Reservation_id,
                        date = reservation.Date,
                        member_id = reservation.Member.Member_id,
                        time_slot_id = timeslotEquipment.Key.Time_slot_id,
                        equipment_id = timeslotEquipment.Value.Equipment_id
                    };

                    // Voeg de nieuwe ReservationEF toe aan de lijst
                    rsEFs.Add(rsEF);
                }

                return rsEFs;
            }
            catch (Exception ex)
            {
                throw new MapException("MapToEF failed", ex);
            }
        }
    }
}
