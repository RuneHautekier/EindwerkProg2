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
    public class Time_slotService
    {
        private ITime_slotRepo time_slotRepo;

        public Time_slotService(ITime_slotRepo time_slotRepo)
        {
            this.time_slotRepo = time_slotRepo;
        }

        public Time_slot GetTime_slotId(int id)
        {
            try
            {
                return time_slotRepo.GetTime_slotId(id);
            }
            catch (Exception ex)
            {
                throw new ServiceException("Time_slotService - GetTime_slotId");
            }
        }
    }
}
