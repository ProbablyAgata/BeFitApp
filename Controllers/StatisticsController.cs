using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models;

namespace BeFit.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StatisticsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var fourWeeksAgo = DateTime.Now.AddDays(-28);

            var stats = await _context.PerformedExercises
                .Include(e => e.ExerciseType)
                .Include(e => e.TrainingSession)
                .Where(e => e.TrainingSession.UserId == userId && e.TrainingSession.StartTime >= fourWeeksAgo)
                .GroupBy(e => e.ExerciseType.Name)
                .Select(g => new ExerciseStatViewModel
                {
                    ExerciseName = g.Key,
                    Count = g.Count(),
                    TotalRepetitions = g.Sum(x => x.Series * x.Repetitions),
                    AverageWeight = g.Average(x => x.Weight),
                    MaxWeight = g.Max(x => x.Weight)
                })
                .ToListAsync();

            return View(stats);
        }
    }

    public class ExerciseStatViewModel
    {
        public string ExerciseName { get; set; }
        public int Count { get; set; }
        public int TotalRepetitions { get; set; }
        public double AverageWeight { get; set; }
        public int MaxWeight { get; set; }
    }
}