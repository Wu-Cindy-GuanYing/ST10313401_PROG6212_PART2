using Xunit;
using ContractMonthlyClaimSystem.Models;
namespace ICClaimTests
{
    public class ClaimTests
    {
        [Fact]
        public void CalculateTotalAmount()
        {
            var claim = new ClaimItem();
            claim.Hours = 20;
            claim.Rate = 670;

            var getResult = claim.CalculateTotalAmount();

            Assert.Equal(13400, getResult);
        }

    }
}
