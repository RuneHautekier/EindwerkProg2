using System;
using FitnessBL.Model;
using FitnessEF.Model;

namespace FitnessEF.Mappers
{
    public class FietsSessieMapper
    {
        // Map de domeinlaag FietsSessie naar EF-klasse FietsSessieEF
        public static CyclingSessionEF MapToDb(Cyclingsession fietsSessie)
        {
            if (fietsSessie == null)
                throw new ArgumentNullException(nameof(fietsSessie));

            return new CyclingSessionEF(
                fietsSessie.Date,
                fietsSessie.Duration,
                fietsSessie.Avg_watt,
                fietsSessie.Max_watt,
                fietsSessie.Avg_cadence,
                fietsSessie.Max_cadence,
                fietsSessie.TrainingsType,
                fietsSessie.Comment,
                fietsSessie.Member.Member_id // Verwijzing naar het klant ID
            );
        }

        // Map de EF-klasse FietsSessieEF naar de domeinlaag FietsSessie
        public static Cyclingsession MapToDomain(CyclingSessionEF fietsSessieEF, Member klant)
        {
            if (fietsSessieEF == null)
                throw new ArgumentNullException(nameof(fietsSessieEF));
            if (klant == null)
                throw new ArgumentNullException(nameof(klant));

            return new Cyclingsession(
                fietsSessieEF.date,
                fietsSessieEF.duration,
                fietsSessieEF.avg_watt,
                fietsSessieEF.max_watt,
                fietsSessieEF.avg_cadence,
                fietsSessieEF.max_cadence,
                fietsSessieEF.trainingtype,
                fietsSessieEF.comment,
                klant
            );
        }
    }
}
