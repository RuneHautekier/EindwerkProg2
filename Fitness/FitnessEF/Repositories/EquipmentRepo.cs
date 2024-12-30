using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Interfaces;
using FitnessBL.Model;
using FitnessEF.Exceptions;
using FitnessEF.Mappers;
using FitnessEF.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FitnessEF.Repositories
{
    public class EquipmentRepo : IEquipmentRepo
    {
        private FitnessContext ctx;

        public EquipmentRepo(string connectionString)
        {
            ctx = new FitnessContext(connectionString);
        }

        private void SaveAndClear()
        {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

        public IEnumerable<Equipment> GetEquipment()
        {
            try
            {
                List<EquipmentEF> equipmentEF = ctx.equipment.Select(x => x).ToList();
                List<Equipment> equipments = new();
                foreach (EquipmentEF eEF in equipmentEF)
                {
                    equipments.Add(MapEquipment.MapToDomain(eEF));
                }
                return equipments;
            }
            catch (Exception ex)
            {
                throw new RepoException("EquipmentRepo - GetEquipment");
            }
        }

        public Equipment GetEquipmentId(int id)
        {
            try
            {
                EquipmentEF equipmentEF = ctx
                    .equipment.Where(x => x.equipment_id == id)
                    .AsNoTracking()
                    .FirstOrDefault();

                if (equipmentEF == null)
                {
                    return null;
                }
                else
                {
                    return MapEquipment.MapToDomain(equipmentEF);
                }
            }
            catch (Exception ex)
            {
                throw new RepoException("EquipmentRepo - GetEquipomentId", ex);
            }
        }

        public Equipment AddEquipment(Equipment equipment)
        {
            try
            {
                EquipmentEF eEF = MapEquipment.MapToDB(equipment);
                ctx.equipment.Add(eEF);
                SaveAndClear();
                equipment.Equipment_id = eEF.equipment_id;
                return equipment;
            }
            catch (Exception ex)
            {
                throw new RepoException("EquipmentRepo - AddEquipment", ex);
            }
        }

        public bool IsEquipmentId(Equipment equipment)
        {
            try
            {
                return ctx.equipment.Any(x => x.equipment_id == equipment.Equipment_id);
            }
            catch (Exception ex)
            {
                throw new RepoException("EquipmentRepo - IsEquipmentID", ex);
            }
        }

        public void UpdateEquipment(Equipment equipment)
        {
            try
            {
                ctx.equipment.Update(MapEquipment.MapToDB(equipment));
                SaveAndClear();
            }
            catch (Exception ex)
            {
                throw new RepoException("EquipmentRepo - UpdateEquipment");
            }
        }

        public void DeleteEquipment(Equipment equipment)
        {
            try
            {
                EquipmentEF equipmentEF = ctx.equipment.FirstOrDefault(x =>
                    x.equipment_id == equipment.Equipment_id
                );
                ctx.equipment.Remove(equipmentEF);
                SaveAndClear();
            }
            catch (Exception ex)
            {
                throw new RepoException("EquipmentRepo - DeleteEquipment");
            }
        }

        public void EquipmentPlaatsOnderhoud(Equipment equipment)
        {
            try
            {
                EquipmentOnderhoudEF eoEF = new EquipmentOnderhoudEF(equipment.Equipment_id);
                ctx.equipmentOnderhoud.Add(eoEF);
                SaveAndClear();
            }
            catch (Exception ex)
            {
                throw new RepoException("EquipmentRepo - EquipmentPlaatsOnderhoud");
            }
        }

        public void EquipmentVerwijderOnderhoud(Equipment equipment)
        {
            try
            {
                EquipmentOnderhoudEF eoEF = ctx
                    .equipmentOnderhoud.Where(eo => eo.equipment_id == equipment.Equipment_id)
                    .AsNoTracking()
                    .FirstOrDefault();

                ctx.equipmentOnderhoud.Remove(eoEF);
                SaveAndClear();
            }
            catch (Exception ex)
            {
                throw new RepoException("EquipmentRepo - EquipmentVerwijderOnderhoud");
            }
        }

        public bool EquipmentInOnderhoud(Equipment equipment)
        {
            try
            {
                return ctx.equipmentOnderhoud.Any(e => e.equipment_id == equipment.Equipment_id);
            }
            catch (Exception ex)
            {
                throw new RepoException("EquipmentRepo - EquipmentInOnderhoud");
            }
        }

        public Equipment GetAvailableEquipment(DateTime date, Time_slot timeSlot, string DeviceType)
        {
            try
            {
                // Stap 1: Haal alle equipment op die mogelijk beschikbaar zijn op de opgegeven datum en tijdslot
                EquipmentEF availableEquipment = ctx
                    .equipment.Where(e =>
                        e.device_type == DeviceType // Filter op het apparaat type
                        && !ctx.reservation.Any(r =>
                            r.equipment_id == e.equipment_id // Controleer of het equipment niet al gereserveerd is
                            && r.date == date
                            && r.time_slot_id == timeSlot.Time_slot_id
                        )
                    )
                    .AsNoTracking()
                    .FirstOrDefault(); // Haal het eerste (beschikbare) equipment op, of null als er geen is

                if (availableEquipment == null)
                {
                    return null;
                }

                // Stap 2: Map het gevonden equipment naar het domeinmodel
                Equipment equipment = MapEquipment.MapToDomain(availableEquipment);

                return equipment;
            }
            catch (Exception ex)
            {
                throw new RepoException("Error in GetAvailableEquipmentWithTransaction", ex);
            }
        }

        public IEnumerable<Reservation> GetFutureReservationsForEquipment(Equipment equipment)
        {
            try
            {
                // Stap 1: Haal de reserveringen op die gekoppeld zijn aan het opgegeven equipment
                List<ReservationEF> futureReservations = ctx
                    .reservation.Include(rs => rs.Equipment) // Zorg ervoor dat de equipment wordt geladen
                    .Where(rs =>
                        rs.equipment_id == equipment.Equipment_id && rs.date > DateTime.Now
                    )
                    .Include(m => m.Member)
                    .Include(ts => ts.Time_slot)
                    .OrderBy(rs => rs.date)
                    .AsNoTracking()
                    .ToList();

                if (futureReservations == null)
                {
                    return new List<Reservation>();
                }

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

        public IDbContextTransaction BeginTransaction()
        {
            return ctx.Database.BeginTransaction();
        }
    }
}
