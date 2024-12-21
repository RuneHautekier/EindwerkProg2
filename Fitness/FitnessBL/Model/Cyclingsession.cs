using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FitnessBL.Model
{
    public class Cyclingsession
    {
        private int cyclingsession_id;
        public int Cyclingsession_id
        {
            get { return cyclingsession_id; }
            set { cyclingsession_id = value; }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                if (value > DateTime.Now)
                {
                    throw new ArgumentException(
                        "De datum van de fietssessie kan niet in de toekomst liggen."
                    );
                }
                date = value;
            }
        }

        private int duration;
        public int Duration
        {
            get { return duration; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException(
                        "De duur van de fietssessie moet groter zijn dan 0."
                    );
                }
                duration = value;
            }
        }

        private int avg_watt;
        public int Avg_watt
        {
            get { return avg_watt; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Het gemiddelde watt kan niet negatief zijn.");
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
                if (value < 0 || value < max_watt)
                {
                    throw new ArgumentException(
                        "Het maximale watt kan niet lager zijn dan het gemiddelde watt of negatief."
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
                    throw new ArgumentException("De gemiddelde cadans kan niet negatief zijn.");
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
                    throw new ArgumentException(
                        "De maximale cadans kan niet lager zijn dan de gemiddelde cadans of negatief."
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
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Het trainingstype mag niet leeg zijn.");
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

        private Member member;
        public Member Member
        {
            get { return member; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("de klant mag niet null zijn.");
                }
                member = value;
            }
        }

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
        {
            Date = datum;
            Duration = duur;
            Avg_cadence = gemiddeldWatt;
            Max_cadence = maximaalWatt;
            Avg_cadence = gemiddeldeCadans;
            Max_cadence = maximaleCadans;
            TrainingsType = trainingsType;
            Comment = opmerking;
            Member = member;
        }

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
        {
            Cyclingsession_id = id;
            Date = datum;
            Duration = duur;
            Avg_cadence = gemiddeldWatt;
            Max_cadence = maximaalWatt;
            Avg_cadence = gemiddeldeCadans;
            Max_cadence = maximaleCadans;
            TrainingsType = trainingsType;
            Comment = opmerking;
            Member = member;
        }
    }
}
