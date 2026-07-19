using BudgetTracker.Core.Models;

namespace BudgetTracker.Core.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetByTokenAsync(string token);
    }
}
