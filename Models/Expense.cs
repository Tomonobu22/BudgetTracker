using Microsoft.AspNetCore.Identity;

namespace BudgetTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DateIncurred { get; set; }
        public int UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
