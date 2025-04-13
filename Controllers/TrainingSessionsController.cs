using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models;

namespace BeFit.Controllers
{
    [Authorize]
    public class TrainingSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrainingSessionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var sessions = await _context.TrainingSessions
                .Where(s => s.UserId == userId)
                .ToListAsync();
            return View(sessions);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var session = await _context.TrainingSessions
                .FirstOrDefaultAsync(s => s.Id == id);
            if (session == null || session.UserId != _userManager.GetUserId(User)) return NotFound();
            return View(session);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartTime,EndTime")] TrainingSession session)
        {
            ModelState.Clear(); // Clear existing model state
            session.UserId = _userManager.GetUserId(User);
            
            if (string.IsNullOrEmpty(session.UserId))
            {
                return Unauthorized();
            }

            TryValidateModel(session); // Revalidate model with UserId

            if (ModelState.IsValid)
            {
                _context.Add(session);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var session = await _context.TrainingSessions.FindAsync(id);
            if (session == null || session.UserId != _userManager.GetUserId(User)) return NotFound();
            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartTime,EndTime")] TrainingSession session)
        {
            if (id != session.Id)
            {
                return NotFound();
            }

            var existing = await _context.TrainingSessions.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existing == null || existing.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            ModelState.Clear();
            session.UserId = existing.UserId;
            TryValidateModel(session);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(session);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.TrainingSessions.AnyAsync(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View(session);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var session = await _context.TrainingSessions.FirstOrDefaultAsync(s => s.Id == id);
            if (session == null || session.UserId != _userManager.GetUserId(User)) return NotFound();
            return View(session);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.TrainingSessions.FindAsync(id);
            if (session == null || session.UserId != _userManager.GetUserId(User)) return NotFound();
            _context.TrainingSessions.Remove(session);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}