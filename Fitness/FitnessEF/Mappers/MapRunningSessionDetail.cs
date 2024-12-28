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
    public class MapRunningSessionDetail
    {
        public static Runningsession_detail MapToDomain(Runningsession_detailEF rsdEF)
        {
            try
            {
                return new Runningsession_detail(
                    MapRunningSessionMain.MapToDomain(rsdEF.MainSession),
                    rsdEF.seq_nr,
                    rsdEF.interval_time,
                    rsdEF.interval_speed
                );
            }
            catch (Exception ex)
            {
                throw new MapException("MapRunningSessionDetail - MapToDomain", ex);
            }
        }

        public static Runningsession_detailEF MapToDB(Runningsession_detail rsd)
        {
            try
            {
                return new Runningsession_detailEF(
                    rsd.MainSession.Runningsession_id,
                    rsd.Seq_nr,
                    rsd.Interval_time,
                    rsd.Interval_speed
                );
            }
            catch (Exception ex)
            {
                throw new MapException("MapRunningSessionDetail - MapToDB", ex);
            }
        }
    }
}
