using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Exceptions;
using FitnessBL.Interfaces;
using FitnessBL.Model;
using FitnessEF.Exceptions;
using FitnessEF.Mappers;
using FitnessEF.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FitnessEF.Repositories
{
    public class ReservationRepo : IReservationRepo
    {
        private FitnessContext ctx;

        public ReservationRepo(string connectionString)
        {
            ctx = new FitnessContext(connectionString);
        }

        private void SaveAndClear()
        {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

        public Reservation GetReservationId(int id)
        {
            try
            {
                List<ReservationEF> reservationsEF = ctx
                    .reservation.Where(x => x.reservation_id == id)
                    .Include(m => m.Member)
                    .Include(e => e.Equipment)
                    .Include(t => t.Time_slot)
                    .AsNoTracking()
                    .ToList();

                if (reservationsEF == null)
                {
                    return null;
                }
                else
                {
                    return MapReservation.MapToDomain(reservationsEF);
                }
            }
            catch (Exception ex)
            {
                throw new RepoException("ReservationRepo - GetReservationId", ex);
            }
        }

        public int GetNieuwReservationId()
        {
            try
            {
                int hoogstBestaandId = ctx.reservation.Max(r => r.reservation_id);
                return hoogstBestaandId + 1;
            }
            catch (Exception ex)
            {
                throw new RepoException("ReservationRepo - GetNieuwReservationId");
            }
        }

        public Reservation AddReservation(Reservation reservation)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    List<ReservationEF> rsEFs = MapReservation.MapToEF(reservation);

                    foreach (ReservationEF rsEF in rsEFs)
                    {
                        ctx.reservation.Add(rsEF);
                    }

                    SaveAndClear();
                    transaction.Commit();
                    return reservation;
                }
                catch (Exception ex)
                {
                    throw new RepoException("TrainingRepo - AddTraining", ex);
                }
            }
        }

        public void CheckIfReservationExists(Reservation rs)
        {
            List<ReservationEF> rsEFs = new List<ReservationEF>();
            foreach (Time_slot timeSlot in rs.TimeslotEquipment.Keys)
            {
                rsEFs = ctx
                    .reservation.Where(ts => ts.time_slot_id == timeSlot.Time_slot_id)
                    .Where(e => e.equipment_id == rs.TimeslotEquipment[timeSlot].Equipment_id)
                    .Where(d => d.date == rs.Date)
                    .Include(ts => ts.Time_slot)
                    .Include(e => e.Equipment)
                    .AsNoTracking()
                    .ToList();
            }

            if (rsEFs.Any())
            {
                throw new RepoException("Deze Reservation bestaat al!");
            }
        }

        public IEnumerable<Reservation> GetFutureReservationsForEquipment(int equipmentId)
        {
            try
            {
                // Stap 1: Haal de reserveringen op die gekoppeld zijn aan het opgegeven equipment
                List<ReservationEF> futureReservations = ctx
                    .reservation.Include(rs => rs.Equipment) // Zorg ervoor dat de equipment wordt geladen
                    .Where(rs => rs.equipment_id == equipmentId && rs.date > DateTime.Now)
                    .Include(m => m.Member)
                    .Include(ts => ts.Time_slot)
                    .OrderBy(rs => rs.date)
                    .AsNoTracking()
                    .ToList();

                // Stap 2: Groepeer de reserveringen per reservation_id
                List<IGrouping<int, ReservationEF>> groupedReservations = futureReservations
                    .GroupBy(rs => rs.reservation_id) // Groepeer op basis van reservation_id
                    .ToList(); // Zet het resultaat om naar een lijst van groepen

                // Stap 3: Map de gegroepeerde reserveringen naar het Reservation-domeinmodel
                List<Reservation> reservations = new List<Reservation>();

                foreach (IGrouping<int, ReservationEF> group in groupedReservations)
                {
                    reservations.Add(MapReservation.MapToDomain(group.ToList())); // Map naar je domeinmodel
                }

                return reservations;
            }
            catch (Exception ex)
            {
                throw new RepoException("Error in GetFutureReservationsForEquipment", ex);
            }
        }

        public void UpdateReservationEquipment(Reservation reservation, Equipment oudEquipment)
        {
            try
            {
                List<ReservationEF> rsEFs = MapReservation.MapToEF(reservation);

                ReservationEF reservationEF1 = rsEFs.First();
                ctx.reservation.Update(reservationEF1);

                if (rsEFs.Count() == 2)
                {
                    ReservationEF reservationEF2 = rsEFs.Last();
                    ctx.reservation.Update(reservationEF2);
                }

                EquipmentOnderhoudEF equipmentOnderhoudEF = new EquipmentOnderhoudEF(
                    oudEquipment.Equipment_id
                );
                ctx.equipmentOnderhoud.Add(equipmentOnderhoudEF);

                // Stap 4: Sla de wijzigingen op
                //ctx.reservation.Update(rsEF);
                SaveAndClear();
            }
            catch (Exception ex)
            {
                // Als er een fout optreedt, rollback de transactie en gooi de uitzondering
                throw new RepoException("Error in UpdateReservationEquipment", ex);
            }
        }
    }
}
