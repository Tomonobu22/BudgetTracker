using BudgetTracker.Data;
using BudgetTracker.Models;
using BudgetTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Repositories.Implementations
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Tag> _dbSet;
        public TagRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Tag>();
        }
        public Task CreateAsync(Tag tag)
        {
            _dbSet.Add(tag);
            return _context.SaveChangesAsync();
        }

        public Task RemoveTagAsync(int tagId, string userId)
        {
            var tag = _dbSet.FirstOrDefault(t => t.Id == tagId && t.UserId == userId);
            if (tag != null)
            {
                _dbSet.Remove(tag);
                return _context.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Tag>> GetAllTagsAsync(string context, string userId)
        {
            var tags = _dbSet.Where(t => t.Context == context && t.UserId == userId);
            return Task.FromResult<IEnumerable<Tag>>(tags.ToList());
        }
    }
}
