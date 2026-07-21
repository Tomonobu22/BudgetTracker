using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Enums;
using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.Services.Interfaces
{
    public interface ITagAppService
    {
        Task RemoveTagAsync(int tagId, string userId);
        Task<IEnumerable<TagDto>> GetAllTagsAsync(RecordType context, string userId);
        Task<int> CreateAsync(TagDto tagDto, string userId);
        Task<int> UpdateAsync(TagDto tagDto, string userId);
        Task<TagDto> GetTagByIdAsync(int tagId, string userId);
    }
}
