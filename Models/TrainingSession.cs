using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeFit.Models
{
    public class TrainingSession
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Proszę wybrać datę i godzinę rozpoczęcia treningu")]
        [Display(Name = "Początek sesji")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Proszę wybrać datę i godzinę zakończenia treningu")]
        [Display(Name = "Koniec sesji")]
        public DateTime? EndTime { get; set; }

        [Required]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}