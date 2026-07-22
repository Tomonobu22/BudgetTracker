using BudgetTracker.Core.Data;
using BudgetTracker.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Core.Repositories.Implementations
{
    // Generic repository can handle all entities
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>(); // Get the DbSet for the entity type T
        }
        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllFromImportIdAsync(int importId, string userId)
        {
            return await _dbSet
                .Where(e => EF.Property<int>(e, "ImportId") == importId && EF.Property<string>(e, "UserId") == userId).Include(e => EF.Property<object>(e, "Tag"))
                .ToListAsync();
        }

        public async Task DeleteByImportIdAsync(int importId, string userId)
        {
            var entitiesToDelete = await _dbSet
                .Where(e => EF.Property<int>(e, "ImportId") == importId && EF.Property<string>(e, "UserId") == userId)
                .ToListAsync();
            if (entitiesToDelete.Any())
            {
                _dbSet.RemoveRange(entitiesToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}
