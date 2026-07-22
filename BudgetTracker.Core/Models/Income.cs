using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTracker.Core.Models
{
    public class Income
    {
        public int Id { get; set; }
        public int? TagId { get; set; }
        public Tag? Tag { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DateReceived { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
