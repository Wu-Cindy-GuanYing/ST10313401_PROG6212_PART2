﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // if you use [Column]
using Microsoft.EntityFrameworkCore;               // for [Precision]

namespace ContractMonthlyClaimSystem.Models
{
    public class Claim
    {
        public enum ClaimStatus { Pending = 0, ApprovedByCoordinator = 1, ApprovedByManager = 2, Rejected = 3, Paid = 4 }

        public int Id { get; set; }

        public int LecturerId { get; set; } // no [Required] needed

        [Required, MaxLength(200)]
        public string LecturerName { get; set; } = string.Empty;

        // If you're on EF Core 7+, prefer DateOnly for a month anchor, or keep DateTime and map to DATE:
        [DataType(DataType.Date)]
        // [Column(TypeName = "DATE")] // For Oracle if you want date-only
        public DateTime Month { get; set; } = DateTime.UtcNow;

        [Precision(9, 2)]
        [Range(typeof(decimal), "0", "1000")]
        public decimal TotalHours { get; set; }

        [Precision(18, 2)]
        [Range(typeof(decimal), "0", "9999999")]
        public decimal TotalAmount { get; set; }

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedDate { get; set; }

        public List<ClaimItem> ClaimItems { get; set; } = new();
        public List<Document> Documents { get; set; } = new();
    }
}
