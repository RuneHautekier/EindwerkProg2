using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessEF.Model
{
    public class CyclingSessionEF
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int cyclingsession_id { get; set; } // Vroeger: FietsSessieId

        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime date { get; set; } // Vroeger: Datum

        [Required]
        public int duration { get; set; } // Vroeger: Duur

        [Required]
        public int avg_watt { get; set; } // Vroeger: GemiddeldWatt

        [Required]
        public int max_watt { get; set; } // Vroeger: MaximaalWatt

        [Required]
        public int avg_cadence { get; set; } // Vroeger: GemiddeldeCadans

        [Required]
        public int max_cadence { get; set; } // Vroeger: MaximaleCadans

        [Required]
        [Column(TypeName = "nvarchar(45)")]
        public string trainingtype { get; set; } // Vroeger: TrainingsType

        [Column(TypeName = "nvarchar(500)")]
        public string? comment { get; set; } // Vroeger: Opmerking

        [ForeignKey("member")]
        public int member_id { get; set; } // Vroeger: KlantId

        // Navigatie eigenschap naar de bijbehorende Member
        public virtual MemberEF member { get; set; } // Vroeger: Klant

        public CyclingSessionEF() { }

        public CyclingSessionEF(
            DateTime date,
            int duration,
            int avg_watt,
            int max_watt,
            int avg_cadence,
            int max_cadence,
            string trainingtype,
            string comment,
            int member_id
        )
        {
            this.date = date;
            this.duration = duration;
            this.avg_watt = avg_watt;
            this.max_watt = max_watt;
            this.avg_cadence = avg_cadence;
            this.max_cadence = max_cadence;
            this.trainingtype = trainingtype;
            this.comment = comment;
            this.member_id = member_id;
        }
    }
}
