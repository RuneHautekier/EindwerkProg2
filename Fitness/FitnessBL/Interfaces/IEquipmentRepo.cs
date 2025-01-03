using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Model;
using Microsoft.EntityFrameworkCore.Storage;

namespace FitnessBL.Interfaces
{
    public interface IEquipmentRepo
    {
        IEnumerable<Equipment> GetEquipment();

        Equipment GetEquipmentId(int id);
        Equipment AddEquipment(Equipment equipment);
        bool IsEquipmentId(Equipment equipment);
        void DeleteEquipment(Equipment equipment);
        void EquipmentPlaatsOnderhoud(Equipment equipment);
        void EquipmentVerwijderOnderhoud(Equipment equipment);
        bool EquipmentInOnderhoud(Equipment equipment);
        Equipment GetAvailableEquipment(DateTime date, Time_slot timeSlot, string DeviceType);
        IEnumerable<Reservation> GetFutureReservationsForEquipment(Equipment equipment);
        IEnumerable<Equipment> GetAllAvailableEquipment(DateTime date, int timeSlotID);
        IDbContextTransaction BeginTransaction();
    }
}
