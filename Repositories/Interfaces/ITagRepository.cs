using BudgetTracker.Models;
namespace BudgetTracker.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task CreateAsync(Tag tag);
        Task RemoveTagAsync(int tagId, string userId);
        Task<IEnumerable<Tag>> GetAllTagsAsync(string context, string userId);
    }
}
