using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Security.Claims;

namespace BeFit.Controllers
{
    [Authorize]
    public class PerformedExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PerformedExercisesController> _logger;

        public PerformedExercisesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<PerformedExercisesController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Przeprowadzone ćwiczenia
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var exercises = await _context.PerformedExercises
                .Include(e => e.ExerciseType)
                .Include(e => e.TrainingSession)
                .Where(e => e.TrainingSession.UserId == userId)
                .OrderByDescending(e => e.TrainingSession.StartTime)
                .ToListAsync();

            return View(exercises);
        }

        // GET: Przeprowadzone ćwiczenia/Szczegóły/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var performedExercise = await _context.PerformedExercises
                .Include(e => e.ExerciseType)
                .Include(e => e.TrainingSession)
                .FirstOrDefaultAsync(e => e.Id == id && e.TrainingSession.UserId == userId);

            if (performedExercise == null)
            {
                return NotFound();
            }

            return View(performedExercise);
        }

        // GET: Przeprowadzone ćwiczenia/Utwórz
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            
            // Pobierz typy ćwiczeń
            var exerciseTypes = await _context.ExerciseTypes
                .OrderBy(e => e.Name)
                .ToListAsync();
            
            // Pobierz sesje treningowe dla aktualnego użytkownika
            var trainingSessions = await _context.TrainingSessions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.StartTime)
                .ToListAsync();

            if (!trainingSessions.Any())
            {
                TempData["Error"] = "Musisz najpierw utworzyć sesję treningową.";
                return RedirectToAction("Create", "TrainingSessions");
            }
            
            ViewData["ExerciseTypeId"] = new SelectList(exerciseTypes, "Id", "Name");
            ViewData["TrainingSessionId"] = new SelectList(
                trainingSessions.Select(t => new
                {
                    Id = t.Id,
                    FormattedDate = t.StartTime.ToString("dd.MM.yyyy HH:mm")
                }),
                "Id",
                "FormattedDate");

            return View();
        }

        // POST: Przeprowadzone ćwiczenia/Utwórz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] PerformedExercise performedExercise)
        {
            var userId = _userManager.GetUserId(User);
            _logger.LogInformation("Create POST action started. UserId: {UserId}", userId);
            _logger.LogInformation("Model state: {@ModelState}", ModelState);
            _logger.LogInformation("Received exercise data: {@PerformedExercise}", performedExercise);

            // Wyczyść istniejące błędy stanu modelu
            ModelState.Clear();

            // Ręczna walidacja wymaganych pól
            if (performedExercise.ExerciseTypeId == 0)
            {
                ModelState.AddModelError("ExerciseTypeId", "Typ ćwiczenia jest wymagany");
            }

            if (performedExercise.TrainingSessionId == 0)
            {
                ModelState.AddModelError("TrainingSessionId", "Sesja treningowa jest wymagana");
            }

            if (performedExercise.Weight < 1 || performedExercise.Weight > 999)
            {
                ModelState.AddModelError("Weight", "Obciążenie musi być między 1 a 999 kg");
            }

            if (performedExercise.Series < 1 || performedExercise.Series > 20)
            {
                ModelState.AddModelError("Series", "Liczba serii musi być między 1 a 20");
            }

            if (performedExercise.Repetitions < 1 || performedExercise.Repetitions > 100)
            {
                ModelState.AddModelError("Repetitions", "Liczba powtórzeń musi być między 1 a 100");
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model is valid. Exercise data: {@PerformedExercise}", performedExercise);

                // Sprawdź, czy sesja treningowa należy do aktualnego użytkownika
                var trainingSession = await _context.TrainingSessions
                    .FirstOrDefaultAsync(t => t.Id == performedExercise.TrainingSessionId && t.UserId == userId);

                if (trainingSession == null)
                {
                    _logger.LogWarning("Training session not found or doesn't belong to user. SessionId: {SessionId}, UserId: {UserId}", 
                        performedExercise.TrainingSessionId, userId);
                    ModelState.AddModelError("TrainingSessionId", "Nieprawidłowa sesja treningowa.");
                }
                else
                {
                    try
                    {
                        // Utwórz nowe Przeprowadzone ćwiczenie z tylko niezbędnymi właściwościami
                        var newExercise = new PerformedExercise
                        {
                            ExerciseTypeId = performedExercise.ExerciseTypeId,
                            TrainingSessionId = performedExercise.TrainingSessionId,
                            Weight = performedExercise.Weight,
                            Series = performedExercise.Series,
                            Repetitions = performedExercise.Repetitions
                        };

                        _context.Add(newExercise);
                await _context.SaveChangesAsync();
                        _logger.LogInformation("Exercise saved successfully. Id: {Id}", newExercise.Id);
                TempData["Success"] = "Ćwiczenie zostało dodane pomyślnie.";

                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            var redirectUrl = Url.Action("Index", "PerformedExercises", null, "http");
                            _logger.LogInformation($"Redirect URL: {redirectUrl}");
                            return Json(new { success = true, redirect = redirectUrl });
                        }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                        _logger.LogError(ex, "Error saving exercise: {Message}", ex.Message);
                ModelState.AddModelError("", "Wystąpił błąd podczas zapisywania ćwiczenia.");
                    }
                }
            }
            else
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                );
                _logger.LogWarning("Model validation failed. Errors: {@Errors}", errors);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, errors = errors });
                }
            }

            // Jeśli dotarliśmy tutaj, coś poszło nie tak - przygotuj ponownie dane widoku
            var exerciseTypes = await _context.ExerciseTypes
                .OrderBy(e => e.Name)
                .ToListAsync();
            
            var trainingSessions = await _context.TrainingSessions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.StartTime)
                .ToListAsync();

            ViewData["ExerciseTypeId"] = new SelectList(exerciseTypes, "Id", "Name", performedExercise.ExerciseTypeId);
            ViewData["TrainingSessionId"] = new SelectList(
                trainingSessions.Select(t => new
                {
                    Id = t.Id,
                    FormattedDate = t.StartTime.ToString("dd.MM.yyyy HH:mm")
                }),
                "Id",
                "FormattedDate",
                performedExercise.TrainingSessionId);
            
            return View(performedExercise);
        }

        // GET: Przeprowadzone ćwiczenia/Edytuj/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var performedExercise = await _context.PerformedExercises
                .Include(e => e.TrainingSession)
                .FirstOrDefaultAsync(e => e.Id == id && e.TrainingSession.UserId == userId);

            if (performedExercise == null)
            {
                return NotFound();
            }

            var exerciseTypes = await _context.ExerciseTypes
                .OrderBy(e => e.Name)
                .ToListAsync();
            
            var trainingSessions = await _context.TrainingSessions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.StartTime)
                .ToListAsync();

            ViewData["ExerciseTypeId"] = new SelectList(exerciseTypes, "Id", "Name", performedExercise.ExerciseTypeId);
            ViewData["TrainingSessionId"] = new SelectList(
                trainingSessions.Select(t => new
                {
                    Id = t.Id,
                    FormattedDate = t.StartTime.ToString("dd.MM.yyyy HH:mm")
                }),
                "Id",
                "FormattedDate",
                performedExercise.TrainingSessionId);

            return View(performedExercise);
        }

        // POST: Przeprowadzone ćwiczenia/Edytuj/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] PerformedExercise performedExercise)
        {
            _logger.LogInformation("Edit action started for exercise ID: {Id}", id);

            if (id != performedExercise.Id)
            {
                _logger.LogWarning("ID mismatch: URL ID {UrlId} != Form ID {FormId}", id, performedExercise.Id);
                return NotFound();
            }

            // Wyczyść istniejący ModelState do obsługi ręcznej walidacji
            ModelState.Clear();

            // Ręczna walidacja
            if (performedExercise.ExerciseTypeId <= 0)
            {
                ModelState.AddModelError("ExerciseTypeId", "Proszę wybrać rodzaj ćwiczenia.");
            }
            if (performedExercise.TrainingSessionId <= 0)
            {
                ModelState.AddModelError("TrainingSessionId", "Proszę wybrać sesję treningową.");
            }
            if (performedExercise.Weight <= 0)
            {
                ModelState.AddModelError("Weight", "Ciężar musi być większy od 0.");
            }
            if (performedExercise.Series <= 0)
            {
                ModelState.AddModelError("Series", "Liczba serii musi być większa od 0.");
            }
            if (performedExercise.Repetitions <= 0)
            {
                ModelState.AddModelError("Repetitions", "Liczba powtórzeń musi być większa od 0.");
            }

            _logger.LogInformation("Received exercise data: {@Exercise}", new
            {
                performedExercise.Id,
                performedExercise.ExerciseTypeId,
                performedExercise.TrainingSessionId,
                performedExercise.Weight,
                performedExercise.Series,
                performedExercise.Repetitions
            });

            try
            {
                // Pobierz istniejące ćwiczenie
                var existingExercise = await _context.PerformedExercises
                    .Include(p => p.TrainingSession)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (existingExercise == null)
                {
                    _logger.LogWarning("Exercise with ID {Id} not found", id);
                    return NotFound();
                }

                // Sprawdź własność
                if (existingExercise.TrainingSession?.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    _logger.LogWarning("Unauthorized access attempt to edit exercise {Id}", id);
                    return Unauthorized();
                }

                // Sprawdź, czy sesja treningowa istnieje i należy do użytkownika
                var trainingSession = await _context.TrainingSessions
                    .FirstOrDefaultAsync(ts => ts.Id == performedExercise.TrainingSessionId);

                if (trainingSession == null)
                {
                    _logger.LogWarning("Training session {Id} not found", performedExercise.TrainingSessionId);
                    ModelState.AddModelError("TrainingSessionId", "Wybrana sesja treningowa nie istnieje.");
                }
                else if (trainingSession.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    _logger.LogWarning("Unauthorized training session access attempt");
                    ModelState.AddModelError("TrainingSessionId", "Nie masz dostępu do wybranej sesji treningowej.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model validation failed: {@Errors}", 
                        ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, errors = ModelState.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                        )});
                    }

                    ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name");
                    ViewData["TrainingSessionId"] = new SelectList(_context.TrainingSessions, "Id", "Name");
                    return View(performedExercise);
                }

                // Zaktualizuj tylko niezbędne właściwości
                existingExercise.ExerciseTypeId = performedExercise.ExerciseTypeId;
                existingExercise.TrainingSessionId = performedExercise.TrainingSessionId;
                existingExercise.Weight = performedExercise.Weight;
                existingExercise.Series = performedExercise.Series;
                existingExercise.Repetitions = performedExercise.Repetitions;

                _context.Update(existingExercise);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Exercise {Id} updated successfully", id);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, redirect = Url.Action("Index") });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exercise {Id}", id);
                ModelState.AddModelError("", "Wystąpił błąd podczas zapisywania zmian. Spróbuj ponownie.");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, errors = new { Error = "Wystąpił błąd podczas zapisywania zmian. Spróbuj ponownie." } });
                }

                ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name");
                ViewData["TrainingSessionId"] = new SelectList(_context.TrainingSessions, "Id", "Name");
                return View(performedExercise);
            }
        }

        // GET: Przeprowadzone ćwiczenia/Usuń/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var performedExercise = await _context.PerformedExercises
                .Include(e => e.ExerciseType)
                .Include(e => e.TrainingSession)
                .FirstOrDefaultAsync(e => e.Id == id && e.TrainingSession.UserId == userId);

            if (performedExercise == null)
            {
                return NotFound();
            }

            return View(performedExercise);
        }

        // POST: Przeprowadzone ćwiczenia/Usuń/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var performedExercise = await _context.PerformedExercises
                .Include(e => e.TrainingSession)
                .FirstOrDefaultAsync(e => e.Id == id && e.TrainingSession.UserId == userId);

            if (performedExercise == null)
            {
                return NotFound();
            }

            _context.PerformedExercises.Remove(performedExercise);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Ćwiczenie zostało usunięte pomyślnie.";
            return RedirectToAction(nameof(Index));
        }

        private bool PerformedExerciseExists(int id)
        {
            return _context.PerformedExercises.Any(e => e.Id == id);
        }
    }
}