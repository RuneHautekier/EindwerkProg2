public class TrainingSessionDTO
{
    public string SessionType { get; set; } // "Running" of "Cycling"
    public int Id { get; set; } // Het ID van de sessie
    public DateTime Date { get; set; } // De datum van de sessie
    public int Duration { get; set; } // De duur van de sessie

    // Velden specifiek voor RunningSession
    public float? AvgSpeed { get; set; }

    // Velden specifiek voor CyclingSession

    public int? AvgWatt { get; set; }
    public int? MaxWatt { get; set; }
    public int? AvgCadence { get; set; }
    public int? MaxCadence { get; set; }
    public string TrainingsType { get; set; }
    public string Comment { get; set; }
    public string? TrainingsImpact { get; set; }
}
