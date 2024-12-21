using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessEF.Model
{
    public class Runningsession_mainEF
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int runningsession_id { get; set; }

        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime date { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int duration { get; set; }

        [Required]
        [Column(TypeName = "float")]
        public float avg_speed { get; set; }

        [Required]
        [ForeignKey("Member")]
        public int member_id { get; set; }

        public virtual MemberEF Member { get; set; }

        public Runningsession_mainEF() { }

        public Runningsession_mainEF(DateTime date, int duration, float avg_speed, int member_id)
        {
            this.date = date;
            this.duration = duration;
            this.avg_speed = avg_speed;
            this.member_id = member_id;
        }

        public Runningsession_mainEF(
            int id,
            DateTime date,
            int duration,
            float avg_speed,
            int member_id
        )
        {
            this.runningsession_id = id;
            this.date = date;
            this.duration = duration;
            this.avg_speed = avg_speed;
            this.member_id = member_id;
        }
    }
}
