namespace BudgetTracker.DTOs
{
    public class IncomeDto
    {
        public int Id { get; set; }
        public string Source { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DateReceived { get; set; }
    }
}
