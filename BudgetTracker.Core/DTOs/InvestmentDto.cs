using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.DTOs
{
    public class InvestmentDto
    {
        public int Id { get; set; }
        public int? TagId { get; set; }
        public Tag? Tag { get; set; }
        public string? Ticker { get; set; }
        public StockPrice? StockPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Quantity { get; set; }
        public DateTime DateInvested { get; set; }
        public decimal CurrentValue { get; set; }
        public int? ImportId { get; set; }
        public decimal TotalInvestment => PurchasePrice * Quantity;
    }
}
