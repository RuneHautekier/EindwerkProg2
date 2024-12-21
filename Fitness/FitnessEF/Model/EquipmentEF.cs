using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessEF.Model
{
    public class EquipmentEF
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int equipment_id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(45)")]
        public string device_type { get; set; }

        public EquipmentEF() { }

        public EquipmentEF(string device_type)
        {
            this.device_type = device_type;
        }

        public EquipmentEF(int equipment_id, string device_type)
        {
            this.equipment_id = equipment_id;
            this.device_type = device_type;
        }
    }
}
