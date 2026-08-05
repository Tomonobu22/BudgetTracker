using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Enums;
using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.Services.Interfaces
{
    public interface ITagAppService
    {
        Task RemoveTagAsync(int tagId, string userId);
        Task<IEnumerable<TagDto>> GetAllTagsAsync(RecordType context, string userId);
        Task<PagedResultDto<TagDto>> GetPagedByUserAsync(string userId, PagingRequestDto request, string? searchTerm = null, RecordType? context = RecordType.Empty);
        Task<int> CreateAsync(TagDto tagDto, string userId);
        Task<int> UpdateAsync(TagDto tagDto, string userId);
        Task<TagDto> GetTagByIdAsync(int tagId, string userId);
    }
}
