using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Model;

namespace FitnessEF.Model
{
    public class ReservationEF
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int reservation_id { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime date { get; set; }

        [Required]
        [ForeignKey("Equipment")]
        public int equipment_id { get; set; }
        public virtual EquipmentEF Equipment { get; set; }

        [Required]
        [ForeignKey("Member")]
        public int member_id { get; set; }
        public virtual MemberEF Member { get; set; }

        [Required]
        [ForeignKey("Time_slot")]
        public int time_slot_id { get; set; }

        public virtual Time_slotEF Time_slot { get; set; }

        public ReservationEF() { }

        public ReservationEF(DateTime date, int equipment_id, int member_id, int time_slot_id)
        {
            this.date = date;
            this.equipment_id = equipment_id;
            this.member_id = member_id;
            this.time_slot_id = time_slot_id;
        }

        public ReservationEF(
            int reservation_id,
            DateTime date,
            int equipment_id,
            int member_id,
            int time_slot_id
        )
        {
            this.reservation_id = reservation_id;
            this.date = date;
            this.equipment_id = equipment_id;
            this.member_id = member_id;
            this.time_slot_id = time_slot_id;
        }
    }
}
