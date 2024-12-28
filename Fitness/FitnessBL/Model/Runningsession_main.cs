using FitnessBL.Exceptions;

namespace FitnessBL.Model
{
    public class Runningsession_main : TrainingSession
    {
        private int runningsession_id;
        public int Runningsession_id
        {
            get { return runningsession_id; }
            set { runningsession_id = value; }
        }

        private float avg_speed;
        public float Avg_speed
        {
            get { return avg_speed; }
            set
            {
                if (value <= 0)
                {
                    throw new TrainingSessionException(
                        "De gemiddelde snelheid moet groter zijn dan 0."
                    );
                }
                avg_speed = value;
            }
        }

        // Constructor
        // Constructor zonder 'id' parameter, roept de base constructor aan
        public Runningsession_main(DateTime datum, int duur, float gemiddeldeSnelheid, Member klant)
            : base(datum, duur, klant) // Roep de constructor van de basisklasse aan
        {
            Avg_speed = gemiddeldeSnelheid;
        }

        // Constructor met 'id' parameter, roept de base constructor aan
        public Runningsession_main(
            int id,
            DateTime datum,
            int duur,
            float gemiddeldeSnelheid,
            Member klant
        )
            : base(datum, duur, klant) // Roep de constructor van de basisklasse aan
        {
            Runningsession_id = id;
            Avg_speed = gemiddeldeSnelheid;
        }
    }
}
