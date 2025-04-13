using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeFit.Models
{
    public class PerformedExercise
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Typ ćwiczenia jest wymagany")]
        [Display(Name = "Typ ćwiczenia")]
        public int ExerciseTypeId { get; set; }

        [Required(ErrorMessage = "Sesja treningowa jest wymagana")]
        [Display(Name = "Sesja treningowa")]
        public int TrainingSessionId { get; set; }

        [Required(ErrorMessage = "Obciążenie jest wymagane")]
        [Range(1, 999, ErrorMessage = "Obciążenie musi być między 1 a 999 kg")]
        [Display(Name = "Obciążenie (kg)")]
        public int Weight { get; set; }

        [Required(ErrorMessage = "Liczba serii jest wymagana")]
        [Range(1, 20, ErrorMessage = "Liczba serii musi być między 1 a 20")]
        [Display(Name = "Liczba serii")]
        public int Series { get; set; }

        [Required(ErrorMessage = "Liczba powtórzeń jest wymagana")]
        [Range(1, 100, ErrorMessage = "Liczba powtórzeń musi być między 1 a 100")]
        [Display(Name = "Liczba powtórzeń")]
        public int Repetitions { get; set; }

        // Właściwości nawigacyjne
        [ForeignKey("ExerciseTypeId")]
        public virtual ExerciseType ExerciseType { get; set; }

        [ForeignKey("TrainingSessionId")]
        public virtual TrainingSession TrainingSession { get; set; }
    }
}
