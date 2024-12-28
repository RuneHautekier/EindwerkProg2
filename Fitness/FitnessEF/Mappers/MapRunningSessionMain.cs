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
    public class MapRunningSessionMain
    {
        public static Runningsession_main MapToDomain(Runningsession_mainEF rsmEF)
        {
            try
            {
                return new Runningsession_main(
                    rsmEF.runningsession_id,
                    rsmEF.date,
                    rsmEF.duration,
                    rsmEF.avg_speed,
                    MapMember.MapToDomain(rsmEF.Member)
                );
            }
            catch (Exception ex)
            {
                throw new MapException("MapRunningSessionMain - MapToDomain", ex);
            }
        }

        public static Runningsession_mainEF MapToDB(Runningsession_main rsm)
        {
            try
            {
                return new Runningsession_mainEF(
                    rsm.Runningsession_id,
                    rsm.Date,
                    rsm.Duration,
                    rsm.Avg_speed,
                    rsm.Member.Member_id
                );
            }
            catch (Exception ex)
            {
                throw new MapException("MapRunningSessionMain - MapToDB", ex);
            }
        }
    }
}
