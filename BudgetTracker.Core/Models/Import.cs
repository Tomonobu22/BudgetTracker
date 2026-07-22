using BudgetTracker.Core.Enums;

namespace BudgetTracker.Core.Models
{
    public class Import
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string BlobName { get; set; } = string.Empty;
        public ImportStatus Status { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public RecordType ImportType { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
