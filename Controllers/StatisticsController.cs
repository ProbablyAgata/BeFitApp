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
                    TotalSessions = g.Count(),
                    TotalRepetitions = g.Sum(x => x.Series * x.Repetitions),
                    AverageWeight = Math.Round(g.Average(x => x.Weight), 1),
                    MaxWeight = g.Max(x => x.Weight),
                    LastTrainingDate = g.Max(x => x.TrainingSession.StartTime),
                    Progress = g.OrderByDescending(x => x.TrainingSession.StartTime)
                                .Take(2)
                                .Select(x => (double)x.Weight)
                                .ToList()
                })
                .OrderByDescending(s => s.LastTrainingDate)
                .ToListAsync();

            // Calculate progress indicators
            foreach (var stat in stats)
            {
                if (stat.Progress.Count >= 2)
                {
                    var latest = stat.Progress[0];
                    var previous = stat.Progress[1];
                    stat.WeightProgress = latest - previous;
                }
            }

            return View(stats);
        }
    }
}