namespace BudgetTracker.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty; // e.g., "Income", "Expense", "Investment"
        public string UserId { get; set; } = string.Empty;
    }
}
