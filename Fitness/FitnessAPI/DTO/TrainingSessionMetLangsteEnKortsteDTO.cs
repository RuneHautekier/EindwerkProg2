namespace FitnessAPI.DTO
{
    public class TrainingSessionMetLangsteEnKortsteDTO
    {
        public int AantalTrainingSessions { get; set; }
        public decimal AantalUren { get; set; }
        public TrainingSessionDTO LangsteTrainingSession { get; set; }
        public TrainingSessionDTO KortsteTrainingSession { get; set; }
        public decimal GemiddeldeDuur { get; set; }
    }
}
