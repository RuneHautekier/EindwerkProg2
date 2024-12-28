using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace FitnessAPI.DTO
{
    public class RunningSessionMainAanmakenDTO
    {
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public float AverageSpeed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
