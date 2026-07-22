using BudgetTracker.Core.Data;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Core.Repositories.Implementations
{
    public class ImportRepository : GenericRepository<Import>, IImportRepository
    {
        public ImportRepository(AppDbContext context) : base(context) { }
        public async Task<IEnumerable<Import>> GetAllByUserAsync(string userId)
        {
            return await _dbSet
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.UploadedAt)
                .ToListAsync();
        }

        public override async Task AddAsync(Import import)
        {
            _dbSet.Add(import);
            await _context.SaveChangesAsync();
        }

        public override async Task<Import?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<int> CountImportsByUserAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken)
        {
            var count = await _dbSet
                .Where(i => i.UserId == userId && i.UploadedAt.Date == date.Date)
                .CountAsync(cancellationToken);
            return count;
        }

        public override async Task UpdateAsync(Import import)
        {
            _dbSet.Update(import);
            await _context.SaveChangesAsync();
        }
    }
}
