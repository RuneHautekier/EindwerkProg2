using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Exceptions;
using FitnessBL.Interfaces;
using FitnessBL.Model;
using FitnessEF.Exceptions;
using FitnessEF.Mappers;
using FitnessEF.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessEF.Repositories
{
    public class ReservationRepo : IReservationRepo
    {
        private FitnessContext ctx;

        public ReservationRepo(string connectionString)
        {
            ctx = new FitnessContext(connectionString);
        }

        private void SaveAndClear()
        {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

        public Reservation GetReservationId(int id)
        {
            try
            {
                List<ReservationEF> reservationsEF = ctx
                    .reservation.Where(x => x.reservation_id == id)
                    .Include(m => m.Member)
                    .Include(e => e.Equipment)
                    .Include(t => t.Time_slot)
                    .AsNoTracking()
                    .ToList();

                if (reservationsEF == null)
                {
                    return null;
                }
                else
                {
                    return MapReservation.MapToDomain(reservationsEF);
                }
            }
            catch (Exception ex)
            {
                throw new RepoException("ReservationRepo - GetReservationId", ex);
            }
        }

        public int GetNieuwReservationId()
        {
            try
            {
                int hoogstBestaandId = ctx.reservation.Max(r => r.reservation_id);
                return hoogstBestaandId + 1;
            }
            catch (Exception ex)
            {
                throw new RepoException("ReservationRepo - GetNieuwReservationId");
            }
        }

        public Reservation AddReservation(Reservation reservation)
        {
            try
            {
                List<ReservationEF> rsEFs = MapReservation.MapToEF(reservation);

                foreach (ReservationEF rsEF in rsEFs)
                {
                    ctx.reservation.Add(rsEF);
                }

                SaveAndClear();
                return reservation;
            }
            catch (Exception ex)
            {
                throw new RepoException("TrainingRepo - AddTraining", ex);
            }
        }
    }
}
