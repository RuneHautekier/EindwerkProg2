using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Model;

namespace FitnessBL.Interfaces
{
    public interface IReservationRepo
    {
        Reservation GetReservationId(int id);
        bool IsReservationId(Reservation reservation);
        int GetNieuwReservationId();
        Reservation AddReservation(Reservation reservation);
        void CheckIfReservationExists(Reservation reservation);

        //void UpdateReservationEquipment(Reservation reservation, Reservation oudeReservation);
        void DeleteReservation(Reservation reservation);
    }
}
