using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Model;
using FitnessEF.Exceptions;
using FitnessEF.Model;

namespace FitnessEF.Mappers
{
    public class MapTime_slot
    {
        public static Time_slot MapToDomain(Time_slotEF time_slotEF)
        {
            try
            {
                return new Time_slot(
                    time_slotEF.time_slot_id,
                    time_slotEF.start_time,
                    time_slotEF.end_time,
                    time_slotEF.part_of_day
                );
            }
            catch (Exception ex)
            {
                throw new MapException("MapTime_slot - MapToDomain", ex);
            }
        }
    }
}
