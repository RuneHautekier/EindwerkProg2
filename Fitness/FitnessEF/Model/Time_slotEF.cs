using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessEF.Model
{
    public class Time_slotEF
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int time_slot_id { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int start_time { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int end_time { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public string part_of_day { get; set; }

        public Time_slotEF() { }

        public Time_slotEF(int start_time, int end_time, string part_of_day)
        {
            this.start_time = start_time;
            this.end_time = end_time;
            this.part_of_day = part_of_day;
        }

        public Time_slotEF(int time_slot_id, int start_time, int end_time, string part_of_day)
        {
            this.time_slot_id = time_slot_id;
            this.start_time = start_time;
            this.end_time = end_time;
            this.part_of_day = part_of_day;
        }
    }
}
