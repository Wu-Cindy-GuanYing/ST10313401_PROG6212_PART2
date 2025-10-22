// ViewModels/ClaimCreateVm.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ContractMonthlyClaimSystem.ViewModels
{
    public class ClaimCreateVm
    {
        [Required, Range(typeof(decimal), "0.25", "1000", ErrorMessage = "Hours must be between 0.25 and 1000.")]
        public decimal HoursWorked { get; set; }

        [Required, Range(typeof(decimal), "0", "10000", ErrorMessage = "Rate must be between 0 and 10000.")]
        public decimal HourlyRate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        // bind the upload to this property
        public IFormFile? Upload { get; set; }
    }
}
