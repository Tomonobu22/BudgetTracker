namespace BudgetTracker.Models
{
    public class Investment
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DateInvested { get; set; }
        public decimal CurrentValue { get; set; }
    }
}
