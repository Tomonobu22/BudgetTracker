using BudgetTracker.Core.Data;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Core.Repositories.Implementations
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;
        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);
            return refreshToken;
        }
    }
}
