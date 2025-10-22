namespace ContractMonthlyClaimSystem.Models
{
    public class CMCSUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // Lecturer, Co-ordinator, Manager
    }
}
