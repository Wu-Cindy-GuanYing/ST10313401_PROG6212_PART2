using System.ComponentModel.DataAnnotations;


namespace ContractMonthlyClaimSystem.Models
{
    public class ClaimItem
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        [DataType(DataType.Date)] public DateTime Date { get; set; }
        [Range(0.25, 24)] public decimal Hours { get; set; }
        [Range(0, 10000)] public decimal Rate { get; set; }
        [MaxLength(500)] public string Description { get; set; } = string.Empty;
    }
}