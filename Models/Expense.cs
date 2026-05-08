using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public int? TagId { get; set; }
        public Tag? Tag { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DateIncurred { get; set; }
        public string UserId { get; set; }
    }
}
