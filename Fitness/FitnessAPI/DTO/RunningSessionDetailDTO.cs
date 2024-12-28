namespace FitnessAPI.DTO
{
    public class RunningSessionDetailDTO
    {
        public RunningSessionMainDTO MainSession { get; set; }
        public int SequalNumber { get; set; }
        public int IntervalTime { get; set; }
        public int IntervalSpeed { get; set; }
    }
}
