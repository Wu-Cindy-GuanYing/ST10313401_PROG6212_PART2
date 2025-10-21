using System.ComponentModel.DataAnnotations;


namespace ContractMonthlyClaimSystem.Models
{
    public class Claim
    {
        public enum ClaimStatus { Pending = 0, ApprovedByCoordinator = 1, ApprovedByManager = 2, Rejected = 3, Paid = 4 }
        public int Id { get; set; }
        [Required] public string LecturerId { get; set; } = default!;
        [Required] public string LecturerName { get; set; } = default!;
        [DataType(DataType.Date)] public DateTime Month { get; set; } = DateTime.UtcNow;
        [Range(0, 1000)] public decimal TotalHours { get; set; }
        [Range(0, 9999999)] public decimal TotalAmount { get; set; }
        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedDate { get; set; }
        public List<ClaimItem> ClaimItems { get; set; } = new();
        public List<Document> Documents { get; set; } = new();
    }
}

