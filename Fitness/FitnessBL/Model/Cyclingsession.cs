using FitnessBL.Exceptions;

namespace FitnessBL.Model
{
    public class Cyclingsession : TrainingSession
    {
        private int cyclingsession_id;
        public int Cyclingsession_id
        {
            get { return cyclingsession_id; }
            set { cyclingsession_id = value; }
        }

        private int avg_watt;
        public int Avg_watt
        {
            get { return avg_watt; }
            set
            {
                if (value < 0)
                {
                    throw new TrainingSessionException(
                        "Het AverageWattage kan niet negatief zijn."
                    );
                }
                avg_watt = value;
            }
        }

        private int max_watt;
        public int Max_watt
        {
            get { return max_watt; }
            set
            {
                if (value < 0 || value < avg_watt)
                {
                    throw new TrainingSessionException(
                        "Het MaxWattage kan niet lager zijn dan het gemiddelde watt of negatief."
                    );
                }
                max_watt = value;
            }
        }

        private int avg_cadence;
        public int Avg_cadence
        {
            get { return avg_cadence; }
            set
            {
                if (value < 0)
                {
                    throw new TrainingSessionException("De AverageCadence kan niet negatief zijn.");
                }
                avg_cadence = value;
            }
        }

        private int max_cadence;
        public int Max_cadence
        {
            get { return max_cadence; }
            set
            {
                if (value < 0 || value < avg_cadence)
                {
                    throw new TrainingSessionException(
                        "De MaxCadance kan niet lager zijn dan de gemiddelde cadans of negatief."
                    );
                }
                max_cadence = value;
            }
        }

        private string trainingsType;
        public string TrainingsType
        {
            get { return trainingsType; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Equals("string"))
                {
                    throw new TrainingSessionException("Het trainingstype mag niet leeg zijn.");
                }
                trainingsType = value;
            }
        }

        private string comment;
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        // Constructor
        // Constructor zonder 'id' parameter, roept de base constructor aan
        public Cyclingsession(
            DateTime datum,
            int duur,
            int gemiddeldWatt,
            int maximaalWatt,
            int gemiddeldeCadans,
            int maximaleCadans,
            string trainingsType,
            string opmerking,
            Member member
        )
            : base(datum, duur, member) // Roep de constructor van de basisklasse aan
        {
            Avg_watt = gemiddeldWatt;
            Max_watt = maximaalWatt;
            Avg_cadence = gemiddeldeCadans;
            Max_cadence = maximaleCadans;
            TrainingsType = trainingsType;
            Comment = opmerking;
        }

        // Constructor met 'id' parameter, roept de base constructor aan
        public Cyclingsession(
            int id,
            DateTime datum,
            int duur,
            int gemiddeldWatt,
            int maximaalWatt,
            int gemiddeldeCadans,
            int maximaleCadans,
            string trainingsType,
            string opmerking,
            Member member
        )
            : base(datum, duur, member) // Roep de constructor van de basisklasse aan
        {
            Cyclingsession_id = id;
            Avg_watt = gemiddeldWatt;
            Max_watt = maximaalWatt;
            Avg_cadence = gemiddeldeCadans;
            Max_cadence = maximaleCadans;
            TrainingsType = trainingsType;
            Comment = opmerking;
        }
    }
}
