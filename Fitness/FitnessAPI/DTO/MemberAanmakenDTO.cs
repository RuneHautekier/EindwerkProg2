using FitnessBL.Enums;

namespace FitnessAPI.DTO
{
    public class MemberAanmakenDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime Birthday { get; set; }
        public List<string> Intrests { get; set; }
        public TypeKlant TypeMember { get; set; }
    }
}
