using FitnessBL.Enums;

namespace FitnessAPI.DTO
{
    public class MemberAanmakenDTO
    {
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public string Email { get; set; }
        public string Adres { get; set; }
        public DateTime Geboortedatum { get; set; }
        public List<string> Interesses { get; set; }
        public TypeKlant TypeKlant { get; set; }
    }
}
