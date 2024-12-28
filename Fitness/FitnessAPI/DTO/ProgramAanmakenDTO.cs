namespace FitnessAPI.DTO
{
    public class ProgramAanmakenDTO
    {
        public string ProgramCode { get; set; }
        public string Name { get; set; }
        public string Target { get; set; }
        public DateTime Startdate { get; set; }
        public int Max_members { get; set; }
    }
}
