using BudgetTracker.Enums;

namespace BudgetTracker.DTOs
{
    public class TagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public RecordType Context { get; set; } = RecordType.Empty;
    }
}
