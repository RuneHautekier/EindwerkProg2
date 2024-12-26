using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Model;
using FitnessEF.Model;

namespace FitnessEF.Mappers
{
    public class MapCyclingSession
    {
        public static CyclingSessionEF MapToDB(Cyclingsession cs)
        {
            if (cs == null)
                throw new ArgumentNullException(nameof(cs));

            return new CyclingSessionEF(
                cs.Date,
                cs.Duration,
                cs.Avg_watt,
                cs.Max_watt,
                cs.Avg_cadence,
                cs.Max_cadence,
                cs.TrainingsType,
                cs.Comment,
                cs.Member.Member_id
            );
        }

        // Map de EF-klasse FietsSessieEF naar de domeinlaag FietsSessie
        public static Cyclingsession MapToDomain(CyclingSessionEF csEF)
        {
            if (csEF == null)
                throw new ArgumentNullException(nameof(csEF));

            return new Cyclingsession(
                csEF.cyclingsession_id,
                csEF.date,
                csEF.duration,
                csEF.avg_watt,
                csEF.max_watt,
                csEF.avg_cadence,
                csEF.max_cadence,
                csEF.trainingtype,
                csEF.comment,
                MapMember.MapToDomain(csEF.member)
            );
        }
    }
}
