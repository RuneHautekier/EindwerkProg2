using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FitnessBL.Enums;

namespace FitnessEF.Model
{
    public class MemberEF
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(45)")]
        public string first_name { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(45)")]
        public string last_name { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? email { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string address { get; set; }

        [Column(TypeName = "date")]
        public DateTime birthday { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? interests { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public string membertype { get; set; }

        public ICollection<ProgramEF> Programs { get; set; } = new List<ProgramEF>();

        public MemberEF() { }

        public MemberEF(
            string first_name,
            string last_name,
            string email,
            string address,
            DateTime birthday,
            string interests,
            string membertype
        )
        {
            this.first_name = first_name;
            this.last_name = last_name;
            this.email = email;
            this.address = address;
            this.birthday = birthday;
            this.interests = interests;
            this.membertype = membertype;
        }

        public MemberEF(
            int member_id,
            string first_name,
            string last_name,
            string email,
            string address,
            DateTime birthday,
            string interests,
            string membertype
        )
        {
            this.member_id = member_id;
            this.first_name = first_name;
            this.last_name = last_name;
            this.email = email;
            this.address = address;
            this.birthday = birthday;
            this.interests = interests;
            this.membertype = membertype;
        }
    }
}
