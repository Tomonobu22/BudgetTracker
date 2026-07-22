using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.DTOs
{
    public class IncomeDto
    {
        public int Id { get; set; }
        public int? TagId { get; set; }
        public Tag? Tag { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DateReceived { get; set; }
    }
}
