
using FitnessBL.Exceptions;
using FitnessBL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessEF.Model
{
    public class ProgrammaEF
    {
        [Required]
        [Column(TypeName = "nvarchar(500)")]
        public int Id { get; set; }
        
        [Required]
        public string Naam;

        [Required]
        public string Doelpubliek;

        [Required]
        public DateTime StartDatum { get; set; }

        [Required]
        public int MaxAantal;

        [Required]
        public Dictionary<int, Klant> Klanten = new Dictionary<int, Klant>();

        public ProgrammaEF()
        {
        }
    }
}
