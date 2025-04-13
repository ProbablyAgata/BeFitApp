namespace BeFit.Models
{
    public class ExerciseStatViewModel
    {
        public string ExerciseName { get; set; }
        public int TotalSessions { get; set; }
        public int TotalRepetitions { get; set; }
        public double AverageWeight { get; set; }
        public int MaxWeight { get; set; }
        public DateTime LastTrainingDate { get; set; }
        public List<double> Progress { get; set; }
        public double WeightProgress { get; set; }
    }
} 