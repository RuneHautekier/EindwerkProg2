using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

                if (reservationsEF.Count() == 0)
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

        public bool IsReservationId(Reservation reservation)
        {
            try
            {
                return ctx.reservation.Any(x => x.reservation_id == reservation.Reservation_id);
            }
            catch (Exception ex)
            {
                throw new RepoException("ReservationRepo - IsReservationId", ex);
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

        public void CheckIfReservationExists(Reservation rs)
        {
            List<ReservationEF> rsEFs = new List<ReservationEF>();
            foreach (Time_slot timeSlot in rs.TimeslotEquipment.Keys)
            {
                rsEFs = ctx
                    .reservation.Where(ts => ts.time_slot_id == timeSlot.Time_slot_id)
                    .Where(e => e.equipment_id == rs.TimeslotEquipment[timeSlot].Equipment_id)
                    .Where(d => d.date == rs.Date)
                    .Where(m => m.member_id == rs.Member.Member_id)
                    .Include(ts => ts.Time_slot)
                    .Include(e => e.Equipment)
                    .Include(m => m.Member)
                    .AsNoTracking()
                    .ToList();
            }

            if (rsEFs.Any())
            {
                throw new RepoException("Deze Reservation bestaat al!");
            }
        }

        //public void UpdateReservationEquipment(Reservation reservation, Equipment oudEquipment)
        //{
        //    try
        //    {
        //        List<ReservationEF> rsEFs = MapReservation.MapToEF(reservation);

        //        ReservationEF reservationEF1 = rsEFs.First();
        //        ctx.reservation.Update(reservationEF1);

        //        if (rsEFs.Count() == 2)
        //        {
        //            ReservationEF reservationEF2 = rsEFs.Last();
        //            ctx.reservation.Update(reservationEF2);
        //        }

        //        SaveAndClear();

        //        EquipmentOnderhoudEF equipmentOnderhoudEF = new EquipmentOnderhoudEF(
        //            oudEquipment.Equipment_id
        //        );
        //        ctx.equipmentOnderhoud.Add(equipmentOnderhoudEF);

        //        SaveAndClear();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new RepoException("Error in UpdateReservationEquipment", ex);
        //    }
        //}

        public void DeleteReservation(Reservation reservation)
        {
            try
            {
                List<ReservationEF> reservationEFs = ctx
                    .reservation.Where(x => x.reservation_id == reservation.Reservation_id)
                    .AsNoTracking()
                    .ToList();

                foreach (ReservationEF rEF in reservationEFs)
                {
                    ctx.reservation.Remove(rEF);
                    SaveAndClear();
                }
            }
            catch (Exception ex)
            {
                throw new RepoException("ReservationRepo - DeleteReservation");
            }
        }
    }
}
