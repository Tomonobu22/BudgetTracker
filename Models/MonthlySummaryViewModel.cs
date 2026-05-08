namespace BudgetTracker.Models
{
    public class MonthlySummaryViewModel
    {
        public MonthlySummaryViewModel() { }
        public List<decimal> MonthlyIncome { get; set; } = new();
        public List<decimal> MonthlyExpenses { get; set; } = new();
        public List<decimal> MonthlyInvestments { get; set; } = new();

        public List<string> MonthNames { get; set; } = new()
        {
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };

        public decimal TotalIncome => MonthlyIncome.Sum();
        public decimal TotalExpenses => MonthlyExpenses.Sum();
        public decimal TotalInvestments => MonthlyInvestments.Sum();
        public int Year { get; set; }
    }
}
