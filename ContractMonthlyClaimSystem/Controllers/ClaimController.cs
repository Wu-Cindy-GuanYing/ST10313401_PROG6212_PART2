using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Services;
using Microsoft.AspNetCore.Mvc;
using static ContractMonthlyClaimSystem.Models.Claim;


public class ClaimController : Controller
{
    private readonly AppDbContext _db; private readonly LocalFileStorage _fs;
    public ClaimController(AppDbContext db, LocalFileStorage fs) { _db = db; _fs = fs; }


    public async Task<IActionResult> Index()
    {
        // For demo, we don't have Identity; filter by all for now
        var claims = await _db.Claims.Include(c => c.Documents).OrderByDescending(c => c.SubmittedDate).ToListAsync();
        return View(claims);
    }


    public IActionResult Create() => View(new ClaimCreateVm());


    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClaimCreateVm vm, IFormFile? upload, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        var claim = new Claim
        {
            LecturerId = "demo-lecturer", // replace with User.Identity if you wire Identity
            LecturerName = "Demo Lecturer",
            TotalHours = vm.HoursWorked,
            TotalAmount = vm.HoursWorked * vm.HourlyRate,
            Status = ClaimStatus.Pending,
            SubmittedDate = DateTime.UtcNow
        };
        if (upload is { Length: > 0 })
        {
            try
            {
                var (path, size, cty) = await _fs.SaveAsync(upload, "claims", ct);
                claim.Documents.Add(new Document { FileName = upload.FileName, FilePath = path, SizeBytes = size, ContentType = cty });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("upload", ex.Message);
                return View(vm);
            }
        }
        _db.Claims.Add(claim);
        await _db.SaveChangesAsync(ct);
        TempData["Message"] = "Claim submitted.";
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Details(int id)
    {
        var claim = await _db.Claims.Include(c => c.Documents).Include(c => c.ClaimItems).FirstOrDefaultAsync(c => c.Id == id);
        if (claim == null) return NotFound();
        return View(claim);
    }
}