using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Model;

namespace FitnessBL.Interfaces
{
    public interface IEquipmentRepo
    {
        IEnumerable<Equipment> GetEquipment();

        Equipment GetEquipmentId(int id);
        IEnumerable<Equipment> GetEquipmentsType(string type);
        Equipment AddEquipment(Equipment equipment);
        bool IsEquipmentId(int id);

        void UpdateEquipment(Equipment equipment);
        void DeleteEquipment(int id);
    }
}
