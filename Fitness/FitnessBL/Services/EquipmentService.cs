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
        private IEquipmentRepo equipmentRepo;

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
            return equipmentRepo.GetEquipmentId(id);
        }

        public IEnumerable<Equipment> GetEquipmentsType(string type)
        {
            IEnumerable<Equipment> equipments = equipmentRepo.GetEquipmentsType(type);
            if (equipments.Count() == 0)
                throw new ServiceException(
                    "EquipmentService - GetEqupimentType - Er is geen equipment van dit type!"
                );
            return equipments;
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

        public Equipment UpdateEquipment(Equipment equipment)
        {
            if (equipment == null)
                throw new ServiceException(
                    "EquipmentService - UpdateEquipment - equipment is null"
                );
            if (!equipmentRepo.IsEquipmentId(equipment))
                throw new ServiceException(
                    "EquipmentService - UpdateEquipment - equipment bestaat niet op id"
                );
            if (equipment.Device_type.Equals("string"))
                throw new ServiceException(
                    "EquipmentService - UpdateEquipment - Gelieve het type van het equipment in te vullen!"
                );
            equipmentRepo.UpdateEquipment(equipment);
            return equipment;
        }

        public void DeleteEquipment(Equipment equipment)
        {
            if (!equipmentRepo.IsEquipmentId(equipment))
                throw new ServiceException(
                    "EquipmentService - DeleteEquipment - equipment bestaat niet op id"
                );
            equipmentRepo.DeleteEquipment(equipment);
        }

        public void EquipmentPlaatsOnderhoud(Equipment equipment)
        {
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

        public void EquipmentVerwijderOnderhoud(Equipment equipment)
        {
            if (!equipmentRepo.IsEquipmentId(equipment))
                throw new ServiceException(
                    "EquipmentService - EquipmentPlaatsOnderhoud - equipment bestaat niet op id"
                );
            if (!equipmentRepo.EquipmentInOnderhoud(equipment))
                throw new ServiceException(
                    "EquipmentService - EquipmentPlaatsOnderhoud - Equipment zit niet in onderhoud!"
                );
            equipmentRepo.EquipmentVerwijderOnderhoud(equipment);
        }
    }
}
