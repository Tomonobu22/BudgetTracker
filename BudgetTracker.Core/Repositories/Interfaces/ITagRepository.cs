using BudgetTracker.Core.Enums;
using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task CreateAsync(Tag tag);
        Task RemoveTagAsync(int tagId, string userId);
        Task UpdateAsync(Tag tag);
        Task<IEnumerable<Tag>> GetAllTagsAsync(RecordType context, string userId);
        Task<Tag?> GetTagByIdAsync(int tagId);
        Task<Tag?> GetTagByNameAsync(string name, RecordType context, string userId);
    }
}
