using BudgetTracker.DTOs;
using BudgetTracker.Models;

namespace BudgetTracker.Services.Interfaces
{
    public interface ITagAppService
    {
        Task RemoveTagAsync(int tagId, string userId);
        Task<IEnumerable<Tag>> GetAllTagsAsync(string context, string userId);
        Task<int> CreateAsync(TagDto tagDto, string userId);
    }
}
