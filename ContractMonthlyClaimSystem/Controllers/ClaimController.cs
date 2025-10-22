﻿// Controllers/ClaimController.cs
using System.Text.RegularExpressions;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimController : Controller
    {
        private readonly AppDbContext _db;

        public ClaimController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var claims = await _db.Claims
                .Include(c => c.ClaimItems)
                .Include(c => c.Documents)
                .OrderByDescending(c => c.SubmittedDate)
                .ToListAsync();

            return View(claims);
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
            Console.WriteLine("=== CLAIM SUBMISSION STARTED ===");

            // --- File validation for multiple files ---
            if (vm.Uploads != null && vm.Uploads.Count > 0)
            {
                Console.WriteLine($"Number of files: {vm.Uploads.Count}");
                var allowed = new List<string> { ".pdf", ".docx", ".xlsx" };
                const long maxBytes = 10 * 1024 * 1024;

                foreach (var file in vm.Uploads)
                {
                    Console.WriteLine($"File: {file.FileName}, Size: {file.Length} bytes");
                    if (file.Length > 0)
                    {
                        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                        Console.WriteLine($"File extension: {ext}");

                        if (!allowed.Contains(ext))
                        {
                            ModelState.AddModelError(nameof(vm.Uploads), $"File '{file.FileName}': Only PDF, DOCX, or XLSX files are allowed.");
                        }

                        if (file.Length > maxBytes)
                        {
                            ModelState.AddModelError(nameof(vm.Uploads), $"File '{file.FileName}' must be 10MB or smaller.");
                        }
                    }
                }
            }

            Console.WriteLine($"ModelState isValid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model validation failed");
                return View(vm);
            }

            try
            {
                Console.WriteLine("Starting database operations...");

                var lecturerId = 0;
                var lecturerName = User?.Identity?.Name ?? "Cindy Wu";
                Console.WriteLine($"Lecturer: {lecturerName} (ID: {lecturerId})");

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

                Console.WriteLine($"Claim created: Hours={vm.HoursWorked}, Rate={vm.HourlyRate}, Total={claim.TotalAmount}");

                claim.ClaimItems.Add(new ClaimItem
                {
                    Date = DateTime.UtcNow.Date,
                    Hours = vm.HoursWorked,
                    Rate = vm.HourlyRate,
                    Description = vm.Notes ?? string.Empty
                });

                Console.WriteLine("Claim item added");

                // --- Save files to database ---
                if (vm.Uploads != null && vm.Uploads.Count > 0)
                {
                    Console.WriteLine("Processing files...");
                    foreach (var file in vm.Uploads.Where(f => f.Length > 0))
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        var fileContent = memoryStream.ToArray();

                        claim.Documents.Add(new Document
                        {
                            FileName = GenerateSafeFileName(file.FileName),
                            OriginalFileName = file.FileName,
                            FileContent = fileContent,
                            ContentType = file.ContentType,
                            SizeBytes = file.Length,
                            UploadedDate = DateTime.UtcNow
                        });
                        Console.WriteLine($"Document added: {file.FileName}, Size: {file.Length} bytes");
                    }
                }

                Console.WriteLine("Adding claim to database context...");
                _db.Claims.Add(claim);

                Console.WriteLine("Saving changes to database...");
                var result = await _db.SaveChangesAsync();
                Console.WriteLine($"SaveChangesAsync completed. Result: {result} rows affected");
                Console.WriteLine($"New claim ID: {claim.Id}");

                TempData["Message"] = "Claim submitted successfully.";
                Console.WriteLine("=== CLAIM SUBMISSION COMPLETED SUCCESSFULLY ===");

                return RedirectToAction(nameof(Details), new { id = claim.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }

                ModelState.AddModelError("", "An error occurred while submitting the claim. Please try again.");
                return View(vm);
            }
        }

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

        [HttpGet]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            var document = await _db.Documents.FindAsync(id);
            if (document == null || document.FileContent == null)
                return NotFound();

            return File(document.FileContent, document.ContentType ?? "application/octet-stream", document.OriginalFileName);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = Claim.ClaimStatus.ApprovedByCoordinator;
            claim.ApprovedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            TempData["Message"] = "Claim approved successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = Claim.ClaimStatus.Rejected;

            await _db.SaveChangesAsync();

            TempData["Message"] = "Claim rejected successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                // Test if we can read from database
                var count = await _db.Claims.CountAsync();

                // Test if we can write to database
                var testClaim = new Claim
                {
                    LecturerId = 999,
                    LecturerName = "Test User",
                    Month = DateTime.UtcNow,
                    TotalHours = 1,
                    TotalAmount = 100,
                    Status = Claim.ClaimStatus.Pending,
                    SubmittedDate = DateTime.UtcNow
                };

                _db.Claims.Add(testClaim);
                var result = await _db.SaveChangesAsync();

                TempData["Message"] = $"Database test successful! Existing claims: {count}, New claim ID: {testClaim.Id}";
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Database test failed: {ex.Message}";
                return RedirectToAction("Create");
            }
        }

        private static string GenerateSafeFileName(string originalFileName)
        {
            var baseName = Path.GetFileNameWithoutExtension(originalFileName);
            var ext = Path.GetExtension(originalFileName);
            var safeBase = Regex.Replace(baseName, @"[^a-zA-Z0-9\-_]+", "-").Trim('-');
            return $"{safeBase}-{Guid.NewGuid():N}{ext}";
        }


    }
}