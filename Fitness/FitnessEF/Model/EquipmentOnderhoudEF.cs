using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessEF.Model
{
    public class EquipmentOnderhoudEF
    {
        [Key]
        public int equipment_id { get; set; } // Foreign key naar EquipmentEF

        [ForeignKey("equipment_id")]
        public EquipmentEF Equipment { get; set; } // Navigatie-eigenschap naar EquipmentEF

        public EquipmentOnderhoudEF() { }

        public EquipmentOnderhoudEF(int equipment_id)
        {
            this.equipment_id = equipment_id;
        }
    }
}
