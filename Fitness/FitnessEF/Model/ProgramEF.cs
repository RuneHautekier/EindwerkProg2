using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessEF.Model
{
    public class ProgramEF
    {
        [Key]
        [Column(TypeName = "nvarchar(10)")]
        public string programCode { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(45)")]
        public string name { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(25)")]
        public string target { get; set; }

        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime startdate { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int max_members { get; set; }

        // Navigation property for many-to-many relationship
        public ICollection<MemberEF> Members { get; set; } = new List<MemberEF>();

        public ProgramEF() { }

        public ProgramEF(string name, string target, DateTime startdate, int max_members)
        {
            this.name = name;
            this.target = target;
            this.startdate = startdate;
            this.max_members = max_members;
        }

        public ProgramEF(
            string programCode,
            string name,
            string target,
            DateTime startdate,
            int max_members,
            List<MemberEF> members
        )
        {
            this.programCode = programCode;
            this.name = name;
            this.target = target;
            this.startdate = startdate;
            this.max_members = max_members;
            Members = members;
        }
    }
}
