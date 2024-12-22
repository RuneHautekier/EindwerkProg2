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
    }
}
