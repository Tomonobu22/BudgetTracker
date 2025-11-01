using BudgetTracker.Models;

namespace BudgetTracker.DTOs
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        public int? TagId { get; set; }
        public Tag? Tag { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DateIncurred { get; set; }
    }
}
