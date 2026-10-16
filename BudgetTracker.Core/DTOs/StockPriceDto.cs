using System;

namespace BudgetTracker.Core.DTOs
{
    public class StockPriceDto
    {
        public string Ticker { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
