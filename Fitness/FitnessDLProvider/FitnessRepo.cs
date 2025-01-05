using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Interfaces;
using FitnessEF.Repositories;

namespace FitnessDLProvider
{
    public class FitnessRepo
    {
        public IMemberRepo memberRepo { get; }
        public IEquipmentRepo equipmentRepo { get; }
        public ITime_slotRepo timeSlotRepo { get; }
        public IReservationRepo reservationRepo { get; }
        public IProgramRepo programRepo { get; }

        public FitnessRepo(string connectionString, string repoType)
        {
            try
            {
                switch (repoType)
                {
                    case "EFCore":
                        memberRepo = new MemberRepo(connectionString);
                        equipmentRepo = new EquipmentRepo(connectionString);
                        timeSlotRepo = new Time_slotRepo(connectionString);
                        reservationRepo = new ReservationRepo(connectionString);
                        programRepo = new ProgramRepo(connectionString);
                        break;
                    default:
                        throw new Exception();
                }
            }
            catch (Exception ex) { }
        }
    }
}
