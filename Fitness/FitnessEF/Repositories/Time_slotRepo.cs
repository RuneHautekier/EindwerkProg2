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
    public class Time_slotRepo : ITime_slotRepo
    {
        private FitnessContext ctx;

        public Time_slotRepo(string connectionString)
        {
            ctx = new FitnessContext(connectionString);
        }

        private void SaveAndClear()
        {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

        public IEnumerable<Time_slot> GetTimeSlots()
        {
            try
            {
                IEnumerable<Time_slotEF> tsEFs = ctx.time_slot.Select(x => x).ToList();

                List<Time_slot> timeSlots = new List<Time_slot>();
                foreach (Time_slotEF tsEF in tsEFs)
                {
                    Time_slot ts = MapTime_slot.MapToDomain(tsEF);
                    timeSlots.Add(ts);
                }

                return timeSlots;
            }
            catch (Exception ex)
            {
                throw new RepoException("Time_slotRepo - GetTimeSlots");
            }
        }

        public Time_slot GetTime_slotId(int id)
        {
            try
            {
                Time_slotEF time_slotEF = ctx
                    .time_slot.Where(x => x.time_slot_id == id)
                    .AsNoTracking()
                    .FirstOrDefault();

                if (time_slotEF == null)
                {
                    return null;
                }
                else
                {
                    return MapTime_slot.MapToDomain(time_slotEF);
                }
            }
            catch (Exception ex)
            {
                throw new RepoException("Time_slotRepo - GetTime_slotId", ex);
            }
        }
    }
}
