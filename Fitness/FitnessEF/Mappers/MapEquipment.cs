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
    public class MapEquipment
    {
        public static Equipment MapToDomain(EquipmentEF equipmentEF)
        {
            try
            {
                return new Equipment(equipmentEF.equipment_id, equipmentEF.device_type);
            }
            catch (Exception ex)
            {
                throw new MapException("MapEquipment - MapToDomain", ex);
            }
        }

        public static EquipmentEF MapToDB(Equipment equipment)
        {
            try
            {
                return new EquipmentEF(equipment.Equipment_id, equipment.Device_type);
            }
            catch (Exception ex)
            {
                throw new MapException("MapEquipment - MapToDB", ex);
            }
        }
    }
}
