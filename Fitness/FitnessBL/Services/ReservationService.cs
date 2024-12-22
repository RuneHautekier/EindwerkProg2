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
    public class ReservationService
    {
        private IReservationRepo reservationRepo;

        public ReservationService(IReservationRepo reservationRepo)
        {
            this.reservationRepo = reservationRepo;
        }

        public Reservation GetReservationId(int id)
        {
            try
            {
                return reservationRepo.GetReservationId(id);
            }
            catch (Exception ex)
            {
                throw new ServiceException("ReservationService - GetReservationId");
            }
        }

        public int GetNieuwReservationId()
        {
            try
            {
                return reservationRepo.GetNieuwReservationId();
            }
            catch (Exception ex)
            {
                throw new ServiceException("ReservationService - GetNieuwReservationId");
            }
        }

        public Reservation AddReservation(Reservation reservation)
        {
            try
            {
                if (reservation == null)
                    throw new ServiceException("AddTraining - Training is null");

                reservationRepo.AddReservation(reservation);
                return reservation;
            }
            catch (Exception ex)
            {
                throw new ServiceException("AddTraining", ex);
            }
        }
    }
}
