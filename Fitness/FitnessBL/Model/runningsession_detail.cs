using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Exceptions;

namespace FitnessBL.Model
{
    public class Runningsession_detail
    {
        public Runningsession_main MainSession { get; set; }
        public int Seq_nr { get; set; }
        private int interval_time;
        public int Interval_time
        {
            get { return interval_time; }
            set
            {
                if (value <= 0)
                    throw new RunningSessionDetailException(
                        "IntervalTime moet groter zijn dan 0!."
                    );
                interval_time = value;
            }
        }
        private int interval_speed;

        public int Interval_speed
        {
            get { return interval_speed; }
            set
            {
                if (value <= 0)
                    throw new RunningSessionDetailException(
                        "IntervalSpeed moet groter zijn dan 0!."
                    );
                interval_speed = value;
            }
        }

        public Runningsession_detail(
            Runningsession_main mainSession,
            int seq_nr,
            int interval_time,
            int interval_speed
        )
        {
            MainSession = mainSession;
            Seq_nr = seq_nr;
            Interval_time = interval_time;
            Interval_speed = interval_speed;
        }
    }
}
