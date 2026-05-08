using BudgetTracker.DTOs;
using BudgetTracker.Enums;
using BudgetTracker.Models;

namespace BudgetTracker.Services.Interfaces
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
