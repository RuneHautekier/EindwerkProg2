using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessEF.Model
{
    public class Runningsession_detailEF
    {
        [Required]
        [ForeignKey("MainSession")]
        public int runningsession_id { get; set; }

        public Runningsession_mainEF MainSession { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int seq_nr { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int interval_time { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int interval_speed { get; set; }

        public Runningsession_detailEF() { }

        public Runningsession_detailEF(
            int runningSessionId,
            int seq_nr,
            int interval_time,
            int interval_speed
        )
        {
            runningsession_id = runningSessionId;
            this.seq_nr = seq_nr;
            this.interval_time = interval_time;
            this.interval_speed = interval_speed;
        }
    }
}
