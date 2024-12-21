using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessBL.Model
{
    public class runningsession_detail
    {
        public Runningsession_main MainSession { get; set; }
        public int Seq_nr { get; set; }
        public int Interval_time { get; set; }
        public int Interval_speed { get; set; }

        public runningsession_detail(
            Runningsession_main mainSession,
            int seq_nr,
            int interval_time,
            int interval_speed
        )
        {
            MainSession = mainSession;
            Seq_nr = seq_nr;
            Interval_time = interval_time;
            interval_speed = interval_speed;
        }
    }
}
