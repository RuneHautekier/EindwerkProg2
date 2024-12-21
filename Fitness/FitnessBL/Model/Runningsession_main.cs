using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FitnessBL.Model
{
    public class Runningsession_main
    {
        private int runningsession_id;
        public int Runningsession_id
        {
            get { return runningsession_id; }
            set { runningsession_id = value; }
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
                        "De datum van de hardloopsessie kan niet in de toekomst liggen."
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
                        "De duur van de hardloopsessie moet groter zijn dan 0."
                    );
                }
                duration = value;
            }
        }

        private float avg_speed;
        public float Avg_speed
        {
            get { return avg_speed; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("De gemiddelde snelheid kan niet negatief zijn.");
                }
                avg_speed = value;
            }
        }

        private Member member;
        public Member Member
        {
            get { return member; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("De klant mag niet null zijn.");
                }
                member = value;
            }
        }

        public Runningsession_main(DateTime datum, int duur, float gemiddeldeSnelheid, Member klant)
        {
            Date = datum;
            Duration = duur;
            Avg_speed = gemiddeldeSnelheid;
            Member = klant;
        }

        public Runningsession_main(
            int id,
            DateTime datum,
            int duur,
            float gemiddeldeSnelheid,
            Member klant
        )
        {
            Runningsession_id = id;
            Date = datum;
            Duration = duur;
            Avg_speed = gemiddeldeSnelheid;
            Member = klant;
        }
    }
}
