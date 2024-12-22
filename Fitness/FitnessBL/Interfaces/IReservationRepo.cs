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
        int GetNieuwReservationId();
        Reservation AddReservation(Reservation reservation);
    }
}
