using FitnessBL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FitnessEF.Model
{
    public class KlantEF
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        
        [Required]
        [Column(TypeName = "nvarchar(45)")]
        public string Voornaam { get; set; }
        
        [Required]
        [Column(TypeName = "nvarchar(45)")]
        public string Achternaam { get; set; }
        
        [Column(TypeName = "nvarchar(50)")]
        public string Emailadres { get; set; }
        
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string Verblijfsplaats { get; set; }
        
        [Column(TypeName = "datetime")]
        public DateTime GeboorteDatum { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public List<string> Interesses { get; set; } = new List<string>();
        
        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public TypeKlant Type { get; set; }

        public KlantEF()
        {
        }
    }
}
