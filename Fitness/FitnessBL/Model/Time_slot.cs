using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessBL.Model
{
    public class Time_slot
    {
        public int Time_slot_id { get; set; }
        public int Start_time { get; set; }
        public int End_time { get; set; }

        public string Part_of_day { get; set; }

        public Time_slot(int id, int startDatum, int endDatum, string dagDeel)
        {
            Time_slot_id = id;
            Start_time = startDatum;
            End_time = endDatum;
            Part_of_day = dagDeel;
        }

        public override string? ToString()
        {
            return $"Id: {Time_slot_id}, Hours: {Start_time} - {End_time}";
        }
    }
}
