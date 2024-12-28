namespace FitnessAPI.DTO
{
    public class RunningSessionMainDTO
    {
        public int RunningSessionId { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public float AverageSpeed { get; set; }
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
