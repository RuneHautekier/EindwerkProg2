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

        public IEnumerable<Equipment> GetEquipmentsType(string type)
        {
            try
            {
                List<EquipmentEF> equipmentEF = ctx
                    .equipment.Where(x => x.device_type == type)
                    .ToList();

                List<Equipment> equipments = new();
                foreach (EquipmentEF eEF in equipmentEF)
                {
                    equipments.Add(MapEquipment.MapToDomain(eEF));
                }
                return equipments;
            }
            catch (Exception ex)
            {
                throw new RepoException("EquipmentRepo - GetEquipmentsType");
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

        public bool IsEquipmentId(int id)
        {
            try
            {
                return ctx.equipment.Any(x => x.equipment_id == id);
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

        public void DeleteEquipment(int id)
        {
            try
            {
                EquipmentEF equipmentEF = ctx.equipment.FirstOrDefault(x => x.equipment_id == id);
                ctx.equipment.Remove(equipmentEF);
                SaveAndClear();
            }
            catch (Exception ex)
            {
                throw new RepoException("EquipmentRepo - DeleteEquipment");
            }
        }
    }
}
