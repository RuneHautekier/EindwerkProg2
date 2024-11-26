using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessBL.Model
{
    public class Tijdslot
    {
        public int Id { get; set; }
        public int StartDatum { get; set; }
        public int EndDatum { get; set; }

        public string DagDeel { get; set; }

        public Tijdslot(int id, int startDatum, int endDatum, string dagDeel)
        {
            Id = id;
            StartDatum = startDatum;
            EndDatum = endDatum;
            DagDeel = dagDeel;
        }
    }
}
