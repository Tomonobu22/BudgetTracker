using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTracker.Core.Models
{
    public class StockPrice
    {
        [Key]
        [MaxLength(20)]
        public string Ticker { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,6)")]
        public decimal Price { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = string.Empty;

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
