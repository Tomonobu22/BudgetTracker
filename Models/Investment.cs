using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTracker.Models
{
    public class Investment
    {
        public int Id { get; set; }
        public int? TagId { get; set; }
        public Tag? Tag { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateInvested { get; set; }
        public decimal CurrentValue { get; set; }
        public string UserId { get; set; }
    }
}
