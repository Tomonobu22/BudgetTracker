using BudgetTracker.Enums;
namespace BudgetTracker.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public RecordType Context { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
