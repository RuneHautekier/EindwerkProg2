using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Exceptions;
using FitnessBL.Interfaces;
using FitnessBL.Model;

namespace FitnessBL.Services
{
    public class EquipmentService
    {
        public IEquipmentRepo equipmentRepo;

        public EquipmentService(IEquipmentRepo equipmentRepo)
        {
            this.equipmentRepo = equipmentRepo;
        }

        public IEnumerable<Equipment> GetEquipment()
        {
            IEnumerable<Equipment> equipment = equipmentRepo.GetEquipment();
            if (equipment.Count() == 0)
                throw new ServiceException("Er zit nog geen equipment in de database!");
            return equipment;
        }

        public Equipment GetEquipmentId(int id)
        {
            Equipment equipment = equipmentRepo.GetEquipmentId(id);
            if (equipment == null)
                throw new ServiceException(
                    "EquipmentService - GetEquipmentId - Er is geen equipment met dit id!"
                );
            return equipment;
        }

        public Equipment AddEquipment(Equipment equipment)
        {
            if (equipment == null)
                throw new ServiceException("EquipmentService - AddEquipment - Equipment is null");
            if (equipment.Device_type.Equals("string"))
                throw new ServiceException(
                    "EquipmentService - AddEquipment - Gelieve het type van het equipment in te vullen!"
                );
            equipmentRepo.AddEquipment(equipment);
            return equipment;
        }

        public void DeleteEquipment(Equipment equipment)
        {
            if (equipment == null)
                throw new ServiceException(
                    "EquipmentService - DeleteEquipment - equipment is null"
                );
            if (!equipmentRepo.IsEquipmentId(equipment))
                throw new ServiceException(
                    "EquipmentService - DeleteEquipment - equipment bestaat niet op id"
                );
            if (GetFutureReservationsForEquipment(equipment).Count() != 0)
                throw new ServiceException(
                    "EquipmentService - DeleteEquipment - equipment kan niet verwijderd worden want heeft nog reservations!"
                );
            equipmentRepo.DeleteEquipment(equipment);
        }

        public IEnumerable<Reservation> GetFutureReservationsForEquipment(Equipment equipment)
        {
            if (equipment == null)
                throw new ServiceException(
                    "EquipmentService - GetFutureReservationsForEquipment - equipment is null"
                );
            if (!equipmentRepo.IsEquipmentId(equipment))
                throw new ServiceException(
                    "EquipmentService - GetFutureReservationsForEquipment - equipment bestaat niet op id"
                );

            IEnumerable<Reservation> reservations = equipmentRepo.GetFutureReservationsForEquipment(
                equipment
            );
            return reservations;
        }

        public void EquipmentPlaatsOnderhoud(Equipment equipment)
        {
            if (equipment == null)
                throw new ServiceException(
                    "EquipmentService - EquipmentPlaatsOnderhoud - equipment is null"
                );

            if (!equipmentRepo.IsEquipmentId(equipment))
                throw new ServiceException(
                    "EquipmentService - EquipmentPlaatsOnderhoud - equipment bestaat niet op id"
                );
            if (equipmentRepo.EquipmentInOnderhoud(equipment))
                throw new ServiceException(
                    "EquipmentService - EquipmentPlaatsOnderhoud - Equipment zit al in onderhoud!"
                );

            equipmentRepo.EquipmentPlaatsOnderhoud(equipment);
        }

        public void EquipmentVerwijderOnderhoud(Equipment equipment, DateTime StartDate)
        {
            if (equipment == null)
                throw new ServiceException(
                    "EquipmentService - EquipmentVerwijderOnderhoud - equipment is null"
                );
            if (!equipmentRepo.IsEquipmentId(equipment))
                throw new ServiceException(
                    "EquipmentService - EquipmentVerwijderOnderhoud - equipment bestaat niet op id"
                );
            if (!equipmentRepo.EquipmentInOnderhoud(equipment))
                throw new ServiceException(
                    "EquipmentService - EquipmentVerwijderOnderhoud - Equipment zit niet in onderhoud!"
                );
            equipmentRepo.EquipmentVerwijderOnderhoud(equipment);
        }

        public bool EquipmentInOnderhoud(Equipment equipment)
        {
            if (equipment == null)
                throw new ServiceException(
                    "EquipmentService - EquipmentInOnderhoud - equipment is null"
                );

            if (!equipmentRepo.IsEquipmentId(equipment))
                throw new ServiceException(
                    "EquipmentService - EquipmentInOnderhoud - equipment bestaat niet op id"
                );

            return equipmentRepo.EquipmentInOnderhoud(equipment);
        }

        public Equipment GetAvailableEquipment(DateTime date, Time_slot timeSlot, string DeviceType)
        {
            if (date < DateTime.Now)
                throw new ServiceException(
                    "EquipmentServie - GetAvailableEquipment - Date moet in de toekomst liggen om te zien of dit equipment in de toekomst al gebruikt wordt!"
                );
            if (timeSlot == null)
                throw new ServiceException(
                    "EquipmentServie - GetAvailableEquipment - TimeSlot is null!"
                );
            if (DeviceType == null)
                throw new ServiceException(
                    "EquipmentServie - GetAvailableEquipment - DeviceType is null!"
                );

            return equipmentRepo.GetAvailableEquipment(date, timeSlot, DeviceType);
        }

        public IEnumerable<Equipment> GetAllAvailableEquipment(DateTime date, int timeSlotId)
        {
            if (date < DateTime.Now)
                throw new ServiceException(
                    "EquipmentService - GetAvailableEquipment - Date moet in de toekomst liggen om te zien of dit equipment in de toekomst al gebruikt wordt!"
                );
            if (timeSlotId <= 0 || timeSlotId > 14)
                throw new ServiceException(
                    "EquipmentService - GetAvailableEquipment - TimeSlot id ligt niet in de range!"
                );

            return equipmentRepo.GetAllAvailableEquipment(date, timeSlotId);
        }
    }
}
