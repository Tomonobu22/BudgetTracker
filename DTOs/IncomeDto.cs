using BudgetTracker.Models;

namespace BudgetTracker.DTOs
{
    public class IncomeDto
    {
        public int Id { get; set; }
        public int? TagId { get; set; }
        public Tag? Tag { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateReceived { get; set; }
    }
}
