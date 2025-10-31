namespace BudgetTracker.Models
{
    public class ReportSummaryViewModel
    {
        public ReportSummaryViewModel() { }
        public decimal TotalExpenses { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalInvestments { get; set; }
        public decimal NetBalance => TotalIncome - TotalExpenses;
        public int Year { get; set; }
    }
}
