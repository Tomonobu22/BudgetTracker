using BudgetTracker.Models;

namespace BudgetTracker.DTOs
{
    public class InvestmentDto
    {
        public int Id { get; set; }
        public int? TagId { get; set; }
        public Tag? Tag { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateInvested { get; set; }
        public decimal CurrentValue { get; set; }
    }
}
