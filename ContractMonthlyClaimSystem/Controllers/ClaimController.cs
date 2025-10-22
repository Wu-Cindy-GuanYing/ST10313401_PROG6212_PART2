// Controllers/ClaimsController.cs
using System.Text.RegularExpressions;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ClaimController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ClaimCreateVm();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClaimCreateVm vm)
        {
            // --- File validation (optional but recommended) ---
            if (vm.Upload != null)
            {
                var allowed = new[] { ".pdf", ".docx", ".xlsx" };
                var ext = Path.GetExtension(vm.Upload.FileName).ToLowerInvariant();

                if (!allowed.Contains(ext))
                    ModelState.AddModelError(nameof(vm.Upload), "Only PDF, DOCX, or XLSX files are allowed.");

                const long maxBytes = 10 * 1024 * 1024; // 10 MB
                if (vm.Upload.Length > maxBytes)
                    ModelState.AddModelError(nameof(vm.Upload), "File must be 10MB or smaller.");
            }

            if (!ModelState.IsValid)
                return View(vm);

            // --- Map to domain entities ---
            // TODO: Replace with your actual user resolution
            var lecturerId = 0; // e.g., from your CMCSUser
            var lecturerName = User?.Identity?.Name ?? "Unknown";

            var claim = new Claim
            {
                LecturerId = lecturerId,
                LecturerName = lecturerName,
                Month = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                TotalHours = vm.HoursWorked,
                TotalAmount = vm.HoursWorked * vm.HourlyRate,
                Status = Claim.ClaimStatus.Pending,
                SubmittedDate = DateTime.UtcNow
            };

            claim.ClaimItems.Add(new ClaimItem
            {
                Date = DateTime.UtcNow.Date,
                Hours = vm.HoursWorked,
                Rate = vm.HourlyRate,
                Description = vm.Notes ?? string.Empty
            });

            // --- Save file if present ---
            if (vm.Upload != null && vm.Upload.Length > 0)
            {
                var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "claim");
                Directory.CreateDirectory(uploadsRoot);

                var ext = Path.GetExtension(vm.Upload.FileName);
                var baseName = Path.GetFileNameWithoutExtension(vm.Upload.FileName);
                // sanitize filename
                var safeBase = Regex.Replace(baseName, @"[^a-zA-Z0-9\-_]+", "-").Trim('-');
                var finalName = $"{safeBase}-{Guid.NewGuid():N}{ext}";
                var fullPath = Path.Combine(uploadsRoot, finalName);

                await using (var stream = System.IO.File.Create(fullPath))
                {
                    await vm.Upload.CopyToAsync(stream);
                }

                claim.Documents.Add(new Document
                {
                    FileName = finalName,
                    FilePath = Path.Combine("uploads", "claim", finalName).Replace("\\", "/"),
                    ContentType = vm.Upload.ContentType,
                    SizeBytes = vm.Upload.Length,
                    UploadedDate = DateTime.UtcNow
                });
            }

            _db.Claims.Add(claim);
            await _db.SaveChangesAsync();

            TempData["Message"] = "Claim submitted successfully.";
            return RedirectToAction(nameof(Details), new { id = claim.Id });
        }

        // Simple landing page after submit
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var claim = await _db.Claims
                .Include(c => c.ClaimItems)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (claim == null) return NotFound();
            return View(claim);
        }
    }
}
