using BudgetTracker.Enums;
using BudgetTracker.Models;

namespace BudgetTracker.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task CreateAsync(Tag tag);
        Task RemoveTagAsync(int tagId, string userId);
        Task UpdateAsync(Tag tag);
        Task<IEnumerable<Tag>> GetAllTagsAsync(RecordType context, string userId);
        Task<Tag?> GetTagByIdAsync(int tagId);
    }
}
