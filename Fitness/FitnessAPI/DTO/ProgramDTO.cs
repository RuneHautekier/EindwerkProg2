namespace FitnessAPI.DTO
{
    public class ProgramDTO
    {
        public string ProgramCode { get; set; }
        public string Name { get; set; }
        public string Target { get; set; }
        public DateTime StartDate { get; set; }
        public int MaxMembers { get; set; }
    }
}
