using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ContractMonthlyClaimSystem.Models.Claim;


public class ApprovalController : Controller
{
    private readonly AppDbContext _db;
    public ApprovalController(AppDbContext db) { _db = db; }


    public async Task<IActionResult> Index()
    {
        var pending = await _db.Claims.Include(c => c.Documents).Where(c => c.Status == ClaimStatus.Pending).OrderBy(c => c.SubmittedDate).ToListAsync();
        return View(pending);
    }


    public async Task<IActionResult> Review(int id)
    {
        var claim = await _db.Claims.Include(c => c.Documents).Include(c => c.ClaimItems).FirstOrDefaultAsync(c => c.Id == id);
        if (claim == null) return NotFound();
        return View(claim);
    }


    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var claim = await _db.Claims.FindAsync(id); if (claim == null) return NotFound();
        // Simple two-step flow: coordinator approval -> manager approval
        claim.Status = claim.Status == ClaimStatus.Pending ? ClaimStatus.ApprovedByCoordinator : ClaimStatus.ApprovedByManager;
        claim.ApprovedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Message"] = $"Claim #{id} {(claim.Status == ClaimStatus.ApprovedByManager ? "approved by Manager" : "approved by Coordinator")}.";
        return RedirectToAction(nameof(Index));
    }


    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, string reason)
    {
        var claim = await _db.Claims.FindAsync(id); if (claim == null) return NotFound();
        claim.Status = ClaimStatus.Rejected; claim.ApprovedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Message"] = $"Claim #{id} rejected."; // Optionally persist reason in another table/column
        return RedirectToAction(nameof(Index));
    }
}