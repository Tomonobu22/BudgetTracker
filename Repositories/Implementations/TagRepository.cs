using BudgetTracker.Data;
using BudgetTracker.Enums;
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

        public Task<IEnumerable<Tag>> GetAllTagsAsync(RecordType context, string userId)
        {
            if (context == RecordType.Empty)
            {
                var allTags = _dbSet.Where(t => t.UserId == userId);
                return Task.FromResult<IEnumerable<Tag>>(allTags.ToList());
            }
            else
            {
                var tags = _dbSet.Where(t => t.Context == context && t.UserId == userId);
                return Task.FromResult<IEnumerable<Tag>>(tags.ToList());
            }
        }
        public Task<Tag?> GetTagByIdAsync(int tagId)
        {
            return _dbSet.FirstOrDefaultAsync(t => t.Id == tagId);
        }
        public async Task UpdateAsync(Tag tag)
        {
            var existingTag = await _dbSet.FirstOrDefaultAsync(t => t.Id == tag.Id);
            if (existingTag != null)
            {
                existingTag.Name = tag.Name;
                existingTag.Context = tag.Context;
                _dbSet.Update(existingTag);
                await _context.SaveChangesAsync();
            }
        }
    }
}
