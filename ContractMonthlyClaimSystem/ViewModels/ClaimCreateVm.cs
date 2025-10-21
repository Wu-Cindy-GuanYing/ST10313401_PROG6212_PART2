using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.ViewModels
{
    public class ClaimCreateVm
    {
        [Required, Range(0.25, 1000)] public decimal HoursWorked { get; set; }
        [Required, Range(0, 10000)] public decimal HourlyRate { get; set; }
        [MaxLength(1000)] public string? Notes { get; set; }
    }
}