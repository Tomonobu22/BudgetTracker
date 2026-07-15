using BudgetTracker.Enums;

namespace BudgetTracker.DTOs
{
    public class ImportDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string BlobName { get; set; } = string.Empty;
        public ImportStatus Status { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}