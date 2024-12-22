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

        public Equipment GetEquipmentId(int id)
        {
            try
            {
                return equipmentRepo.GetEquipmentId(id);
            }
            catch (Exception ex)
            {
                throw new ServiceException("EquipmentService - GetEquipmentId");
            }
        }
    }
}
