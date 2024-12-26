namespace FitnessAPI.DTO
{
    public class CyclingSessionAanmakenDTO
    {
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public int AverageWattage { get; set; }
        public int MaxWattage { get; set; }
        public int AverageCadence { get; set; }
        public int MaxCadence { get; set; }
        public string TrainingsType { get; set; }
        public string Comment { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
    }
}
